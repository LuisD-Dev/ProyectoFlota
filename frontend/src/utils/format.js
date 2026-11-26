export function resolveImageUrl(path) {
    if (!path) return ''

    try {
        const apiUrl = new URL(import.meta.env.VITE_API_URL)
        const imageUrl = new URL(path, `${apiUrl.origin}/`)

        return ['http:', 'https:'].includes(imageUrl.protocol)
            ? imageUrl.href
            : ''
    } catch {
        return ''
    }
}
// SQL devuelve las fechas UTC de las órdenes sin información de zona.
export function parseApiUtcDate(value) {
    if (!value) return new Date(Number.NaN)
    const text = String(value)
    const hasTime = text.includes('T')
    const hasZone = /(?:Z|[+-]\d{2}:?\d{2})$/i.test(text)
    return new Date(hasTime && !hasZone ? `${text}Z` : text)
}
