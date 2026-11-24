import { useEffect, useState } from 'react'
import { authService } from '@/services/authService'
import { tokenStorage } from '@/utils/tokenStorage'

export function useCurrentUser() {
    const [session, setSession] = useState({
        loading: true,
        user: null,
        error: null,
    })

    useEffect(() => {
        const controller = new AbortController()

        authService
            .getCurrentUser({ signal: controller.signal })
            .then((user) => {
                if (!controller.signal.aborted) {
                    setSession({ loading: false, user, error: null })
                }
            })
            .catch((error) => {
                if (controller.signal.aborted) return

                if (error.status === 401) {
                    tokenStorage.clear()
                }

                setSession({ loading: false, user: null, error })
            })

        return () => controller.abort()
    }, [])

    return session
}