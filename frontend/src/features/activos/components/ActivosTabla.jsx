import { Link } from 'react-router'
import { Badge } from '@/components/ui/badge'
import { Skeleton } from '@/components/ui/skeleton'
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table'

const estados = {
    0: 'Operativo',
    1: 'En mantenimiento',
    2: 'Fuera de servicio',
    3: 'Dado de baja',
}

export default function ActivosTabla({ activos, loading }) {
    return (
        <div className="overflow-x-auto rounded-md border">
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>Código</TableHead>
                        <TableHead>Nombre</TableHead>
                        <TableHead>Categoría</TableHead>
                        <TableHead>Placa</TableHead>
                        <TableHead className="text-right">Kilometraje</TableHead>
                        <TableHead>Estado</TableHead>
                    </TableRow>
                </TableHeader>

                <TableBody>
                    {loading ? (
                        Array.from({ length: 5 }, (_, index) => (
                            <TableRow key={index}>
                                {Array.from({ length: 6 }, (_, column) => (
                                    <TableCell key={column}>
                                        <Skeleton className="h-5 w-full" />
                                    </TableCell>
                                ))}
                            </TableRow>
                        ))
                    ) : (
                        activos.map((activo) => (
                            <TableRow key={activo.id}>
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
                                    <Badge variant="outline">
                                        {estados[activo.estado] ?? 'Desconocido'}
                                    </Badge>
                                </TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </div>
    )
}