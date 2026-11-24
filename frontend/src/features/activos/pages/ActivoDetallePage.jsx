import { Link, useOutletContext, useParams } from 'react-router'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import EstadoActivoBadge from '@/features/activos/components/EstadoActivoBadge'
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'

import ActivoImagen from '@/features/activos/components/ActivoImagen'
import { useActivo } from '@/hooks/useActivo'
import { getErrorMessage } from '@/utils/apiError'
import ActivoAcciones from '@/features/activos/components/ActivoAcciones'



const estadosOrden = {
    0: 'Solicitada',
    1: 'Diagnosticada',
    2: 'Aprobada',
    3: 'Rechazada',
    4: 'En ejecución',
    5: 'En espera de repuesto',
    6: 'Completada',
    7: 'Cancelada',
}

function Dato({ label, children }) {
    return (
        <div>
            <dt className="text-sm text-muted-foreground">{label}</dt>
            <dd className="mt-1 font-medium">{children}</dd>
        </div>
    )
}

export default function ActivoDetallePage() {
    const { id } = useParams()
    const { user } = useOutletContext()

    const puedeAdministrar = user.rol === 'Administrador'

    const parsedId = Number(id)
    const validId =
        /^\d+$/.test(id ?? '') &&
        Number.isInteger(parsedId) &&
        parsedId >= 1 &&
        parsedId <= 2147483647

    const { activo, loading, error, reload } = useActivo(
        validId ? parsedId : null,
    )

    if (!validId) {
        return (
            <section className="space-y-4">
                <p role="alert">El identificador del activo no es válido.</p>

                <Button asChild variant="outline">
                    <Link to="/activos">Volver a activos</Link>
                </Button>
            </section>
        )
    }

    if (loading) {
        return <p role="status">Cargando detalle del activo...</p>
    }

    if (error) {
        return (
            <section className="space-y-4">
                <p role="alert">{getErrorMessage(error)}</p>

                <div className="flex gap-2">
                    <Button variant="outline" onClick={reload}>
                        Reintentar
                    </Button>

                    <Button asChild variant="outline">
                        <Link to="/activos">Volver a activos</Link>
                    </Button>
                </div>
            </section>
        )
    }

    if (!activo) {
        return <p>No se encontró información del activo.</p>
    }

    return (
        <section className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold">{activo.nombre}</h1>
                    <p className="text-muted-foreground">{activo.codigo}</p>
                </div>

                <Button asChild variant="outline">
                    <Link to="/activos">Volver a activos</Link>
                </Button>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Información del activo</CardTitle>
                </CardHeader>

                <CardContent>
                    <dl className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
                        <Dato label="Estado">
                            <EstadoActivoBadge estado={activo.estado} />
                        </Dato>

                        <Dato label="Categoría">
                            {activo.categoriaActivoNombre}
                        </Dato>

                        <Dato label="Placa">
                            {activo.placa || 'Sin placa'}
                        </Dato>

                        <Dato label="Fecha de adquisición">
                            {activo.fechaAdquisicion}
                        </Dato>

                        <Dato label="Antigüedad">
                            {activo.antiguedadAnios} años
                        </Dato>

                        <Dato label="Kilometraje">
                            {activo.kilometrajeActual.toLocaleString('es-CR')} km
                        </Dato>

                        <Dato label="Valor de referencia">
                            {activo.valorReferencia.toLocaleString('es-CR', {
                                style: 'currency',
                                currency: 'CRC',
                            })}
                        </Dato>

                        <Dato label="Intervalo de mantenimiento de la categoría">
                            {activo.categoria.intervaloMantenimientoKm.toLocaleString(
                                'es-CR',
                            )} km
                        </Dato>

                        <Dato label="Cantidad de órdenes">
                            {activo.cantidadOrdenes}
                        </Dato>
                    </dl>

                    <div className="mt-6">
                        <h2 className="font-medium">Descripción</h2>

                        <p className="mt-1 whitespace-pre-wrap text-muted-foreground">
                            {activo.descripcion || 'Sin descripción.'}
                        </p>
                    </div>
                </CardContent>
            </Card>

            <ActivoImagen
                key={`${activo.id}-${activo.imagenUrl}`}
                activo={activo}
                puedeAdministrar={puedeAdministrar}
                onUpdated={reload}
            />
            {puedeAdministrar && (
                <ActivoAcciones
                    activo={activo}
                    onUpdated={reload}
                />
            )}

            <Card>
                <CardHeader>
                    <CardTitle>Órdenes asociadas</CardTitle>
                </CardHeader>

                <CardContent>
                    {activo.ordenes.length === 0 ? (
                        <p>No hay órdenes disponibles para este activo.</p>
                    ) : (
                        <ul className="space-y-3">
                            {activo.ordenes.map((orden) => (
                                <li
                                    key={orden.codigo}
                                    className="rounded-md border p-4"
                                >
                                    <div className="flex flex-wrap items-center justify-between gap-2">
                                        <span className="font-medium">
                                            {orden.codigo}
                                        </span>

                                        <Badge variant="outline">
                                            {estadosOrden[orden.estadoActual] ?? 'Desconocido'}
                                        </Badge>
                                    </div>

                                    <p className="mt-2 text-sm">
                                        Tipo: {orden.tipoMantenimiento} · Prioridad:{' '}
                                        {orden.prioridad}
                                    </p>

                                    <p className="mt-1 text-sm text-muted-foreground">
                                        Fecha de solicitud: {orden.fechaSolicitud}
                                    </p>
                                </li>
                            ))}
                        </ul>
                    )}
                </CardContent>
            </Card>
        </section>
    )
}