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

import ActivoForm from '@/features/activos/components/ActivoForm'
import { useActivo } from '@/hooks/useActivo'
import { useCategorias } from '@/hooks/useCategorias'
import { activoService } from '@/services/activoService'
import { toActivoRequest } from '@/schemas/activoSchema'
import { getErrorMessage } from '@/utils/apiError'

export default function ActivoFormDialog({
    activoId,
    onClose,
    onSaved,
}) {
    const [saving, setSaving] = useState(false)

    const {
        activo,
        loading: loadingActivo,
        error: errorActivo,
        reload: recargarActivo,
    } = useActivo(activoId)

    const {
        categorias,
        loading: loadingCategorias,
        error: errorCategorias,
        reload: recargarCategorias,
    } = useCategorias()

    const loading = loadingActivo || loadingCategorias
    const error = errorActivo || errorCategorias

    async function guardar(values) {
        setSaving(true)

        try {
            const request = toActivoRequest(values, activo)

            if (activoId === null) {
                await activoService.create(request)
                toast.success('Activo registrado correctamente.')
            } else {
                await activoService.update(activoId, request)
                toast.success('Activo actualizado correctamente.')
            }

            onSaved()
        } finally {
            setSaving(false)
        }
    }

    function reintentar() {
        recargarCategorias()

        if (activoId !== null) {
            recargarActivo()
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
                className="max-h-[90svh] overflow-y-auto sm:max-w-2xl"
                onEscapeKeyDown={(event) => {
                    if (saving) event.preventDefault()
                }}
                onInteractOutside={(event) => {
                    if (saving) event.preventDefault()
                }}
            >
                <DialogHeader>
                    <DialogTitle>
                        {activoId === null ? 'Nuevo activo' : 'Editar activo'}
                    </DialogTitle>

                    <DialogDescription>
                        Complete los datos del activo. La imagen se administra por separado.
                    </DialogDescription>
                </DialogHeader>

                {loading ? (
                    <p role="status">Cargando formulario...</p>
                ) : error ? (
                    <div className="space-y-3" role="alert">
                        <p>{getErrorMessage(error)}</p>
                        <Button variant="outline" onClick={reintentar}>
                            Reintentar
                        </Button>
                    </div>
                ) : categorias.length === 0 ? (
                    <p>Debe registrar al menos una categoría antes de crear activos.</p>
                ) : (
                    <ActivoForm
                        activo={activo}
                        categorias={categorias}
                        onSubmit={guardar}
                        onCancel={onClose}
                    />
                )}
            </DialogContent>
        </Dialog>
    )
}