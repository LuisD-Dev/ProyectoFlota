import { useState } from 'react'
import { useOutletContext } from 'react-router'

import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table'

import OrdenFormDialog from '@/features/ordenes/components/OrdenFormDialog'
import { useOrdenes } from '@/hooks/useOrdenes'
import { getErrorMessage } from '@/utils/apiError'

const estados = {
    0: 'Solicitada',
    1: 'Diagnosticada',
    2: 'Aprobada',
    3: 'Rechazada',
    4: 'En ejecución',
    5: 'En espera de repuesto',
    6: 'Completada',
    7: 'Cancelada',
}

function mostrarFecha(valor) {
    if (!valor) return 'Sin fecha'

    const fecha = new Date(valor)

    return Number.isNaN(fecha.getTime())
        ? 'Fecha no disponible'
        : fecha.toLocaleString('es-CR')
}

export default function OrdenesPage() {
    const { user } = useOutletContext()
    const { ordenes, loading, error, reload } = useOrdenes()
    const [mostrarFormulario, setMostrarFormulario] = useState(false)

    const puedeSolicitar = [
        'Administrador',
        'Solicitante/Consulta',
    ].includes(user.rol)

    function solicitudGuardada() {
        setMostrarFormulario(false)
        reload()
    }

    return (
        <section className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold">
                        Órdenes de mantenimiento
                    </h1>

                    <p className="text-sm text-muted-foreground">
                        Consulta de órdenes y registro de solicitudes.
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

                    {puedeSolicitar && (
                        <Button
                            onClick={() => setMostrarFormulario(true)}
                        >
                            Nueva solicitud
                        </Button>
                    )}
                </div>
            </div>

            {loading ? (
                <p role="status">Cargando órdenes...</p>
            ) : error ? (
                <div role="alert" className="space-y-3">
                    <p className="text-destructive">
                        {getErrorMessage(error)}
                    </p>

                    <Button variant="outline" onClick={reload}>
                        Reintentar
                    </Button>
                </div>
            ) : ordenes.length === 0 ? (
                <p>No hay órdenes de mantenimiento para mostrar.</p>
            ) : (
                <div className="overflow-x-auto rounded-md border">
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Código</TableHead>
                                <TableHead>Activo</TableHead>
                                <TableHead>Motivo</TableHead>
                                <TableHead>Tipo</TableHead>
                                <TableHead>Prioridad</TableHead>
                                <TableHead>Fecha de solicitud</TableHead>
                                <TableHead>Estado</TableHead>
                            </TableRow>
                        </TableHeader>

                        <TableBody>
                            {ordenes.map((orden) => (
                                <TableRow key={orden.id}>
                                    <TableCell className="font-medium">
                                        {orden.codigo}
                                    </TableCell>

                                    <TableCell>
                                        <p className="font-medium">
                                            {orden.activoCodigo}
                                        </p>
                                        <p className="text-sm text-muted-foreground">
                                            {orden.activoNombre}
                                        </p>
                                    </TableCell>

                                    <TableCell className="max-w-xs whitespace-normal">
                                        {orden.motivo}
                                    </TableCell>

                                    <TableCell>
                                        {orden.tipoMantenimiento}
                                    </TableCell>

                                    <TableCell>
                                        {orden.prioridad}
                                    </TableCell>

                                    <TableCell>
                                        {mostrarFecha(orden.fechaSolicitud)}
                                    </TableCell>

                                    <TableCell>
                                        <Badge variant="outline">
                                            {estados[orden.estadoActual] ??
                                                'Desconocido'}
                                        </Badge>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </div>
            )}

            {mostrarFormulario && puedeSolicitar && (
                <OrdenFormDialog
                    onClose={() => setMostrarFormulario(false)}
                    onSaved={solicitudGuardada}
                />
            )}
        </section>
    )
}