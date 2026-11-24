import { Link } from 'react-router'
import EstadoActivoBadge from '@/features/activos/components/EstadoActivoBadge'
import ActivoMiniatura from '@/features/activos/components/ActivoMiniatura'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table'


export default function ActivosTabla({
    activos,
    loading,
    onEditar,
}) {
    return (
        <div className="overflow-x-auto rounded-md border">
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>Imagen</TableHead>
                        <TableHead>Código</TableHead>
                        <TableHead>Nombre</TableHead>
                        <TableHead>Categoría</TableHead>
                        <TableHead>Placa</TableHead>
                        <TableHead className="text-right">Kilometraje</TableHead>
                        <TableHead>Estado</TableHead>

                        {onEditar && <TableHead>Acciones</TableHead>}
                    </TableRow>
                </TableHeader>

                <TableBody>
                    {loading ? (
                        Array.from({ length: 5 }, (_, index) => (
                            <TableRow key={index}>
                                {Array.from(
                                    { length: onEditar ? 8 : 7 },
                                    (_, column) => (
                                        <TableCell key={column}>
                                            <Skeleton className="h-5 w-full" />
                                        </TableCell>
                                    ),
                                )}
                            </TableRow>
                        ))
                    ) : (
                        activos.map((activo) => (
                            <TableRow key={activo.id}>
                                <TableCell>
                                    <ActivoMiniatura
                                        key={`${activo.id}-${activo.imagenUrl}`}
                                        activo={activo}
                                    />
                                </TableCell>
                                <TableCell className="font-medium">
                                    <Link
                                        to={`/activos/${activo.id}`}
                                        className="underline underline-offset-4"
                                    >
                                        {activo.codigo}
                                    </Link>
                                </TableCell>

                                <TableCell>{activo.nombre}</TableCell>

                                <TableCell>
                                    {activo.categoriaActivoNombre}
                                </TableCell>

                                <TableCell>
                                    {activo.placa || 'Sin placa'}
                                </TableCell>

                                <TableCell className="text-right">
                                    {activo.kilometrajeActual.toLocaleString('es-CR')} km
                                </TableCell>

                                <TableCell>
                                    <EstadoActivoBadge estado={activo.estado} />
                                </TableCell>

                                {onEditar && (
                                    <TableCell>
                                        <Button
                                            variant="outline"
                                            size="sm"
                                            onClick={() => onEditar(activo)}
                                        >
                                            Editar
                                        </Button>
                                    </TableCell>
                                )}
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </div>
    )
}