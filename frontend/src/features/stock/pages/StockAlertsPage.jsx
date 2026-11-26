import { useOutletContext } from 'react-router'

import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table'

import { useStockAlerts } from '@/hooks/useStockAlerts'

function mostrarCantidad(valor) {
    return Number(valor).toLocaleString('es-CR', {
        maximumFractionDigits: 2,
    })
}

function mostrarFecha(valor) {
    if (!valor) return 'Sin registrar'

    const fecha = new Date(valor)

    return Number.isNaN(fecha.getTime())
        ? 'Fecha no disponible'
        : fecha.toLocaleString('es-CR')
}

export default function StockAlertsPage() {
    const { user } = useOutletContext()

    const puedeConsultar = [
        'Administrador',
        'Coordinador/Técnico',
    ].includes(user.rol)

    const {
        estadoConexion,
        errorConexion,
        alertas,
        repuestos,
        errorStock,
        fechaConsulta,
        loading,
        actualizarStock,
        reintentarConexion,
    } = useStockAlerts(puedeConsultar)

    if (!puedeConsultar) {
        return (
            <p role="alert" className="text-destructive">
                No tiene permisos para consultar las alertas de inventario.
            </p>
        )
    }

    const conectado = estadoConexion === 'Conectado'

    return (
        <section className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold">
                        Alertas de inventario
                    </h1>

                    <p className="text-sm text-muted-foreground">
                        Avisos de repuestos que pasan a stock bajo mínimo.
                    </p>
                </div>

                <div className="flex flex-wrap items-center gap-2">
                    <Badge
                        variant="outline"
                        className={
                            conectado
                                ? 'border-emerald-300 bg-emerald-50 text-emerald-800'
                                : 'border-amber-300 bg-amber-50 text-amber-800'
                        }
                        role="status"
                    >
                        {estadoConexion}
                    </Badge>

                    {estadoConexion === 'Desconectado' && (
                        <Button
                            variant="outline"
                            onClick={reintentarConexion}
                        >
                            Reintentar conexión
                        </Button>
                    )}
                </div>
            </div>

            {errorConexion && (
                <p
                    role="alert"
                    className="rounded-md border border-destructive/30 p-3 text-sm text-destructive"
                >
                    {errorConexion}
                </p>
            )}

            <Card>
                <CardHeader>
                    <CardTitle>
                        Avisos recibidos en esta sesión
                    </CardTitle>

                    <p className="text-sm text-muted-foreground">
                        Últimos 20 eventos recibidos mientras esta vista
                        permanece abierta.
                    </p>
                </CardHeader>

                <CardContent>
                    <div
                        aria-live="polite"
                        aria-relevant="additions"
                        className="space-y-3"
                    >
                        {alertas.length === 0 ? (
                            <p className="text-sm text-muted-foreground">
                                Todavía no se han recibido alertas.
                                La lista de stock bajo se consulta por separado.
                            </p>
                        ) : (
                            alertas.map((alerta) => (
                                <article
                                    key={alerta.eventoId}
                                    className="rounded-md border border-amber-300 bg-amber-50 p-4 text-amber-950"
                                >
                                    <h2 className="font-semibold">
                                        {alerta.codigo} — {alerta.nombre}
                                    </h2>

                                    <p className="mt-1 text-sm">
                                        El stock pasó de{' '}
                                        {mostrarCantidad(
                                            alerta.stockAnterior,
                                        )}{' '}
                                        a{' '}
                                        {mostrarCantidad(
                                            alerta.stockActual,
                                        )}
                                        . Mínimo:{' '}
                                        {mostrarCantidad(
                                            alerta.stockMinimo,
                                        )}
                                        .
                                    </p>

                                    <p className="mt-2 text-xs">
                                        {mostrarFecha(alerta.fechaUtc)}
                                    </p>
                                </article>
                            ))
                        )}
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <div className="flex flex-wrap items-center justify-between gap-3">
                        <CardTitle>
                            Stock bajo consultado
                        </CardTitle>

                        <Button
                            variant="outline"
                            onClick={actualizarStock}
                            disabled={loading}
                        >
                            {loading
                                ? 'Consultando...'
                                : 'Actualizar inventario'}
                        </Button>
                    </div>

                    <p className="text-sm text-muted-foreground">
                        Última consulta: {mostrarFecha(fechaConsulta)}.
                        Se actualiza al recibir una alerta o recuperar
                        la conexión. También puede actualizarlo manualmente.
                    </p>
                </CardHeader>

                <CardContent>
                    {loading ? (
                        <p role="status">Consultando inventario...</p>
                    ) : errorStock ? (
                        <p role="alert" className="text-destructive">
                            {errorStock}
                        </p>
                    ) : repuestos.length === 0 ? (
                        <p>No hay repuestos bajo mínimo.</p>
                    ) : (
                        <div className="overflow-x-auto rounded-md border">
                            <Table>
                                <TableHeader>
                                    <TableRow>
                                        <TableHead>Código</TableHead>
                                        <TableHead>Repuesto</TableHead>
                                        <TableHead className="text-right">
                                            Stock actual
                                        </TableHead>
                                        <TableHead className="text-right">
                                            Stock mínimo
                                        </TableHead>
                                    </TableRow>
                                </TableHeader>

                                <TableBody>
                                    {repuestos.map((repuesto) => (
                                        <TableRow key={repuesto.id}>
                                            <TableCell className="font-medium">
                                                {repuesto.codigo}
                                            </TableCell>

                                            <TableCell>
                                                {repuesto.nombre}
                                            </TableCell>

                                            <TableCell className="text-right">
                                                {mostrarCantidad(
                                                    repuesto.stockActual,
                                                )}
                                            </TableCell>

                                            <TableCell className="text-right">
                                                {mostrarCantidad(
                                                    repuesto.stockMinimo,
                                                )}
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </div>
                    )}
                </CardContent>
            </Card>
        </section>
    )
}