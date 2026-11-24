import { useNavigate } from 'react-router'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { toast } from 'sonner'

import { Button } from '@/components/ui/button'
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

import { loginSchema } from '@/schemas/authSchemas'
import { authService } from '@/services/authService'
import {
    applyServerErrors,
    getErrorMessage,
} from '@/utils/apiError'

export default function LoginPage() {
    const navigate = useNavigate()

    const {
        register,
        handleSubmit,
        setError,
        formState: { errors, isSubmitting },
    } = useForm({
        resolver: zodResolver(loginSchema),
        defaultValues: {
            login: '',
            password: '',
        },
    })

    async function onSubmit(values) {
        try {
            await authService.login(values.login, values.password)
            toast.success('Sesión iniciada correctamente.')
            navigate('/', { replace: true })
        } catch (error) {
            const assigned = applyServerErrors(
                error,
                setError,
                ['login', 'password'],
            )

            if (!assigned) {
                setError('root', {
                    message: getErrorMessage(error),
                })
            }
        }
    }

    return (
        <main className="flex min-h-svh items-center justify-center bg-muted/40 p-4">
            <Card className="w-full max-w-sm">
                <CardHeader>
                    <CardTitle>Iniciar sesión</CardTitle>
                </CardHeader>

                <CardContent>
                    <form
                        onSubmit={handleSubmit(onSubmit)}
                        className="space-y-4"
                        noValidate
                    >
                        <div className="space-y-2">
                            <Label htmlFor="login">Usuario</Label>
                            <Input
                                id="login"
                                autoComplete="username"
                                aria-invalid={Boolean(errors.login)}
                                aria-describedby={errors.login ? 'login-error' : undefined}
                                {...register('login')}
                            />
                            {errors.login && (
                                <p id="login-error" className="text-sm text-destructive">
                                    {errors.login.message}
                                </p>
                            )}
                        </div>

                        <div className="space-y-2">
                            <Label htmlFor="password">Contraseña</Label>
                            <Input
                                id="password"
                                type="password"
                                autoComplete="current-password"
                                aria-invalid={Boolean(errors.password)}
                                aria-describedby={
                                    errors.password ? 'password-error' : undefined
                                }
                                {...register('password')}
                            />
                            {errors.password && (
                                <p id="password-error" className="text-sm text-destructive">
                                    {errors.password.message}
                                </p>
                            )}
                        </div>

                        {errors.root && (
                            <p role="alert" className="text-sm text-destructive">
                                {errors.root.message}
                            </p>
                        )}

                        <Button
                            type="submit"
                            className="w-full"
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? 'Ingresando...' : 'Ingresar'}
                        </Button>
                    </form>
                </CardContent>
            </Card>
        </main>
    )
}