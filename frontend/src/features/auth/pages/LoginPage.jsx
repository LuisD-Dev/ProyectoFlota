import { useState } from 'react'
import { ArrowRight, Eye, EyeOff, Truck } from 'lucide-react'
import { useLocation, useNavigate } from 'react-router'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { toast } from 'sonner'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

import { loginSchema } from '@/schemas/authSchemas'
import { authService } from '@/services/authService'
import {
    applyServerErrors,
    getErrorMessage,
} from '@/utils/apiError'

export default function LoginPage() {
    const [visible, setVisible] = useState(false)
    const navigate = useNavigate()
    const location = useLocation()

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
        <main className="grid min-h-svh place-items-center bg-[#edf3f7] p-4 sm:p-8">
            <div className="grid w-full max-w-4xl overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-xl shadow-slate-900/10 md:grid-cols-[0.92fr_1fr]">
                <header className="relative isolate flex flex-col justify-between gap-5 overflow-hidden bg-linear-to-br from-[#102f45] via-[#164f70] to-[#087e83] p-6 text-white sm:p-9 md:min-h-[29rem]">
                    <div aria-hidden="true" className="pointer-events-none absolute -bottom-32 -right-36 -z-10 size-96 rounded-full border-[36px] border-white/5 ring-[36px] ring-white/5" />
                    <div className="flex size-12 items-center justify-center rounded-2xl border border-white/25 bg-white/10">
                        <Truck aria-hidden="true" size={27} />
                    </div>
                    <div>
                        <p className="max-w-xs text-3xl leading-tight font-semibold tracking-tight md:text-5xl">Mantenimiento de Flota</p>
                        <p className="mt-3 max-w-xs text-sm leading-relaxed text-blue-50 md:text-base">Consultá activos y seguí las órdenes de mantenimiento de tu equipo.</p>
                    </div>
                </header>
                <section aria-labelledby="titulo-login" className="flex flex-col justify-center p-6 sm:p-9 lg:p-12">
                    <h1 id="titulo-login" className="mb-7 text-2xl font-semibold tracking-tight text-foreground sm:text-3xl">Iniciar sesión</h1>
                    {location.state?.message && (
                        <p role="status" className="mb-4 text-sm text-muted-foreground">
                            {location.state.message}
                        </p>
                    )}
                    <form
                        onSubmit={handleSubmit(onSubmit)}
                        className="space-y-4"
                        noValidate
                    >
                        <div className="space-y-2">
                            <Label htmlFor="login">Usuario</Label>
                            <Input
                                id="login" className="h-11 rounded-xl"
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
                            <div className="relative">
                                <Input
                                    id="password"
                                    type={visible ? "text" : "password"} className="h-11 rounded-xl pr-12"
                                    autoComplete="current-password"
                                    aria-invalid={Boolean(errors.password)}
                                    aria-describedby={
                                        errors.password ? 'password-error' : undefined
                                    }
                                    {...register('password')}
                                />
                                <button
                                    type="button"
                                    aria-label={visible ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                                    onClick={() => setVisible(!visible)}
                                    className="absolute inset-y-0 right-0 grid w-11 place-items-center rounded-xl text-muted-foreground hover:text-primary focus-visible:outline-primary"
                                >
                                    {visible ? <EyeOff size={18} aria-hidden="true" /> : <Eye size={18} aria-hidden="true" />}
                                </button>
                            </div>
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
                            className="h-11 w-full rounded-xl font-semibold"
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? 'Ingresando…' : 'Ingresar'}
                            <ArrowRight size={17} aria-hidden="true" />
                        </Button>
                    </form>
                </section>
            </div>
        </main>
    )
}
