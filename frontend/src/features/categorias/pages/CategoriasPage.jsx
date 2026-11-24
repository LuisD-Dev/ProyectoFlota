import { useState } from 'react'
import { useOutletContext } from 'react-router'
import { toast } from 'sonner'

import { Button } from '@/components/ui/button'
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table'

import ConfirmDialog from '@/components/shared/ConfirmDialog'
import CategoriaFormDialog from '@/features/categorias/components/CategoriaFormDialog'
import { useCategorias } from '@/hooks/useCategorias'
import { categoriaService } from '@/services/categoriaService'
import { getErrorMessage } from '@/utils/apiError'

export default function CategoriasPage() {
    const { user } = useOutletContext()

    const puedeAdministrar = user.rol === 'Administrador'
    const puedeConsultar =
        puedeAdministrar || user.rol === 'Coordinador/Técnico'

    const { categorias, loading, error, reload } =
        useCategorias(puedeConsultar)

    const [formulario, setFormulario] = useState(null)
    const [categoriaEliminar, setCategoriaEliminar] = useState(null)
    const [eliminando, setEliminando] = useState(false)
    const [errorEliminar, setErrorEliminar] = useState(null)

    function cerrarFormulario() {
        setFormulario(null)
    }

    function categoriaGuardada() {
        cerrarFormulario()
        reload()
    }

    function solicitarEliminar(categoria) {
        setErrorEliminar(null)
        setCategoriaEliminar(categoria)
    }

    function cancelarEliminar() {
        if (eliminando) return
        setCategoriaEliminar(null)
        setErrorEliminar(null)
    }

    async function eliminar() {
        if (!categoriaEliminar || eliminando) return

        setEliminando(true)
        setErrorEliminar(null)

        try {
            await categoriaService.remove(categoriaEliminar.id)
            toast.success('Categoría eliminada correctamente.')
            setCategoriaEliminar(null)
            reload()
        } catch (error) {
            setErrorEliminar(getErrorMessage(error))
        } finally {
            setEliminando(false)
        }
    }

    if (!puedeConsultar) {
        return (
            <p role="alert">
                No tiene permisos para consultar las categorías.
            </p>
        )
    }

    return (
        <section className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold">
                        Categorías de activo
                    </h1>
                    <p className="text-sm text-muted-foreground">
                        Clasificación e intervalos de mantenimiento de los activos.
                    </p>
                </div>

                <div className="flex gap-2">
                    <Button
                        variant="outline"
                        onClick={reload}
                        disabled={loading}
                    >
                        Actualizar
                    </Button>

                    {puedeAdministrar && (
                        <Button
                            onClick={() =>
                                setFormulario({ categoria: null })
                            }
                        >
                            Nueva categoría
                        </Button>
                    )}
                </div>
            </div>

            {loading ? (
                <p role="status">Cargando categorías...</p>
            ) : error ? (
                <div role="alert" className="space-y-3">
                    <p className="text-destructive">
                        {getErrorMessage(error)}
                    </p>
                    <Button variant="outline" onClick={reload}>
                        Reintentar
                    </Button>
                </div>
            ) : categorias.length === 0 ? (
                <p>No hay categorías registradas.</p>
            ) : (
                <div className="overflow-x-auto rounded-md border">
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Nombre</TableHead>
                                <TableHead>Descripción</TableHead>
                                <TableHead className="text-right">
                                    Intervalo de mantenimiento
                                </TableHead>

                                {puedeAdministrar && (
                                    <TableHead>Acciones</TableHead>
                                )}
                            </TableRow>
                        </TableHeader>

                        <TableBody>
                            {categorias.map((categoria) => (
                                <TableRow key={categoria.id}>
                                    <TableCell className="font-medium">
                                        {categoria.nombre}
                                    </TableCell>

                                    <TableCell className="max-w-sm whitespace-normal">
                                        {categoria.descripcion ||
                                            'Sin descripción'}
                                    </TableCell>

                                    <TableCell className="text-right">
                                        {categoria.intervaloMantenimientoKm.toLocaleString(
                                            'es-CR',
                                        )}{' '}
                                        km
                                    </TableCell>

                                    {puedeAdministrar && (
                                        <TableCell>
                                            <div className="flex gap-2">
                                                <Button
                                                    variant="outline"
                                                    size="sm"
                                                    onClick={() =>
                                                        setFormulario({
                                                            categoria,
                                                        })
                                                    }
                                                >
                                                    Editar
                                                </Button>

                                                <Button
                                                    variant="outline"
                                                    size="sm"
                                                    className="text-destructive"
                                                    onClick={() =>
                                                        solicitarEliminar(
                                                            categoria,
                                                        )
                                                    }
                                                >
                                                    Eliminar
                                                </Button>
                                            </div>
                                        </TableCell>
                                    )}
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </div>
            )}

            {formulario !== null && (
                <CategoriaFormDialog
                    categoria={formulario.categoria}
                    onClose={cerrarFormulario}
                    onSaved={categoriaGuardada}
                />
            )}

            <ConfirmDialog
                open={categoriaEliminar !== null}
                title="Eliminar categoría"
                description={
                    categoriaEliminar
                        ? `¿Desea eliminar la categoría "${categoriaEliminar.nombre}"? Solo se puede eliminar si no tiene activos asociados.`
                        : ''
                }
                confirmText="Eliminar"
                loading={eliminando}
                error={errorEliminar}
                onConfirm={eliminar}
                onCancel={cancelarEliminar}
            />
        </section>
    )
}