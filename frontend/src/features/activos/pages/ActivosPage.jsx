import { useState } from 'react'
import { useOutletContext, useSearchParams } from 'react-router'

import { Button } from '@/components/ui/button'
import Paginacion from '@/components/shared/Paginacion'
import ActivosTabla from '@/features/activos/components/ActivosTabla'
import ActivosFiltros from '@/features/activos/components/ActivosFiltros'
import ActivoFormDialog from '@/features/activos/components/ActivoFormDialog'

import { useActivos } from '@/hooks/useActivos'
import { useCategorias } from '@/hooks/useCategorias'
import { getErrorMessage } from '@/utils/apiError'

export default function ActivosPage() {
    const [searchParams, setSearchParams] = useSearchParams()
    const [formulario, setFormulario] = useState(null)
    const { user } = useOutletContext()

    const puedeAdministrar = user.rol === 'Administrador'

    const puedeConsultarCategorias =
        puedeAdministrar || user.rol === 'Coordinador/Técnico'

    const requestedPage = Number(searchParams.get('page') ?? 1)

    const page =
        Number.isInteger(requestedPage) &&
            requestedPage >= 1 &&
            requestedPage <= 2147483647
            ? requestedPage
            : 1

    const filtros = {
        texto: searchParams.get('texto') ?? '',
        categoriaActivoId: searchParams.get('categoriaActivoId') ?? '',
        estado: searchParams.get('estado') ?? '',
        kilometrajeDesde: searchParams.get('kilometrajeDesde') ?? '',
        kilometrajeHasta: searchParams.get('kilometrajeHasta') ?? '',
        sortBy: searchParams.get('sortBy') ?? 'nombre',
        sortDirection: searchParams.get('sortDirection') ?? 'asc',
        page,
        pageSize: 10,
    }

    const { data, loading, error, reload } = useActivos(filtros)
    const activos = data?.items ?? []

    const {
        categorias,
        loading: loadingCategorias,
        error: errorCategorias,
        reload: recargarCategorias,
    } = useCategorias(puedeConsultarCategorias)

    function cambiarPagina(nextPage) {
        setSearchParams((current) => {
            const next = new URLSearchParams(current)
            next.set('page', String(nextPage))
            return next
        })
    }

    function actualizarFiltros(changes) {
        setSearchParams((current) => {
            const next = new URLSearchParams(current)

            for (const [key, value] of Object.entries(changes)) {
                if (value === '' || value === null || value === undefined) {
                    next.delete(key)
                } else {
                    next.set(key, String(value))
                }
            }

            next.delete('page')
            return next
        })
    }

    function cerrarFormulario() {
        setFormulario(null)
    }

    function alGuardar() {
        setFormulario(null)
        reload()
    }

    return (
        <section className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold">Activos</h1>
                    <p className="text-muted-foreground">
                        Consulta y administración de los activos de la flota.
                    </p>
                </div>

                <div className="flex flex-wrap gap-2">
                    <Button
                        variant="outline"
                        onClick={reload}
                        disabled={loading}
                    >
                        Actualizar
                    </Button>

                    {puedeAdministrar && (
                        <Button onClick={() => setFormulario({ activoId: null })}>
                            Nuevo activo
                        </Button>
                    )}
                </div>
            </div>

            <ActivosFiltros
                filtros={filtros}
                categorias={categorias}
                mostrarCategorias={puedeConsultarCategorias}
                loadingCategorias={loadingCategorias}
                onChange={actualizarFiltros}
                onClear={() => setSearchParams({})}
            />

            {errorCategorias && (
                <div role="alert" className="space-y-2">
                    <p>
                        No se pudieron cargar las categorías:{' '}
                        {getErrorMessage(errorCategorias)}
                    </p>

                    <Button variant="outline" onClick={recargarCategorias}>
                        Reintentar categorías
                    </Button>
                </div>
            )}

            {error ? (
                <div className="space-y-3" role="alert">
                    <p>{getErrorMessage(error)}</p>

                    <Button variant="outline" onClick={reload}>
                        Reintentar
                    </Button>
                </div>
            ) : !loading && activos.length === 0 ? (
                <div className="space-y-3">
                    <p>No se encontraron activos para esta consulta.</p>

                    <Button
                        variant="outline"
                        onClick={() => setSearchParams({})}
                    >
                        Restablecer consulta
                    </Button>
                </div>
            ) : (
                <>
                    <ActivosTabla
                        activos={activos}
                        loading={loading}
                        onEditar={
                            puedeAdministrar
                                ? (activo) => setFormulario({ activoId: activo.id })
                                : undefined
                        }
                    />

                    {!loading && (
                        <Paginacion
                            page={data.page}
                            totalPages={data.totalPages}
                            totalCount={data.totalCount}
                            onPageChange={cambiarPagina}
                        />
                    )}
                </>
            )}

            {formulario && (
                <ActivoFormDialog
                    key={formulario.activoId ?? 'nuevo'}
                    activoId={formulario.activoId}
                    onClose={cerrarFormulario}
                    onSaved={alGuardar}
                />
            )}
        </section>
    )
}