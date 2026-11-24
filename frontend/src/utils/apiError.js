export class ApiError extends Error {
    constructor(status, message, errors = {}, traceId = null) {
        super(message)
        this.name = 'ApiError'
        this.status = status
        this.errors = errors
        this.traceId = traceId
    }
}

export function createApiError(status, body) {
    const data =
        body !== null && typeof body === 'object' ? body : {}

    const message =
        status >= 500
            ? 'Ocurrió un error interno. Intentá nuevamente.'
            : data.detail || data.title || 'No se pudo completar la operación.'

    return new ApiError(
        status,
        message,
        data.errors ?? {},
        data.traceId ?? null,
    )
}

export function createNetworkError() {
    return new ApiError(
        0,
        'No se pudo conectar con el servidor. Verificá que la API esté ejecutándose.',
    )
}

export function getErrorMessage(error) {
    return error instanceof ApiError
        ? error.message
        : 'Ocurrió un error inesperado. Intentá nuevamente.'
}

export function applyServerErrors(error, setError, fieldNames) {
    if (!(error instanceof ApiError)) return false

    let assigned = false

    for (const [key, messages] of Object.entries(error.errors)) {
        const field = fieldNames.find(
            (name) => name.toLowerCase() === key.toLowerCase(),
        )

        if (!field) continue

        const message = Array.isArray(messages)
            ? messages.join(' ')
            : String(messages)

        setError(field, {
            type: 'server',
            message,
        })

        assigned = true
    }

    return assigned
}