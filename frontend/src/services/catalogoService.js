import { httpClient } from '@/api/httpClient'

export const catalogoService = {
    async getCategorias(options) {
        const result = await httpClient.get('CategoriaActivo', options)
        return result.data ?? []
    },
}