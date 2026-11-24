import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'

import {
    categoriaSchema,
    getCategoriaDefaultValues,
} from '@/schemas/categoriaSchema'

import {
    applyServerErrors,
    getErrorMessage,
} from '@/utils/apiError'

export default function CategoriaForm({
    categoria,
    onSubmit,
    onCancel,
}) {
    const {
        register,
        handleSubmit,
        setError,
        formState: { errors, isSubmitting },
    } = useForm({
        resolver: zodResolver(categoriaSchema),
        defaultValues: getCategoriaDefaultValues(categoria),
    })

    async function enviar(values) {
        try {
            await onSubmit(values)
        } catch (error) {
            const assigned = applyServerErrors(error, setError, [
                'nombre',
                'descripcion',
                'intervaloMantenimientoKm',
            ])

            if (!assigned) {
                setError('root', {
                    message: getErrorMessage(error),
                })
            }
        }
    }

    return (
        <form
            onSubmit={handleSubmit(enviar)}
            className="space-y-4"
            noValidate
        >
            <fieldset disabled={isSubmitting} className="space-y-4">
                <div className="space-y-2">
                    <Label htmlFor="nombre">Nombre</Label>

                    <Input
                        id="nombre"
                        maxLength={100}
                        aria-invalid={Boolean(errors.nombre)}
                        aria-describedby={
                            errors.nombre ? 'nombre-error' : undefined
                        }
                        {...register('nombre')}
                    />

                    {errors.nombre && (
                        <p
                            id="nombre-error"
                            className="text-sm text-destructive"
                        >
                            {errors.nombre.message}
                        </p>
                    )}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="descripcion">
                        Descripción opcional
                    </Label>

                    <Textarea
                        id="descripcion"
                        maxLength={500}
                        rows={3}
                        aria-invalid={Boolean(errors.descripcion)}
                        aria-describedby={
                            errors.descripcion
                                ? 'descripcion-error'
                                : undefined
                        }
                        {...register('descripcion')}
                    />

                    {errors.descripcion && (
                        <p
                            id="descripcion-error"
                            className="text-sm text-destructive"
                        >
                            {errors.descripcion.message}
                        </p>
                    )}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="intervaloMantenimientoKm">
                        Intervalo de mantenimiento (km)
                    </Label>

                    <Input
                        id="intervaloMantenimientoKm"
                        type="number"
                        min={0}
                        max={2147483647}
                        step={1}
                        aria-invalid={Boolean(
                            errors.intervaloMantenimientoKm,
                        )}
                        aria-describedby={
                            errors.intervaloMantenimientoKm
                                ? 'intervalo-error'
                                : undefined
                        }
                        {...register('intervaloMantenimientoKm')}
                    />

                    {errors.intervaloMantenimientoKm && (
                        <p
                            id="intervalo-error"
                            className="text-sm text-destructive"
                        >
                            {errors.intervaloMantenimientoKm.message}
                        </p>
                    )}
                </div>

                {errors.root && (
                    <p
                        role="alert"
                        className="text-sm text-destructive"
                    >
                        {errors.root.message}
                    </p>
                )}

                <div className="flex justify-end gap-2">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={onCancel}
                    >
                        Cancelar
                    </Button>

                    <Button type="submit">
                        {isSubmitting ? 'Guardando...' : 'Guardar'}
                    </Button>
                </div>
            </fieldset>
        </form>
    )
}