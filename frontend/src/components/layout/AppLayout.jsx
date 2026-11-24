import {
    NavLink,
    Outlet,
    useNavigate,
    useOutletContext,
} from 'react-router'

import { Button } from '@/components/ui/button'
import { authService } from '@/services/authService'

export default function AppLayout() {
    const { user } = useOutletContext()
    const navigate = useNavigate()

    function cerrarSesion() {
        authService.logout()
        navigate('/login', { replace: true })
    }

    function estiloEnlace({ isActive }) {
        return `rounded-md px-3 py-2 text-sm ${isActive
                ? 'bg-muted font-medium'
                : 'text-muted-foreground'
            }`
    }

    return (
        <div className="min-h-svh bg-background text-foreground">
            <header className="border-b">
                <div className="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-4 p-4">
                    <span className="font-semibold">
                        Mantenimiento de Flota
                    </span>

                    <nav
                        aria-label="Navegación principal"
                        className="flex flex-wrap gap-1"
                    >
                        <NavLink to="/" end className={estiloEnlace}>
                            Inicio
                        </NavLink>

                        <NavLink to="/activos" className={estiloEnlace}>
                            Activos
                        </NavLink>
                    </nav>

                    <div className="flex flex-wrap items-center gap-3">
                        <div className="text-sm">
                            <p className="font-medium">{user.nombreCompleto}</p>
                            <p className="text-muted-foreground">{user.rol}</p>
                        </div>

                        <Button variant="outline" onClick={cerrarSesion}>
                            Cerrar sesión
                        </Button>
                    </div>
                </div>
            </header>

            <main className="mx-auto max-w-6xl p-4 md:p-6">
                <Outlet context={{ user }} />
            </main>
        </div>
    )
}