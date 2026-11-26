import {
    HubConnectionBuilder,
    LogLevel,
} from '@microsoft/signalr'

import { httpClient } from '@/api/httpClient'
import { tokenStorage } from '@/utils/tokenStorage'

export const stockAlertService = {
    crearConexion() {
        const apiUrl = (
            import.meta.env.VITE_API_URL ?? ''
        ).replace(/\/+$/, '')

        if (!apiUrl) {
            throw new Error(
                'Falta configurar VITE_API_URL en .env.',
            )
        }

        // De https://localhost:7090/api
        // obtenemos https://localhost:7090/hubs/stock.
        const hubUrl =
            apiUrl.replace(/\/api$/i, '') + '/hubs/stock'

        return new HubConnectionBuilder()
            .withUrl(hubUrl, {
                accessTokenFactory: () =>
                    tokenStorage.get() ?? '',
            })
            .withAutomaticReconnect([
                0,
                2000,
                10000,
                30000,
            ])
            .configureLogging(LogLevel.Warning)
            .build()
    },

    async getStockBajo(options) {
        const result = await httpClient.get('Repuesto', {
            ...options,
            params: {
                stockBajo: true,
            },
        })

        return result.data ?? []
    },
}