import { useState } from 'react'
import { ImageOff } from 'lucide-react'
import { resolveImageUrl } from '@/utils/format'

export default function ActivoMiniatura({ activo }) {
  const [failed, setFailed] = useState(false)
  const src = resolveImageUrl(activo.imagenUrl)

  if (!src || failed) {
    return <div className="flex h-14 w-20 items-center justify-center rounded-md border bg-muted text-muted-foreground" role="img" aria-label="Sin imagen disponible"><ImageOff className="size-5" aria-hidden="true" /></div>
  }

  return <img src={src} alt={`Imagen de ${activo.nombre}`} loading="lazy" className="h-14 w-20 rounded-md border bg-white object-cover" onError={() => setFailed(true)} />
}
