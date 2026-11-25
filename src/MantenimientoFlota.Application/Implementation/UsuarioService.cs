using System.Text;
using MapsterMapper;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.DTO.Usuario;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Exceptions;
using MantenimientoFlota.Infrastructure.Interfaces;
namespace MantenimientoFlota.Application.Implementation;

public sealed class UsuarioService(IUsuarioRepository repository, IMapper mapper) : IUsuarioService
{
    public async Task<Result<IReadOnlyList<UsuarioDto>>> GetAllAsync() =>
        Result<IReadOnlyList<UsuarioDto>>.Success(mapper.Map<List<UsuarioDto>>(await repository.GetAllAsync()));

    public async Task<Result<UsuarioDto>> GetByIdAsync(int id) =>
        Result<UsuarioDto>.Success(mapper.Map<UsuarioDto>(await BuscarAsync(id)));

    public async Task<Result<IReadOnlyList<RolDto>>> GetRolesAsync() =>
        Result<IReadOnlyList<RolDto>>.Success(mapper.Map<List<RolDto>>(await repository.GetRolesAsync()));

    private async Task<Usuario> BuscarAsync(int id)
    {
        if (id <= 0) throw new InvalidRequestException("El identificador debe ser positivo.");
        return await repository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException("No se encontró el usuario.");
    }

    public async Task<Result<UsuarioDto>> CreateAsync(CrearUsuarioDto dto)
    {
        // BCrypt limita la entrada a 72 bytes, no a 72 caracteres Unicode.
        if (Encoding.UTF8.GetByteCount(dto.Password) > 72)
            throw new InvalidRequestException("La contraseña no puede superar 72 bytes UTF-8.");
        if (await repository.GetRoleNameAsync(dto.RolId) is null)
            throw new ResourceNotFoundException("No se encontró el rol.");
        var usuario = mapper.Map<Usuario>(dto);
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12);
        usuario.Activo = true;
        usuario.FechaCreacionUtc = DateTime.UtcNow;
        await repository.CreateAsync(usuario);
        return await GetCreatedAsync(usuario.Id);
    }

    private async Task<Result<UsuarioDto>> GetCreatedAsync(int id) =>
        Result<UsuarioDto>.Success(mapper.Map<UsuarioDto>(await BuscarAsync(id)), 201);

    public async Task<Result<object?>> UpdateAsync(int id, ActualizarUsuarioDto dto)
    {
        var usuario = await BuscarAsync(id);
        if (await repository.GetRoleNameAsync(dto.RolId) is null)
            throw new ResourceNotFoundException("No se encontró el rol.");
        mapper.Map(dto, usuario);
        await repository.UpdateAsync(usuario);
        return Result<object?>.Success(null, 200, "OK", "Usuario actualizado.");
    }

    public async Task<Result<object?>> DesactivarAsync(int id)
    {
        await BuscarAsync(id);
        await repository.DesactivarAsync(id);
        return Result<object?>.Success(null, 200, "OK", "Usuario desactivado.");
    }
}
