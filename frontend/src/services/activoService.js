import { httpClient } from '@/api/httpClient'

export const activoService = {
    async getAll(filtros, options) {
        const result = await httpClient.get('Activo/buscar', {
            params: {
                texto: filtros.texto,
                categoriaActivoId: filtros.categoriaActivoId,
                estado: filtros.estado,
                kilometrajeDesde: filtros.kilometrajeDesde,
                kilometrajeHasta: filtros.kilometrajeHasta,
                sortBy: filtros.sortBy,
                sortDirection: filtros.sortDirection,
                pageNumber: filtros.page,
                pageSize: filtros.pageSize,
            },
            ...options,
        })

        return {
            items: result.data ?? [],
            page: result.pageNumber ?? filtros.page,
            pageSize: result.pageSize ?? filtros.pageSize,
            totalCount: result.totalRecords ?? 0,
            totalPages: result.totalPages ?? 0,
        }
    },

    async getById(id, options) {
        const result = await httpClient.get(`Activo/${id}`, options)
        return result.data
    },
    async create(activo) {
        const result = await httpClient.post('Activo', activo)
        return result.data
    },

    async update(id, activo) {
        const result = await httpClient.put(`Activo/${id}`, activo)
        return result
    },
    async uploadImage(id, archivo) {
        const formData = new FormData()
        formData.append('Archivo', archivo)

        const result = await httpClient.put(
            `Activo/${id}/imagen`,
            formData,
        )

        return result.data
    },

    async changeState(id, estado) {
        return httpClient.patch(`Activo/${id}/estado`, {
            estado,
        })
    },

    async remove(id) {
        return httpClient.delete(`Activo/${id}`)
    },
}