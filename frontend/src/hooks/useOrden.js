import { useEffect, useState } from 'react'
import { ordenService } from '@/services/ordenService'

export function useOrden(id) {
    const [reloadCount, setReloadCount] = useState(0)
    const [resultado, setResultado] = useState({
        key: null,
        data: null,
        error: null,
    })

    const requestKey = `${id}#${reloadCount}`

    useEffect(() => {
        if (id === null) return

        const controller = new AbortController()

        ordenService
            .getById(id, { signal: controller.signal })
            .then((data) => {
                if (!controller.signal.aborted) {
                    setResultado({
                        key: requestKey,
                        data,
                        error: null,
                    })
                }
            })
            .catch((error) => {
                if (!controller.signal.aborted) {
                    setResultado({
                        key: requestKey,
                        data: null,
                        error,
                    })
                }
            })

        return () => controller.abort()
    }, [id, requestKey])

    const loading = id !== null && resultado.key !== requestKey

    return {
        orden: id !== null && !loading ? resultado.data : null,
        error: id !== null && !loading ? resultado.error : null,
        loading,
        reload: () => setReloadCount((value) => value + 1),
    }
}