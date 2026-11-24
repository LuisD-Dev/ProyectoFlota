import { useEffect, useState } from 'react'
import { activoService } from '@/services/activoService'

export function useActivosSolicitud() {
    const [reloadCount, setReloadCount] = useState(0)
    const [resultado, setResultado] = useState({
        key: null,
        data: [],
        error: null,
    })

    useEffect(() => {
        const controller = new AbortController()

        activoService
            .getListado({ signal: controller.signal })
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
        activos: loading ? [] : resultado.data,
        loading,
        error: loading ? null : resultado.error,
        reload: () => setReloadCount((value) => value + 1),
    }
}