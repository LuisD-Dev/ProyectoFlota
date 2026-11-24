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
}