import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select'

function FiltroSelect({
    id,
    label,
    value,
    options,
    onChange,
    disabled = false,
}) {
    return (
        <div className="space-y-2">
            <Label htmlFor={id}>{label}</Label>

            <Select
                value={value}
                onValueChange={onChange}
                disabled={disabled}
            >
                <SelectTrigger id={id} className="w-full">
                    <SelectValue />
                </SelectTrigger>

                <SelectContent>
                    {options.map((option) => (
                        <SelectItem key={option.value} value={option.value}>
                            {option.label}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>
        </div>
    )
}

export default function ActivosFiltros({
    filtros,
    categorias = [],
    mostrarCategorias,
    loadingCategorias,
    onChange,
    onClear,
}) {
    return (
        <div className="space-y-4 rounded-md border p-4">
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
                <div className="space-y-2">
                    <Label htmlFor="texto">Buscar</Label>

                    <Input
                        id="texto"
                        placeholder="Código, nombre o placa"
                        maxLength={150}
                        value={filtros.texto}
                        onChange={(event) =>
                            onChange({ texto: event.target.value })
                        }
                    />
                </div>

                {mostrarCategorias && (
                    <FiltroSelect
                        id="categoriaActivoId"
                        label={
                            loadingCategorias ? 'Cargando categorías...' : 'Categoría'
                        }
                        value={filtros.categoriaActivoId || 'todas'}
                        disabled={loadingCategorias}
                        onChange={(value) =>
                            onChange({
                                categoriaActivoId: value === 'todas' ? '' : value,
                            })
                        }
                        options={[
                            { value: 'todas', label: 'Todas' },
                            ...categorias.map((categoria) => ({
                                value: String(categoria.id),
                                label: categoria.nombre,
                            })),
                        ]}
                    />
                )}

                <FiltroSelect
                    id="estado"
                    label="Estado"
                    value={filtros.estado || 'todos'}
                    onChange={(value) =>
                        onChange({
                            estado: value === 'todos' ? '' : value,
                        })
                    }
                    options={[
                        { value: 'todos', label: 'Todos' },
                        { value: '0', label: 'Operativo' },
                        { value: '1', label: 'En mantenimiento' },
                        { value: '2', label: 'Fuera de servicio' },
                        { value: '3', label: 'Dado de baja' },
                    ]}
                />

                <FiltroSelect
                    id="sortBy"
                    label="Ordenar por"
                    value={filtros.sortBy}
                    onChange={(value) => onChange({ sortBy: value })}
                    options={[
                        { value: 'nombre', label: 'Nombre' },
                        { value: 'codigo', label: 'Código' },
                        { value: 'fecha', label: 'Fecha de adquisición' },
                        { value: 'kilometraje', label: 'Kilometraje' },
                    ]}
                />

                <FiltroSelect
                    id="sortDirection"
                    label="Dirección"
                    value={filtros.sortDirection}
                    onChange={(value) =>
                        onChange({ sortDirection: value })
                    }
                    options={[
                        { value: 'asc', label: 'Ascendente' },
                        { value: 'desc', label: 'Descendente' },
                    ]}
                />

                <div className="space-y-2">
                    <Label htmlFor="kilometrajeDesde">Kilometraje desde</Label>

                    <Input
                        id="kilometrajeDesde"
                        type="number"
                        min="0"
                        max="2147483647"
                        step="1"
                        value={filtros.kilometrajeDesde}
                        onChange={(event) =>
                            onChange({ kilometrajeDesde: event.target.value })
                        }
                    />
                </div>

                <div className="space-y-2">
                    <Label htmlFor="kilometrajeHasta">Kilometraje hasta</Label>

                    <Input
                        id="kilometrajeHasta"
                        type="number"
                        min="0"
                        max="2147483647"
                        step="1"
                        value={filtros.kilometrajeHasta}
                        onChange={(event) =>
                            onChange({ kilometrajeHasta: event.target.value })
                        }
                    />
                </div>
            </div>

            <Button variant="outline" onClick={onClear}>
                Limpiar filtros
            </Button>
        </div>
    )
}