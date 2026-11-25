using MantenimientoFlota.Domain.DTO.Usuario;
using MantenimientoFlota.Domain.DTO.PatternResult;
namespace MantenimientoFlota.Application.Interfaces;

public interface IUsuarioService
{
    Task<Result<IReadOnlyList<UsuarioDto>>> GetAllAsync();
    Task<Result<UsuarioDto>> GetByIdAsync(int id);
    Task<Result<IReadOnlyList<RolDto>>> GetRolesAsync();
    Task<Result<UsuarioDto>> CreateAsync(CrearUsuarioDto dto);
    Task<Result<object?>> UpdateAsync(int id, ActualizarUsuarioDto dto);
    Task<Result<object?>> DesactivarAsync(int id);
}
