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