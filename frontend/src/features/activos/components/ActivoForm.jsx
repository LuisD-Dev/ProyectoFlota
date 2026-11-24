import { Controller, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select'

import {
    activoSchema,
    getActivoDefaultValues,
} from '@/schemas/activoSchema'
import {
    applyServerErrors,
    getErrorMessage,
} from '@/utils/apiError'

function CampoInput({
    name,
    label,
    register,
    errors,
    type = 'text',
    ...props
}) {
    return (
        <div className="space-y-2">
            <Label htmlFor={name}>{label}</Label>

            <Input
                id={name}
                type={type}
                aria-invalid={Boolean(errors[name])}
                aria-describedby={errors[name] ? `${name}-error` : undefined}
                {...register(name)}
                {...props}
            />

            {errors[name] && (
                <p id={`${name}-error`} className="text-sm text-destructive">
                    {errors[name].message}
                </p>
            )}
        </div>
    )
}

export default function ActivoForm({
    activo,
    categorias,
    onSubmit,
    onCancel,
}) {
    const {
        register,
        control,
        handleSubmit,
        setError,
        formState: { errors, isSubmitting },
    } = useForm({
        resolver: zodResolver(activoSchema),
        defaultValues: getActivoDefaultValues(activo),
    })

    async function enviar(values) {
        try {
            await onSubmit(values)
        } catch (error) {
            const assigned = applyServerErrors(error, setError, [
                'codigo',
                'nombre',
                'descripcion',
                'fechaAdquisicion',
                'valorReferencia',
                'estado',
                'placa',
                'kilometrajeActual',
                'categoriaActivoId',
            ])

            if (!assigned) {
                setError('root', { message: getErrorMessage(error) })
            }
        }
    }

    return (
        <form
            onSubmit={handleSubmit(enviar)}
            className="space-y-5"
            noValidate
        >
            <fieldset disabled={isSubmitting} className="space-y-5">
                <div className="grid gap-4 sm:grid-cols-2">
                    <CampoInput
                        name="codigo"
                        label="Código"
                        register={register}
                        errors={errors}
                        placeholder="VEH-FLOTA-001"
                        maxLength={50}
                    />

                    <CampoInput
                        name="nombre"
                        label="Nombre"
                        register={register}
                        errors={errors}
                        maxLength={150}
                    />

                    <CampoInput
                        name="fechaAdquisicion"
                        label="Fecha de adquisición"
                        type="date"
                        register={register}
                        errors={errors}
                    />

                    <CampoInput
                        name="valorReferencia"
                        label="Valor de referencia"
                        type="number"
                        register={register}
                        errors={errors}
                        min="0"
                        step="0.01"
                    />

                    <CampoInput
                        name="placa"
                        label="Placa"
                        register={register}
                        errors={errors}
                        maxLength={20}
                    />

                    <CampoInput
                        name="kilometrajeActual"
                        label="Kilometraje actual"
                        type="number"
                        register={register}
                        errors={errors}
                        min="0"
                        max="2147483647"
                        step="1"
                    />

                    <Controller
                        name="categoriaActivoId"
                        control={control}
                        render={({ field, fieldState }) => (
                            <div className="space-y-2">
                                <Label htmlFor="categoriaActivoId">Categoría</Label>

                                <Select
                                    name={field.name}
                                    value={field.value || undefined}
                                    onValueChange={field.onChange}
                                    disabled={isSubmitting}
                                >
                                    <SelectTrigger
                                        id="categoriaActivoId"
                                        className="w-full"
                                        ref={field.ref}
                                        onBlur={field.onBlur}
                                        aria-invalid={fieldState.invalid}
                                        aria-describedby={
                                            fieldState.error ? 'categoria-error' : undefined
                                        }
                                    >
                                        <SelectValue placeholder="Seleccione una categoría" />
                                    </SelectTrigger>

                                    <SelectContent>
                                        {categorias.map((categoria) => (
                                            <SelectItem
                                                key={categoria.id}
                                                value={String(categoria.id)}
                                            >
                                                {categoria.nombre}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>

                                {fieldState.error && (
                                    <p id="categoria-error" className="text-sm text-destructive">
                                        {fieldState.error.message}
                                    </p>
                                )}
                            </div>
                        )}
                    />

                    <Controller
                        name="estado"
                        control={control}
                        render={({ field, fieldState }) => (
                            <div className="space-y-2">
                                <Label htmlFor="estado">Estado</Label>

                                <Select
                                    name={field.name}
                                    value={field.value}
                                    onValueChange={field.onChange}
                                    disabled={isSubmitting}
                                >
                                    <SelectTrigger
                                        id="estado"
                                        className="w-full"
                                        ref={field.ref}
                                        onBlur={field.onBlur}
                                        aria-invalid={fieldState.invalid}
                                        aria-describedby={
                                            fieldState.error ? 'estado-error' : undefined
                                        }
                                    >
                                        <SelectValue />
                                    </SelectTrigger>

                                    <SelectContent>
                                        <SelectItem value="0">Operativo</SelectItem>
                                        <SelectItem value="1">En mantenimiento</SelectItem>
                                        <SelectItem value="2">Fuera de servicio</SelectItem>
                                        <SelectItem value="3">Dado de baja</SelectItem>
                                    </SelectContent>
                                </Select>

                                {fieldState.error && (
                                    <p id="estado-error" className="text-sm text-destructive">
                                        {fieldState.error.message}
                                    </p>
                                )}
                            </div>
                        )}
                    />
                </div>

                <div className="space-y-2">
                    <Label htmlFor="descripcion">Descripción</Label>

                    <Textarea
                        id="descripcion"
                        maxLength={500}
                        aria-invalid={Boolean(errors.descripcion)}
                        aria-describedby={
                            errors.descripcion ? 'descripcion-error' : undefined
                        }
                        {...register('descripcion')}
                    />

                    {errors.descripcion && (
                        <p id="descripcion-error" className="text-sm text-destructive">
                            {errors.descripcion.message}
                        </p>
                    )}
                </div>
            </fieldset>

            {errors.root && (
                <p role="alert" className="text-sm text-destructive">
                    {errors.root.message}
                </p>
            )}

            <div className="flex justify-end gap-2">
                <Button
                    type="button"
                    variant="outline"
                    disabled={isSubmitting}
                    onClick={onCancel}
                >
                    Cancelar
                </Button>

                <Button type="submit" disabled={isSubmitting}>
                    {isSubmitting
                        ? 'Guardando...'
                        : activo
                            ? 'Guardar cambios'
                            : 'Registrar activo'}
                </Button>
            </div>
        </form>
    )
}