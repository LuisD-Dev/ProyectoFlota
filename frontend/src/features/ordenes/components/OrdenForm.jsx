import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'

import {
    ordenSchema,
    getOrdenDefaultValues,
} from '@/schemas/ordenSchema'

import {
    applyServerErrors,
    getErrorMessage,
} from '@/utils/apiError'

export default function OrdenForm({ activos, onSubmit, onCancel }) {
    const {
        register,
        handleSubmit,
        setError,
        formState: { errors, isSubmitting },
    } = useForm({
        resolver: zodResolver(ordenSchema),
        defaultValues: getOrdenDefaultValues(),
    })

    async function enviar(values) {
        try {
            await onSubmit(values)
        } catch (error) {
            const assigned = applyServerErrors(error, setError, [
                'activoId',
                'motivo',
                'tipoMantenimiento',
                'prioridad',
            ])

            if (!assigned) {
                setError('root', {
                    message: getErrorMessage(error),
                })
            }
        }
    }

    function mensajeError(nombre) {
        return errors[nombre] ? (
            <p
                id={`${nombre}-error`}
                className="text-sm text-destructive"
            >
                {errors[nombre].message}
            </p>
        ) : null
    }

    return (
        <form
            onSubmit={handleSubmit(enviar)}
            className="space-y-4"
            noValidate
        >
            <fieldset disabled={isSubmitting} className="space-y-4">
                <div className="space-y-2">
                    <Label htmlFor="activoId">Activo</Label>

                    <select
                        id="activoId"
                        className="h-9 w-full rounded-md border border-input bg-background px-3 text-sm"
                        aria-invalid={Boolean(errors.activoId)}
                        aria-describedby={
                            errors.activoId ? 'activoId-error' : undefined
                        }
                        {...register('activoId')}
                    >
                        <option value="">Seleccione un activo</option>

                        {activos.map((activo) => (
                            <option key={activo.id} value={String(activo.id)}>
                                {activo.codigo} — {activo.nombre}
                            </option>
                        ))}
                    </select>

                    {mensajeError('activoId')}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="motivo">Motivo</Label>

                    <Textarea
                        id="motivo"
                        maxLength={1000}
                        rows={4}
                        aria-invalid={Boolean(errors.motivo)}
                        aria-describedby={
                            errors.motivo ? 'motivo-error' : undefined
                        }
                        {...register('motivo')}
                    />

                    {mensajeError('motivo')}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="tipoMantenimiento">
                        Tipo de mantenimiento
                    </Label>

                    <Input
                        id="tipoMantenimiento"
                        placeholder="Ejemplo: Preventivo"
                        maxLength={100}
                        aria-invalid={Boolean(errors.tipoMantenimiento)}
                        aria-describedby={
                            errors.tipoMantenimiento
                                ? 'tipoMantenimiento-error'
                                : undefined
                        }
                        {...register('tipoMantenimiento')}
                    />

                    {mensajeError('tipoMantenimiento')}
                </div>

                <div className="space-y-2">
                    <Label htmlFor="prioridad">Prioridad</Label>

                    <Input
                        id="prioridad"
                        placeholder="Ejemplo: Alta"
                        maxLength={50}
                        aria-invalid={Boolean(errors.prioridad)}
                        aria-describedby={
                            errors.prioridad ? 'prioridad-error' : undefined
                        }
                        {...register('prioridad')}
                    />

                    {mensajeError('prioridad')}
                </div>

                {errors.root && (
                    <p role="alert" className="text-sm text-destructive">
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
                        {isSubmitting
                            ? 'Enviando...'
                            : 'Registrar solicitud'}
                    </Button>
                </div>
            </fieldset>
        </form>
    )
}