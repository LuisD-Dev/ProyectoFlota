using MantenimientoFlota.Domain.DTO.Activo;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Infrastructure.Interfaces;

public interface IActivoRepository
{
    Task<(IEnumerable<Activo> Items, int TotalRecords)> SearchAsync(BuscarActivosDto consulta);

    Task<IEnumerable<Activo>> GetAllAsync(
        int? categoriaActivoId = null,
        int? estado = null);

    Task<Activo?> GetForUpdateAsync(int id);

    Task<Activo?> GetByIdAsync(int id);

    Task<(Activo? Entidad, int Cantidad)> GetDetalleByIdAsync(int id);

    Task<Activo?> AddAsync(Activo activo);

    Task UpdateAsync(Activo activo);

    Task UpdateEstadoAsync(Activo activo);

    Task ActualizarImagenUrlAsync(int id, string imagenUrl, CancellationToken cancellationToken);

    Task<bool> ExistsByCodigoAsync(string codigo, int? idExcluir = null);

    Task<bool> CategoriaExisteAsync(int categoriaActivoId);

    Task<bool> HasAssociatedMaintenanceOrdersAsync(int activoId);

    Task<bool> HasOpenMaintenanceOrdersAsync(int activoId);
}
