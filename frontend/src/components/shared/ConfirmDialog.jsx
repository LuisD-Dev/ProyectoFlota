import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from '@/components/ui/alert-dialog'

export default function ConfirmDialog({
    open,
    title,
    description,
    confirmText = 'Confirmar',
    loading = false,
    error,
    onConfirm,
    onCancel,
}) {
    return (
        <AlertDialog
            open={open}
            onOpenChange={(nextOpen) => {
                if (!nextOpen && !loading) onCancel()
            }}
        >
            <AlertDialogContent
                onEscapeKeyDown={(event) => {
                    if (loading) event.preventDefault()
                }}
            >
                <AlertDialogHeader>
                    <AlertDialogTitle>{title}</AlertDialogTitle>
                    <AlertDialogDescription>
                        {description}
                    </AlertDialogDescription>
                </AlertDialogHeader>

                {error && (
                    <p role="alert" className="text-sm text-destructive">
                        {error}
                    </p>
                )}

                <AlertDialogFooter>
                    <AlertDialogCancel
                        disabled={loading}
                        onClick={onCancel}
                    >
                        Cancelar
                    </AlertDialogCancel>

                    <AlertDialogAction
                        disabled={loading}
                        onClick={(event) => {
                            event.preventDefault()
                            onConfirm()
                        }}
                    >
                        {loading ? 'Procesando...' : confirmText}
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}