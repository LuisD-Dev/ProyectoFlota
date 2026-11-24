import { useEffect, useState } from 'react'
import { activoService } from '@/services/activoService'

export function useActivos(filtros) {
    const [reloadCount, setReloadCount] = useState(0)
    const [resultado, setResultado] = useState({
        key: null,
        data: null,
        error: null,
    })

    const filtrosKey = JSON.stringify(filtros)
    const requestKey = `${filtrosKey}#${reloadCount}`

    useEffect(() => {
        const controller = new AbortController()

        activoService
            .getAll(JSON.parse(filtrosKey), {
                signal: controller.signal,
            })
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
    }, [filtrosKey, requestKey])

    const loading = resultado.key !== requestKey

    return {
        data: loading ? null : resultado.data,
        error: loading ? null : resultado.error,
        loading,
        reload: () => setReloadCount((value) => value + 1),
    }
}