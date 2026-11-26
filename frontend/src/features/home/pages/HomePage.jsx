import { useState } from 'react'
import { Link, useOutletContext } from 'react-router'
import { ArrowRight, ClipboardList, Layers, Truck } from 'lucide-react'
import { useOrdenes } from '@/hooks/useOrdenes'
import { parseApiUtcDate } from '@/utils/format'

const modulos = [
    { ruta: '/ordenes', titulo: 'Órdenes', detalle: 'Consultá solicitudes y seguí cada mantenimiento.', Icono: ClipboardList, color: 'bg-blue-50 text-blue-800' },
    { ruta: '/activos', titulo: 'Activos', detalle: 'Buscá vehículos y equipos de la flota.', Icono: Truck, color: 'bg-teal-50 text-teal-800' },
    { ruta: '/categorias', titulo: 'Categorías', detalle: 'Consultá la clasificación de los activos.', Icono: Layers, color: 'bg-amber-50 text-amber-800' },
]

const fechaCorta = new Intl.DateTimeFormat('es-CR', { day: 'numeric', month: 'short', year: 'numeric' })

function OrdenesRecientes() {
    const { ordenes, loading, error, reload } = useOrdenes()

    if (loading) return <p role="status" className="p-5">Cargando órdenes…</p>
    if (error) return (
        <div role="alert" className="flex flex-wrap items-center gap-3 p-5">
            <p>No se pudieron cargar las órdenes.</p>
            <button onClick={reload} className="font-medium text-primary underline">Reintentar</button>
        </div>
    )
    if (!ordenes.length) return <p className="p-5 text-muted-foreground">Todavía no hay órdenes registradas.</p>

    const recientes = [...ordenes].sort((a, b) =>
        (parseApiUtcDate(b.fechaSolicitud).getTime() || 0) - (parseApiUtcDate(a.fechaSolicitud).getTime() || 0)
    ).slice(0, 4)

    return (
        <ul className="divide-y">
            {recientes.map((orden) => (
                <li key={orden.id}>
                    <Link to={`/ordenes/${orden.id}`} className="flex flex-wrap items-center gap-x-4 gap-y-1 px-5 py-4 hover:bg-muted focus-visible:outline-primary">
                        <span className="font-mono text-sm font-semibold text-primary">{orden.codigo}</span>
                        <span className="min-w-32 flex-1">{orden.activoNombre || orden.activoCodigo}</span>
                        <span className="text-sm text-muted-foreground">
                            {Number.isFinite(parseApiUtcDate(orden.fechaSolicitud).getTime()) ? fechaCorta.format(parseApiUtcDate(orden.fechaSolicitud)) : 'Sin fecha'}
                        </span>
                        <ArrowRight aria-hidden="true" size={16} />
                    </Link>
                </li>
            ))}
        </ul>
    )
}

export default function HomePage() {
    const [fechaActual] = useState(() => new Date())
    const { user } = useOutletContext()
    const verCategorias = ['Administrador', 'Coordinador/Técnico'].includes(user.rol)
    const visibles = modulos.filter(({ ruta }) => ruta !== '/categorias' || verCategorias)

    return (
        <div className="space-y-6">
            <header className="relative isolate overflow-hidden rounded-3xl bg-linear-to-br from-[#102f45] via-[#164f70] to-[#087e83] px-6 py-9 text-white sm:px-10 sm:py-12">
                <div aria-hidden="true" className="pointer-events-none absolute -right-12 -bottom-28 -z-10 size-80 rounded-full border-[32px] border-white/5 ring-[32px] ring-white/5" />
                <p className="mb-3 text-sm font-medium text-teal-100">{fechaCorta.format(fechaActual)}</p>
                <h1 className="text-3xl font-semibold tracking-tight sm:text-5xl">Panel de flota</h1>
                <p className="mt-3 max-w-xl text-base text-blue-50">Seguimiento de activos y órdenes de mantenimiento.</p>
            </header>
            <nav aria-label="Accesos a módulos" className={`grid gap-4 ${verCategorias ? 'md:grid-cols-3' : 'md:grid-cols-2'}`}>
                {visibles.map(({ ruta, titulo, detalle, Icono, color }) => (
                    <Link key={ruta} to={ruta} className="group rounded-2xl border bg-card p-5 shadow-sm transition-colors hover:border-primary focus-visible:outline-primary">
                        <div className="mb-5 flex items-center justify-between">
                            <span className={`rounded-xl p-3 ${color}`}><Icono size={23} aria-hidden="true" /></span>
                            <ArrowRight size={18} aria-hidden="true" className="text-muted-foreground group-hover:text-primary" />
                        </div>
                        <span className="block text-xl font-semibold">{titulo}</span>
                        <span className="mt-1 block text-sm text-muted-foreground">{detalle}</span>
                    </Link>
                ))}
            </nav>
            <section aria-labelledby="ordenes-recientes" className="overflow-hidden rounded-2xl border bg-card shadow-sm">
                <div className="flex flex-wrap items-center justify-between gap-3 border-b p-5">
                    <h2 id="ordenes-recientes" className="text-xl font-semibold">Órdenes recientes</h2>
                    <Link to="/ordenes" className="text-sm font-semibold text-primary hover:underline">Ver todas las órdenes</Link>
                </div>
                <OrdenesRecientes />
            </section>
        </div>
    )
}
