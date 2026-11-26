import { Link, useParams } from 'react-router'

import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'

import { useOrden } from '@/hooks/useOrden'
import { getErrorMessage } from '@/utils/apiError'
import { parseApiUtcDate } from '@/utils/format'

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
    if (!valor) return 'Sin registrar'

    const fecha = parseApiUtcDate(valor)

    return Number.isNaN(fecha.getTime())
        ? 'Fecha no disponible'
        : fecha.toLocaleString('es-CR')
}

function Dato({ titulo, children }) {
    return (
        <div className="space-y-1">
            <dt className="text-sm text-muted-foreground">
                {titulo}
            </dt>
            <dd className="whitespace-pre-wrap break-words font-medium">
                {children}
            </dd>
        </div>
    )
}

export default function OrdenDetallePage() {
    const { id } = useParams()
    const numeroId = Number(id)

    const idValido =
        /^[1-9]\d*$/.test(id ?? '') &&
        Number.isInteger(numeroId) &&
        numeroId <= 2147483647

    const { orden, loading, error, reload } = useOrden(
        idValido ? numeroId : null,
    )

    return (
        <section className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold">
                        Detalle de orden
                    </h1>

                    <p className="text-sm text-muted-foreground">
                        Información de la solicitud de mantenimiento.
                    </p>
                </div>

                <Link
                    to="/ordenes"
                    className="text-sm underline underline-offset-4"
                >
                    Volver a órdenes
                </Link>
            </div>

            {!idValido ? (
                <p role="alert" className="text-destructive">
                    El identificador de la orden no es válido.
                </p>
            ) : loading ? (
                <p role="status">Cargando orden...</p>
            ) : error ? (
                <div role="alert" className="space-y-3">
                    <p className="text-destructive">
                        {getErrorMessage(error)}
                    </p>

                    <Button variant="outline" onClick={reload}>
                        Reintentar
                    </Button>
                </div>
            ) : !orden ? (
                <p>No se encontró información de la orden.</p>
            ) : (
                <>
                    <Card>
                        <CardHeader>
                            <CardTitle>{orden.codigo}</CardTitle>
                        </CardHeader>

                        <CardContent>
                            <dl className="grid gap-5 sm:grid-cols-2">
                                <Dato titulo="Estado">
                                    <Badge variant="outline">
                                        {estados[orden.estadoActual] ??
                                            'Desconocido'}
                                    </Badge>
                                </Dato>

                                <Dato titulo="Prioridad">
                                    {orden.prioridad}
                                </Dato>

                                <Dato titulo="Tipo de mantenimiento">
                                    {orden.tipoMantenimiento}
                                </Dato>

                                <Dato titulo="Días desde la solicitud">
                                    {orden.diasTranscurridosDesdeSolicitud}
                                </Dato>

                                <Dato titulo="Fecha de solicitud">
                                    {mostrarFecha(orden.fechaSolicitud)}
                                </Dato>

                                <Dato titulo="Fecha programada">
                                    {mostrarFecha(orden.fechaProgramada)}
                                </Dato>

                                <Dato titulo="Fecha de cierre">
                                    {mostrarFecha(orden.fechaCierre)}
                                </Dato>

                                <Dato titulo="Costo total">
                                    {new Intl.NumberFormat('es-CR', {
                                        style: 'currency',
                                        currency: 'CRC',
                                    }).format(orden.costoTotal)}
                                </Dato>

                                <div className="sm:col-span-2">
                                    <Dato titulo="Motivo">
                                        {orden.motivo}
                                    </Dato>
                                </div>
                            </dl>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle>Activo asociado</CardTitle>
                        </CardHeader>

                        <CardContent>
                            <dl className="grid gap-5 sm:grid-cols-2">
                                <Dato titulo="Código">
                                    <Link
                                        to={`/activos/${orden.activoId}`}
                                        className="underline underline-offset-4"
                                    >
                                        {orden.activoCodigo}
                                    </Link>
                                </Dato>

                                <Dato titulo="Nombre">
                                    {orden.activoNombre}
                                </Dato>
                            </dl>
                        </CardContent>
                    </Card>
                </>
            )}
        </section>
    )
}