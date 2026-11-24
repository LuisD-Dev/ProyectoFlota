import { Badge } from '@/components/ui/badge'

const estados = {
  0: { label: 'Operativo', color: 'border-emerald-200 bg-emerald-50 text-emerald-800' },
  1: { label: 'En mantenimiento', color: 'border-amber-200 bg-amber-50 text-amber-900' },
  2: { label: 'Fuera de servicio', color: 'border-orange-200 bg-orange-50 text-orange-900' },
  3: { label: 'Dado de baja', color: 'border-slate-300 bg-slate-100 text-slate-700' },
}

export default function EstadoActivoBadge({ estado }) {
  const item = estados[estado] ?? { label: 'Desconocido', color: '' }
  return <Badge variant="outline" className={item.color}>{item.label}</Badge>
}
