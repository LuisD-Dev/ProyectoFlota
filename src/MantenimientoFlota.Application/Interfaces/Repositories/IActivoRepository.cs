using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Enums;
using MantenimientoFlota.Application.DTOs.Activo;

namespace MantenimientoFlota.Application.Interfaces.Repositories;

public interface IActivoRepository
{
    Task<IEnumerable<Activo>> GetAllAsync(
        int? categoriaActivoId = null,
        EstadoActivo? estado = null);

    Task<Activo?> GetByIdAsync(int id);

    Task<ActivoDetalleDto?> GetDetalleByIdAsync(int id);

    Task<Activo?> AddAsync(Activo activo);

    void Update(Activo activo);

    void UpdateEstado(Activo activo);

    Task<bool> ExistsByCodigoAsync(string codigo, int? idExcluir = null);

    Task<bool> CategoriaExisteAsync(int categoriaActivoId);

    Task<bool> HasAssociatedMaintenanceOrdersAsync(int activoId);
}
