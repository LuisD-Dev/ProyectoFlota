import { NavLink, Outlet } from 'react-router'

export default function AppLayout() {
    return (
        <div className="min-h-svh bg-background text-foreground">
            <header className="border-b">
                <div className="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-4 p-4">
                    <span className="font-semibold">
                        Mantenimiento de Flota
                    </span>

                    <nav aria-label="Navegación principal">
                        <NavLink
                            to="/"
                            end
                            className={({ isActive }) =>
                                `rounded-md px-3 py-2 text-sm ${isActive
                                    ? 'bg-muted font-medium'
                                    : 'text-muted-foreground'
                                }`
                            }
                        >
                            Inicio
                        </NavLink>
                    </nav>
                </div>
            </header>

            <main className="mx-auto max-w-6xl p-4 md:p-6">
                <Outlet />
            </main>
        </div>
    )
}