import { useOutletContext, useSearchParams } from 'react-router'
import { Button } from '@/components/ui/button'
import ActivosTabla from '@/features/activos/components/ActivosTabla'
import Paginacion from '@/components/shared/Paginacion'
import { useActivos } from '@/hooks/useActivos'
import { getErrorMessage } from '@/utils/apiError'
import ActivosFiltros from '@/features/activos/components/ActivosFiltros'
import { useCategorias } from '@/hooks/useCategorias'

export default function ActivosPage() {
    const [searchParams, setSearchParams] = useSearchParams()

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
    const { user } = useOutletContext()

    const puedeConsultarCategorias =
        user.rol === 'Administrador' ||
        user.rol === 'Coordinador/Técnico'

    const {
        categorias,
        loading: loadingCategorias,
        error: errorCategorias,
        reload: recargarCategorias,
    } = useCategorias(puedeConsultarCategorias)
    return (
        <section className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold">Activos</h1>
                    <p className="text-muted-foreground">
                        Consulta de los activos de la flota.
                    </p>
                </div>

                <Button
                    variant="outline"
                    onClick={reload}
                    disabled={loading}
                >
                    Actualizar
                </Button>
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
                        No se pudieron cargar las categorías: {getErrorMessage(errorCategorias)}
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
                    <ActivosTabla activos={activos} loading={loading} />

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
        </section>
    )
}