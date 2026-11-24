import { Button } from '@/components/ui/button'

export default function Paginacion({
    page,
    totalPages,
    totalCount,
    onPageChange,
}) {
    return (
        <nav
            aria-label="Paginación del listado"
            className="flex flex-wrap items-center justify-between gap-3"
        >
            <p className="text-sm text-muted-foreground">
                {totalCount} registros · Página {page} de{' '}
                {Math.max(1, totalPages)}
            </p>

            <div className="flex gap-2">
                <Button
                    variant="outline"
                    disabled={page <= 1}
                    onClick={() => onPageChange(page - 1)}
                >
                    Anterior
                </Button>

                <Button
                    variant="outline"
                    disabled={totalPages === 0 || page >= totalPages}
                    onClick={() => onPageChange(page + 1)}
                >
                    Siguiente
                </Button>
            </div>
        </nav>
    )
}