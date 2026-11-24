import { useState } from 'react'
import { z } from 'zod'
import { toast } from 'sonner'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'

import { activoService } from '@/services/activoService'
import { resolveImageUrl } from '@/utils/format'
import { getErrorMessage } from '@/utils/apiError'
import { imageCredits } from '@/utils/imageCredits'

const imagenSchema = z
    .instanceof(File, { error: 'Seleccione una imagen.' })
    .refine((file) => file.size > 0, 'El archivo está vacío.')
    .refine(
        (file) => file.size <= 2 * 1024 * 1024,
        'La imagen no puede superar 2 MiB.',
    )
    .refine(
        (file) => /\.(png|jpe?g)$/i.test(file.name),
        'Solo se permiten archivos PNG, JPG o JPEG.',
    )

export default function ActivoImagen({
    activo,
    puedeAdministrar,
    onUpdated,
}) {
    const [archivo, setArchivo] = useState(null)
    const [saving, setSaving] = useState(false)
    const [error, setError] = useState('')
    const [imagenFallida, setImagenFallida] = useState(false)

    const imageUrl = resolveImageUrl(activo.imagenUrl)
  const credit = imageCredits[activo.imagenUrl]

    async function subirImagen(event) {
        event.preventDefault()

        const validation = imagenSchema.safeParse(archivo)

        if (!validation.success) {
            setError(validation.error.issues[0].message)
            return
        }

        setSaving(true)
        setError('')

        try {
            await activoService.uploadImage(activo.id, validation.data)
            toast.success('Imagen actualizada correctamente.')
            onUpdated()
        } catch (err) {
            setError(getErrorMessage(err))
        } finally {
            setSaving(false)
        }
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>Imagen del activo</CardTitle>
            </CardHeader>

            <CardContent className="space-y-4">
                {imageUrl && !imagenFallida ? (
                    <img
                        src={imageUrl}
                        alt={activo.nombre}
                        className="max-h-72 w-full rounded-md border object-contain"
                        onError={() => setImagenFallida(true)}
                    />
                ) : (
                    <p className="text-muted-foreground">
                        {imageUrl
                            ? 'No se pudo cargar la imagen.'
                            : 'Este activo no tiene imagen.'}
                    </p>
                )}

                {credit && (
                  <p className="text-xs leading-relaxed text-muted-foreground">
                    {credit.note}{' '}
                    <a href={credit.source} target="_blank" rel="noreferrer" className="underline underline-offset-2">{credit.author}</a>
                    {' · '}
                    <a href={credit.licenseUrl} target="_blank" rel="noreferrer" className="underline underline-offset-2">{credit.license}</a>
                  </p>
                )}

                {puedeAdministrar && (
                    <form onSubmit={subirImagen} className="space-y-3">
                        <div className="space-y-2">
                            <Label htmlFor="archivo">
                                Subir o reemplazar imagen
                            </Label>

                            <Input
                                id="archivo"
                                type="file"
                                accept=".png,.jpg,.jpeg"
                                disabled={saving}
                                aria-invalid={Boolean(error)}
                                aria-describedby="archivo-ayuda"
                                onChange={(event) => {
                                    setArchivo(event.target.files?.[0] ?? null)
                                    setError('')
                                }}
                            />

                            <p
                                id="archivo-ayuda"
                                className="text-sm text-muted-foreground"
                            >
                                PNG o JPEG, máximo 2 MiB.
                            </p>
                        </div>

                        {error && (
                            <p role="alert" className="text-sm text-destructive">
                                {error}
                            </p>
                        )}

                        <Button type="submit" disabled={saving || !archivo}>
                            {saving ? 'Subiendo...' : 'Guardar imagen'}
                        </Button>
                    </form>
                )}
            </CardContent>
        </Card>
    )
}