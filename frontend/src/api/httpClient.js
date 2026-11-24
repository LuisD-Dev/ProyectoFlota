import { tokenStorage } from '@/utils/tokenStorage'
import {
    createApiError,
    createNetworkError,
} from '@/utils/apiError'

const API_URL = (import.meta.env.VITE_API_URL ?? '').replace(/\/+$/, '')

function buildQueryString(params) {
    const search = new URLSearchParams()

    for (const [key, value] of Object.entries(params ?? {})) {
        if (value === undefined || value === null || value === '') continue

        if (Array.isArray(value)) {
            value.forEach((item) => search.append(key, item))
        } else {
            search.append(key, value)
        }
    }

    const query = search.toString()
    return query ? `?${query}` : ''
}

async function request(method, path, options = {}) {
    if (!API_URL) {
        throw createApiError(0, {
            detail: 'Falta configurar VITE_API_URL en .env.',
        })
    }

    const { params, body, headers, signal } = options
    const url = `${API_URL}/${path.replace(/^\/+/, '')}${buildQueryString(params)}`
    const finalHeaders = { Accept: 'application/json', ...headers }
    const token = tokenStorage.get()

    if (token) finalHeaders.Authorization = `Bearer ${token}`

    let finalBody

    if (body instanceof FormData) {
        finalBody = body
    } else if (body !== undefined) {
        finalHeaders['Content-Type'] = 'application/json'
        finalBody = JSON.stringify(body)
    }

    let response

    try {
        response = await fetch(url, {
            method,
            headers: finalHeaders,
            body: finalBody,
            signal,
        })
    } catch (error) {
        if (error.name === 'AbortError') throw error
        throw createNetworkError()
    }

    const text = await response.text()
    const contentType = response.headers.get('Content-Type') ?? ''
    let data = null

    if (text) {
        if (contentType.includes('json')) {
            try {
                data = JSON.parse(text)
            } catch {
                throw createApiError(response.status, {
                    detail: 'El servidor devolvió una respuesta no válida.',
                })
            }
        } else {
            data = text
        }
    }

    if (!response.ok) {
        const isLogin = path.replace(/^\/+/, '') === 'Auth/login'

        if (
            response.status === 401 &&
            !isLogin &&
            token &&
            tokenStorage.get() === token
        ) {
            tokenStorage.clear()
            window.dispatchEvent(new Event('auth:expired'))
        }

        throw createApiError(response.status, data)
    }
    return data
}

export const httpClient = {
    get: (path, options) => request('GET', path, options),

    post: (path, body, options) =>
        request('POST', path, { ...options, body }),

    put: (path, body, options) =>
        request('PUT', path, { ...options, body }),

    patch: (path, body, options) =>
        request('PATCH', path, { ...options, body }),

    delete: (path, options) => request('DELETE', path, options),
}