import { useState } from 'react'
import { toast } from 'sonner'

import { Button } from '@/components/ui/button'
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'
import ConfirmDialog from '@/components/shared/ConfirmDialog'

import { activoService } from '@/services/activoService'
import { getErrorMessage } from '@/utils/apiError'

const acciones = {
    fueraServicio: {
        title: 'Cambiar a Fuera de servicio',
        description:
            'El activo quedará fuera de servicio. La operación será rechazada si tiene órdenes abiertas.',
        confirmText: 'Cambiar estado',
    },

    estadoBaja: {
        title: 'Cambiar a Dado de baja',
        description:
            'Se cambiará el estado administrativo a Dado de baja. La operación será rechazada si tiene órdenes abiertas.',
        confirmText: 'Cambiar estado',
    },

    bajaLogica: {
        title: 'Dar de baja lógica al activo',
        description:
            'El registro se conservará con estado Dado de baja. Esta operación será rechazada si tiene cualquier orden asociada.',
        confirmText: 'Dar de baja',
    },
}

export default function ActivoAcciones({ activo, onUpdated }) {
    const [accion, setAccion] = useState(null)
    const [saving, setSaving] = useState(false)
    const [error, setError] = useState('')

    const seleccion = accion ? acciones[accion] : null

    function abrirConfirmacion(nextAction) {
        setError('')
        setAccion(nextAction)
    }

    function cancelar() {
        if (saving) return

        setAccion(null)
        setError('')
    }

    async function confirmar() {
        if (!accion || saving) return

        setSaving(true)
        setError('')

        try {
            if (accion === 'bajaLogica') {
                await activoService.remove(activo.id)
            } else {
                const estado = accion === 'fueraServicio' ? 2 : 3
                await activoService.changeState(activo.id, estado)
            }

            toast.success('Operación completada correctamente.')
            setAccion(null)
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
                <CardTitle>Acciones administrativas</CardTitle>
            </CardHeader>

            <CardContent className="space-y-4">
                <p className="text-sm text-muted-foreground">
                    Los cambios se validan en el servidor. El registro del activo
                    se conserva en la base de datos.
                </p>

                <div className="flex flex-wrap gap-2">
                    <Button
                        variant="outline"
                        disabled={saving || activo.estado === 2}
                        onClick={() => abrirConfirmacion('fueraServicio')}
                    >
                        Fuera de servicio
                    </Button>

                    <Button
                        variant="outline"
                        disabled={saving || activo.estado === 3}
                        onClick={() => abrirConfirmacion('estadoBaja')}
                    >
                        Cambiar a Dado de baja
                    </Button>

                    <Button
                        variant="destructive"
                        disabled={saving || activo.estado === 3}
                        onClick={() => abrirConfirmacion('bajaLogica')}
                    >
                        Baja lógica
                    </Button>
                </div>

                {seleccion && (
                    <ConfirmDialog
                        open
                        title={seleccion.title}
                        description={`${activo.codigo}: ${seleccion.description}`}
                        confirmText={seleccion.confirmText}
                        loading={saving}
                        error={error}
                        onConfirm={confirmar}
                        onCancel={cancelar}
                    />
                )}
            </CardContent>
        </Card>
    )
}