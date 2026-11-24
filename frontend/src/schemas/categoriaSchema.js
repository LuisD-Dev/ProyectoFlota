import { z } from 'zod'

export const categoriaSchema = z.object({
    nombre: z
        .string()
        .max(100, 'El nombre no puede superar 100 caracteres.')
        .trim()
        .min(1, 'El nombre es obligatorio.')
        .regex(/^[^<>]*$/, 'El nombre no puede contener < ni >.'),

    descripcion: z
        .string()
        .max(500, 'La descripción no puede superar 500 caracteres.')
        .regex(/^[^<>]*$/, 'La descripción no puede contener < ni >.'),

    intervaloMantenimientoKm: z
        .string()
        .trim()
        .min(1, 'El intervalo de mantenimiento es obligatorio.')
        .pipe(
            z.coerce
                .number({ error: 'Ingrese un número válido.' })
                .int('El intervalo debe ser un número entero.')
                .min(0, 'El intervalo no puede ser negativo.')
                .max(2147483647, 'El intervalo es demasiado grande.'),
        ),
})

export function getCategoriaDefaultValues(categoria) {
    return {
        nombre: categoria?.nombre ?? '',
        descripcion: categoria?.descripcion ?? '',
        intervaloMantenimientoKm: String(
            categoria?.intervaloMantenimientoKm ?? 0,
        ),
    }
}

export function toCategoriaRequest(values) {
    return {
        nombre: values.nombre,
        descripcion: values.descripcion.trim() || null,
        intervaloMantenimientoKm: values.intervaloMantenimientoKm,
    }
}