import { Navigate, Outlet } from 'react-router'
import { Button } from '@/components/ui/button'
import { useCurrentUser } from '@/hooks/useCurrentUser'
import { tokenStorage } from '@/utils/tokenStorage'
import { getErrorMessage } from '@/utils/apiError'

export default function ProtectedRoute() {
    const { loading, user, error } = useCurrentUser()

    if (!tokenStorage.get() || error?.status === 401) {
        return <Navigate to="/login" replace />
    }

    if (loading) {
        return (
            <main className="p-6" role="status">
                Comprobando sesión...
            </main>
        )
    }

    if (error) {
        return (
            <main className="space-y-4 p-6">
                <p role="alert">{getErrorMessage(error)}</p>

                <Button onClick={() => window.location.reload()}>
                    Reintentar
                </Button>
            </main>
        )
    }

    if (!user) {
        return <Navigate to="/login" replace />
    }

    return <Outlet context={{ user }} />
}