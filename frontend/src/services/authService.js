import { httpClient } from '@/api/httpClient'
import { tokenStorage } from '@/utils/tokenStorage'
import { createApiError } from '@/utils/apiError'

export const authService = {
    async login(login, password) {
        const result = await httpClient.post('Auth/login', {
            login,
            password,
        })

        const auth = result?.data

        if (!auth?.token) {
            throw createApiError(0, {
                detail: 'La API no devolvió un token de acceso.',
            })
        }

        tokenStorage.set(auth.token)
        return auth
    },

    async getCurrentUser(options) {
        const result = await httpClient.get('Auth/me', options)
        return result.data
    },

    logout() {
        tokenStorage.clear()
    },
}