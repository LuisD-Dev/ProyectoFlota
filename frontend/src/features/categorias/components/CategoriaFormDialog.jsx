import { useState } from 'react'
import { toast } from 'sonner'

import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog'

import CategoriaForm from '@/features/categorias/components/CategoriaForm'
import { categoriaService } from '@/services/categoriaService'
import { toCategoriaRequest } from '@/schemas/categoriaSchema'

export default function CategoriaFormDialog({
    categoria,
    onClose,
    onSaved,
}) {
    const [saving, setSaving] = useState(false)
    const editando = categoria != null

    async function guardar(values) {
        setSaving(true)

        try {
            const request = toCategoriaRequest(values)

            if (editando) {
                await categoriaService.update(categoria.id, request)
                toast.success('Categoría actualizada correctamente.')
            } else {
                await categoriaService.create(request)
                toast.success('Categoría registrada correctamente.')
            }

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
                className="max-h-[90svh] overflow-y-auto"
                onEscapeKeyDown={(event) => {
                    if (saving) event.preventDefault()
                }}
                onInteractOutside={(event) => {
                    if (saving) event.preventDefault()
                }}
            >
                <DialogHeader>
                    <DialogTitle>
                        {editando ? 'Editar categoría' : 'Nueva categoría'}
                    </DialogTitle>

                    <DialogDescription>
                        Complete los datos de la categoría de activo.
                    </DialogDescription>
                </DialogHeader>

                <CategoriaForm
                    categoria={categoria}
                    onSubmit={guardar}
                    onCancel={onClose}
                />
            </DialogContent>
        </Dialog>
    )
}