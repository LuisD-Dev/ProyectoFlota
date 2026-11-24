import { useEffect, useState } from 'react'
import { catalogoService } from '@/services/catalogoService'

export function useCategorias(enabled = true) {
    const [reloadCount, setReloadCount] = useState(0)
    const [resultado, setResultado] = useState({
        key: null,
        data: [],
        error: null,
    })

    useEffect(() => {
        if (!enabled) return

        const controller = new AbortController()

        catalogoService
            .getCategorias({ signal: controller.signal })
            .then((data) => {
                if (!controller.signal.aborted) {
                    setResultado({ key: reloadCount, data, error: null })
                }
            })
            .catch((error) => {
                if (!controller.signal.aborted) {
                    setResultado({ key: reloadCount, data: [], error })
                }
            })

        return () => controller.abort()
    }, [enabled, reloadCount])

    const loading = enabled && resultado.key !== reloadCount

    return {
        categorias: enabled && !loading ? resultado.data : [],
        error: enabled && !loading ? resultado.error : null,
        loading,
        reload: () => setReloadCount((value) => value + 1),
    }
}