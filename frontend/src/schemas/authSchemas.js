import { z } from 'zod'

export const loginSchema = z.object({
    login: z
        .string()
        .trim()
        .min(1, 'El usuario es obligatorio.')
        .max(50, 'El usuario no puede superar 50 caracteres.'),

    password: z
        .string()
        .min(1, 'La contraseña es obligatoria.')
        .max(100, 'La contraseña no puede superar 100 caracteres.'),
})