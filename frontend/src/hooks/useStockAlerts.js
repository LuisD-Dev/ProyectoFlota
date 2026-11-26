import { useEffect, useRef, useState } from 'react'

import { stockAlertService } from '@/services/stockAlertService'
import { getErrorMessage } from '@/utils/apiError'
import { tokenStorage } from '@/utils/tokenStorage'

export function useStockAlerts(enabled = true) {
    const [intento, setIntento] = useState(0)
    const [consultaVersion, setConsultaVersion] = useState(0)

    const [conexion, setConexion] = useState({
        intento: null,
        estado: 'Conectando',
        error: null,
    })

    const [consulta, setConsulta] = useState({
        version: null,
        repuestos: [],
        error: null,
        fecha: null,
    })

    const [alertas, setAlertas] = useState([])
    const eventosRecibidos = useRef(new Set())

    // Conexión y recepción de eventos de SignalR.
    useEffect(() => {
        if (!enabled) return

        let activo = true
        let connection

        function cambiarEstado(estado, error = null) {
            if (!activo) return

            setConexion({
                intento,
                estado,
                error,
            })
        }

        function recibirAlerta(alerta) {
            if (!activo) return

            const eventoId = alerta.eventoId

            if (eventosRecibidos.current.has(eventoId)) return

            eventosRecibidos.current.add(eventoId)

            // Mantener acotada la memoria de eventos identificados.
            if (eventosRecibidos.current.size > 200) {
                const primero =
                    eventosRecibidos.current.values().next().value

                eventosRecibidos.current.delete(primero)
            }

            setAlertas((anteriores) =>
                [alerta, ...anteriores].slice(0, 20),
            )

            // Consultar el inventario después de recibir el aviso.
            setConsultaVersion((version) => version + 1)
        }

        async function suscribirse() {
            await connection.invoke('SuscribirseStock')

            if (!activo) return

            cambiarEstado('Conectado')

            // Recuperar el estado actual tras conectar o reconectar.
            setConsultaVersion((version) => version + 1)
        }

        async function detenerConexion() {
            try {
                await connection?.stop()
            } catch {
                // La limpieza no debe dejar una promesa rechazada sin manejar.
            }
        }

        function sesionRevocada() {
            if (!activo) return
            cambiarEstado('Desconectado', 'La sesión perdió el permiso para recibir alertas.')
            void detenerConexion()
            // Reutilizar el cierre de sesión que ya atiende ProtectedRoute.
            tokenStorage.clear()
            window.dispatchEvent(new Event('auth:expired'))
        }

        async function iniciar() {
            // StrictMode desmonta inmediatamente el primer efecto en desarrollo.
            // Esperar un microturno evita negociar una conexión ya descartada.
            await Promise.resolve()
            if (!activo) return

            try {
                connection = stockAlertService.crearConexion()

                // Registrar el receptor antes de iniciar la conexión.
                connection.on(
                    'StockBajoDetectado',
                    recibirAlerta,
                )

                connection.on('StockSesionRevocada', sesionRevocada)

                connection.onreconnecting(() => {
                    cambiarEstado('Reconectando')
                })

                connection.onreconnected(async () => {
                    try {
                        await suscribirse()
                    } catch {
                        cambiarEstado(
                            'Desconectado',
                            'No se pudo recuperar la suscripción. Reintentá la conexión.',
                        )

                        await detenerConexion()
                    }
                })

                connection.onclose(() => {
                    cambiarEstado(
                        'Desconectado',
                        'La conexión se cerró. Podés reintentar.',
                    )
                })

                await connection.start()

                if (!activo) {
                    await detenerConexion()
                    return
                }

                await suscribirse()
            } catch {
                cambiarEstado(
                    'Desconectado',
                    'No se pudo conectar con las alertas. Revisá que la API esté iniciada y reintentá.',
                )

                if (connection) {
                    await detenerConexion()
                }
            }
        }

        void iniciar()

        return () => {
            activo = false

            if (connection) {
                connection.off(
                    'StockBajoDetectado',
                    recibirAlerta,
                )

                connection.off('StockSesionRevocada', sesionRevocada)
                void detenerConexion()
            }
        }
    }, [enabled, intento])

    // Consulta HTTP del inventario bajo mínimo.
    useEffect(() => {
        if (!enabled) return

        const controller = new AbortController()

        stockAlertService
            .getStockBajo({
                signal: controller.signal,
            })
            .then((repuestos) => {
                if (controller.signal.aborted) return

                setConsulta({
                    version: consultaVersion,
                    repuestos,
                    error: null,
                    fecha: new Date(),
                })
            })
            .catch((error) => {
                if (controller.signal.aborted) return

                setConsulta({
                    version: consultaVersion,
                    repuestos: [],
                    error: getErrorMessage(error),
                    fecha: null,
                })
            })

        return () => controller.abort()
    }, [enabled, consultaVersion])

    const conexionActual =
        conexion.intento === intento
            ? conexion
            : {
                estado: 'Conectando',
                error: null,
            }

    const loading =
        enabled && consulta.version !== consultaVersion

    return {
        estadoConexion: enabled
            ? conexionActual.estado
            : 'Deshabilitado',

        errorConexion: enabled
            ? conexionActual.error
            : null,

        alertas: enabled ? alertas : [],

        repuestos:
            enabled && !loading ? consulta.repuestos : [],

        errorStock:
            enabled && !loading ? consulta.error : null,

        fechaConsulta:
            enabled && !loading ? consulta.fecha : null,

        loading,

        actualizarStock: () =>
            setConsultaVersion((version) => version + 1),

        reintentarConexion: () =>
            setIntento((version) => version + 1),
    }
}