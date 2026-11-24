import { useEffect, useState } from 'react'
import { ordenService } from '@/services/ordenService'

export function useOrdenes() {
    const [reloadCount, setReloadCount] = useState(0)
    const [resultado, setResultado] = useState({
        key: null,
        data: [],
        error: null,
    })

    useEffect(() => {
        const controller = new AbortController()

        ordenService
            .getAll(undefined, { signal: controller.signal })
            .then((data) => {
                if (!controller.signal.aborted) {
                    setResultado({
                        key: reloadCount,
                        data,
                        error: null,
                    })
                }
            })
            .catch((error) => {
                if (!controller.signal.aborted) {
                    setResultado({
                        key: reloadCount,
                        data: [],
                        error,
                    })
                }
            })

        return () => controller.abort()
    }, [reloadCount])

    const loading = resultado.key !== reloadCount

    return {
        ordenes: loading ? [] : resultado.data,
        error: loading ? null : resultado.error,
        loading,
        reload: () => setReloadCount((value) => value + 1),
    }
}