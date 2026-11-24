import { useState } from 'react'
import { toast } from 'sonner'

import { Button } from '@/components/ui/button'
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog'

import OrdenForm from '@/features/ordenes/components/OrdenForm'
import { useActivosSolicitud } from '@/hooks/useActivosSolicitud'
import { ordenService } from '@/services/ordenService'
import { toOrdenRequest } from '@/schemas/ordenSchema'
import { getErrorMessage } from '@/utils/apiError'

export default function OrdenFormDialog({ onClose, onSaved }) {
    const [saving, setSaving] = useState(false)
    const { activos, loading, error, reload } = useActivosSolicitud()

    async function guardar(values) {
        setSaving(true)

        try {
            const request = toOrdenRequest(values)
            await ordenService.create(request)

            toast.success('Solicitud registrada correctamente.')
            onSaved()
        } finally {
            setSaving(false)
        }
    }

    return (
        <Dialog
            open
            onOpenChange={(open) => {
                if (!open && !saving) onClose()
            }}
        >
            <DialogContent
                className="max-h-[90svh] overflow-y-auto sm:max-w-xl"
                onEscapeKeyDown={(event) => {
                    if (saving) event.preventDefault()
                }}
                onInteractOutside={(event) => {
                    if (saving) event.preventDefault()
                }}
            >
                <DialogHeader>
                    <DialogTitle>
                        Nueva solicitud de mantenimiento
                    </DialogTitle>

                    <DialogDescription>
                        Seleccione el activo e indique el mantenimiento requerido.
                    </DialogDescription>
                </DialogHeader>

                {loading ? (
                    <p role="status">Cargando activos...</p>
                ) : error ? (
                    <div role="alert" className="space-y-3">
                        <p className="text-sm text-destructive">
                            {getErrorMessage(error)}
                        </p>

                        <Button variant="outline" onClick={reload}>
                            Reintentar
                        </Button>
                    </div>
                ) : activos.length === 0 ? (
                    <p>
                        No hay activos disponibles para seleccionar.
                    </p>
                ) : (
                    <OrdenForm
                        activos={activos}
                        onSubmit={guardar}
                        onCancel={onClose}
                    />
                )}
            </DialogContent>
        </Dialog>
    )
}