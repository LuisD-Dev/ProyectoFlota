import { httpClient } from '@/api/httpClient'

export const categoriaService = {
    async create(datos) {
        const result = await httpClient.post('CategoriaActivo', datos)
        return result.data
    },

    async update(id, datos) {
        return httpClient.put(`CategoriaActivo/${id}`, datos)
    },

    async remove(id) {
        return httpClient.delete(`CategoriaActivo/${id}`)
    },
}