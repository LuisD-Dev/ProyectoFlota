import { z } from 'zod'

function numeroObligatorio(message) {
    return z
        .string()
        .trim()
        .min(1, message)
        .pipe(
            z.coerce.number({
                error: 'Debe ingresar un número válido.',
            }),
        )
}

export const activoSchema = z.object({
    codigo: z
        .string()
        .max(50, 'El código no puede superar 50 caracteres.')
        .trim()
        .min(1, 'El código es obligatorio.')
        .regex(
            /^VEH-(?:[A-Z]+-)*[0-9]{3}$/,
            'Use un código como VEH-001 o VEH-FLOTA-001.',
        ),

    nombre: z
        .string()
        .max(150, 'El nombre no puede superar 150 caracteres.')
        .trim()
        .min(1, 'El nombre es obligatorio.')
        .regex(/^[^<>]*$/, 'El nombre no puede contener < ni >.'),

    descripcion: z
        .string()
        .max(500, 'La descripción no puede superar 500 caracteres.')
        .regex(/^[^<>]*$/, 'La descripción no puede contener < ni >.'),

    fechaAdquisicion: z.iso.date({
        error: 'Ingrese una fecha de adquisición válida.',
    }),

    valorReferencia: numeroObligatorio(
        'El valor de referencia es obligatorio.',
    ).pipe(
        z.number().min(0, 'El valor no puede ser negativo.'),
    ),

    estado: z
        .enum(['0', '1', '2', '3'], {
            error: 'Seleccione un estado válido.',
        })
        .transform(Number),

    placa: z
        .string()
        .max(20, 'La placa no puede superar 20 caracteres.'),

    kilometrajeActual: numeroObligatorio(
        'El kilometraje es obligatorio.',
    ).pipe(
        z
            .number()
            .int('El kilometraje debe ser entero.')
            .min(0, 'El kilometraje no puede ser negativo.')
            .max(2147483647, 'El kilometraje supera el máximo permitido.'),
    ),

    categoriaActivoId: numeroObligatorio(
        'Seleccione una categoría.',
    ).pipe(
        z
            .number()
            .int('La categoría no es válida.')
            .min(1, 'Seleccione una categoría.')
            .max(2147483647, 'La categoría no es válida.'),
    ),
})

export function getActivoDefaultValues(activo = null) {
    return {
        codigo: activo?.codigo ?? '',
        nombre: activo?.nombre ?? '',
        descripcion: activo?.descripcion ?? '',
        fechaAdquisicion: activo?.fechaAdquisicion ?? '',
        valorReferencia:
            activo ? String(activo.valorReferencia) : '',
        estado: String(activo?.estado ?? 0),
        placa: activo?.placa ?? '',
        kilometrajeActual:
            activo ? String(activo.kilometrajeActual) : '0',
        categoriaActivoId:
            activo ? String(activo.categoriaActivoId) : '',
    }
}

export function toActivoRequest(values, activo = null) {
    return {
        ...values,
        imagenUrl: activo?.imagenUrl || null,
    }
}