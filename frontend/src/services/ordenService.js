import { httpClient } from '@/api/httpClient'

export const ordenService = {
    async getAll(activoId, options) {
        const result = await httpClient.get('OrdenMantenimiento', {
            ...options,
            params: { activoId },
        })

        return result.data ?? []
    },

    async getById(id, options) {
        const result = await httpClient.get(
            `OrdenMantenimiento/${id}`,
            options,
        )

        return result.data
    },

    async create(datos) {
        const result = await httpClient.post(
            'OrdenMantenimiento',
            datos,
        )

        return result.data
    },
}