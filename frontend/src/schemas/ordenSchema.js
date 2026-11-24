import { z } from 'zod'

function textoObligatorio(nombre, maximo) {
    return z
        .string()
        .max(
            maximo,
            `${nombre} no puede superar ${maximo} caracteres.`,
        )
        .trim()
        .min(1, `${nombre} es obligatorio.`)
        .regex(
            /^[^<>]*$/,
            `${nombre} no puede contener < ni >.`,
        )
}

export const ordenSchema = z.object({
    activoId: z
        .string()
        .trim()
        .min(1, 'Seleccione un activo.')
        .pipe(
            z.coerce
                .number({ error: 'Seleccione un activo válido.' })
                .int('El identificador debe ser entero.')
                .min(1, 'Seleccione un activo válido.')
                .max(2147483647, 'El identificador es demasiado grande.'),
        ),

    motivo: textoObligatorio('El motivo', 1000),

    tipoMantenimiento: textoObligatorio(
        'El tipo de mantenimiento',
        100,
    ),

    prioridad: textoObligatorio('La prioridad', 50),
})

export function getOrdenDefaultValues() {
    return {
        activoId: '',
        motivo: '',
        tipoMantenimiento: '',
        prioridad: '',
    }
}

export function toOrdenRequest(values) {
    return {
        activoId: values.activoId,
        motivo: values.motivo,
        tipoMantenimiento: values.tipoMantenimiento,
        prioridad: values.prioridad,
    }
}