using MapsterMapper;
using MantenimientoFlota.Application.Interfaces;
using MantenimientoFlota.Domain.Constants;
using MantenimientoFlota.Domain.DTO.OrdenMantenimiento;
using MantenimientoFlota.Domain.DTO.PatternResult;
using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Exceptions;
using MantenimientoFlota.Infrastructure.Interfaces;
using E = MantenimientoFlota.Domain.Constants.EstadoOrdenMantenimiento;
namespace MantenimientoFlota.Application.Implementation;

public sealed class OrdenProcesoService(IOrdenProcesoRepository repository, IOrdenMantenimientoService consultas,
    IActivoService activos, ICurrentUser user, IMapper mapper) : IOrdenProcesoService
{
    private const decimal MaximoImporte = 9999999999999999.99m;
    private int UsuarioId => user.UsuarioId ?? throw new ForbiddenException("No se pudo identificar al usuario.");

    private static void ValidarImporte(decimal importe)
    {
        if (importe < 0 || importe > MaximoImporte)
            throw new InvalidRequestException("El importe supera el máximo permitido.");
    }

    private static void ValidarDosDecimales(decimal valor)
    {
        if (decimal.Round(valor, 2) != valor)
            throw new InvalidRequestException("La cantidad y las horas admiten como máximo dos decimales.");
    }

    private async Task<OrdenMantenimiento> BuscarAsync(int id)
    {
        if (id <= 0) throw new InvalidRequestException("El identificador debe ser positivo.");
        return await repository.GetForUpdateAsync(id)
            ?? throw new ResourceNotFoundException("No se encontró la orden.");
    }

    private void RegistrarCambio(OrdenMantenimiento orden, int nuevo, string? observacion)
    {
        if (orden.EstadoActual == nuevo) return;
        repository.Agregar(new HistorialEstado
        {
            OrdenMantenimientoId = orden.Id, EstadoAnterior = orden.EstadoActual,
            EstadoNuevo = nuevo, FechaUtc = DateTime.UtcNow, UsuarioId = UsuarioId, Observacion = observacion
        });
        orden.EstadoActual = nuevo;
    }

    private void CambiarActivo(Activo activo, int nuevo)
    {
        if (activo.Estado == nuevo) return;
        repository.Agregar(new HistorialEstado
        {
            ActivoId = activo.Id, EstadoAnterior = activo.Estado, EstadoNuevo = nuevo,
            FechaUtc = DateTime.UtcNow, UsuarioId = UsuarioId, Observacion = "Cambio automático por orden de mantenimiento."
        });
        activo.Estado = nuevo;
    }

    public Task<Result<object?>> CambiarEstadoAsync(int id, CambiarEstadoOrdenDto dto) => repository.EjecutarAsync(async () =>
    {
        var orden = await BuscarAsync(id);
        var nuevo = dto.Estado ?? throw new InvalidRequestException("El estado es obligatorio.");
        var permitido = orden.EstadoActual switch
        {
            E.Solicitada => nuevo is E.Diagnosticada or E.Cancelada,
            E.Diagnosticada => nuevo is E.Aprobada or E.Rechazada or E.Cancelada,
            E.Aprobada => nuevo is E.EnEjecucion or E.Cancelada,
            E.EnEjecucion => nuevo is E.Completada or E.Cancelada,
            E.EnEsperaDeRepuesto => nuevo == E.EnEjecucion,
            _ => false
        };
        if (!permitido) throw new ConflictException("La transición de estado no está permitida.");

        if (nuevo == E.Diagnosticada)
        {
            var especialidadId = dto.EspecialidadRequeridaId
                ?? throw new InvalidRequestException("El diagnóstico debe indicar la especialidad requerida.");
            if (await repository.GetEspecialidadAsync(especialidadId) is null)
                throw new ResourceNotFoundException("No se encontró la especialidad.");
            orden.EspecialidadRequeridaId = especialidadId;
        }
        if (nuevo == E.Aprobada)
        {
            if (dto.FechaProgramada is null)
                throw new InvalidRequestException("La aprobación debe indicar una fecha programada.");
            if (dto.FechaProgramada.Value.Kind != DateTimeKind.Utc)
                throw new InvalidRequestException("La fecha programada debe expresarse en UTC (sufijo Z).");
            if (dto.FechaProgramada < orden.FechaSolicitud)
                throw new InvalidRequestException("La fecha programada no puede ser anterior a la solicitud.");
            orden.FechaProgramada = dto.FechaProgramada;
        }
        if (nuevo == E.EnEjecucion)
        {
            if (orden.EspecialidadRequeridaId is null)
                throw new ConflictException("La orden requiere un diagnóstico con especialidad.");
            if (orden.Activo.Estado is EstadoActivo.FueraDeServicio or EstadoActivo.DadoDeBaja)
                throw new ConflictException("El activo está fuera de servicio o dado de baja.");
            if (await repository.TieneOtraOrdenEnEjecucionAsync(orden.ActivoId, orden.Id))
                throw new ConflictException("El activo ya tiene otra orden en ejecución o en espera.");
            if (orden.EstadoActual == E.EnEsperaDeRepuesto)
            {
                var pendientes = (await repository.GetRepuestosAsync(id)).Where(x => x.Pendiente).ToList();
                if (pendientes.Count == 0 || !HayStock(pendientes))
                    throw new ConflictException("No hay stock suficiente para reanudar todos los repuestos pendientes.");
                Consumir(pendientes);
            }
            CambiarActivo(orden.Activo, EstadoActivo.EnMantenimiento);
        }
        if (nuevo == E.Completada)
        {
            var repuestos = await repository.GetRepuestosAsync(id);
            if (repuestos.Any(x => x.Pendiente)) throw new ConflictException("Hay repuestos pendientes.");
            var tecnicos = await repository.GetTecnicosAsync(id);
            if (tecnicos.Count == 0) throw new ConflictException("Debe registrar técnicos y horas antes del cierre.");
            orden.CostoTotal = decimal.Round(repuestos.Sum(x => decimal.Round(x.Cantidad * x.CostoUnitario, 2, MidpointRounding.AwayFromZero)) +
                tecnicos.Sum(x => decimal.Round(x.HorasTrabajadas * x.TarifaPorHora, 2, MidpointRounding.AwayFromZero)), 2, MidpointRounding.AwayFromZero);
            ValidarImporte(orden.CostoTotal);
        }
        if (nuevo is E.Completada or E.Cancelada or E.Rechazada) orden.FechaCierre = DateTime.UtcNow;
        if (nuevo == E.Completada || (nuevo == E.Cancelada && orden.EstadoActual == E.EnEjecucion))
            CambiarActivo(orden.Activo, EstadoActivo.Operativo);
        RegistrarCambio(orden, nuevo, dto.Observacion?.Trim());
        return Result<object?>.Success(null, 200, "OK", "Estado actualizado e historial registrado.");
    });

    private static bool HayStock(List<RepuestoOrden> lineas) => lineas
        .GroupBy(x => x.RepuestoId).All(g => g.First().Repuesto.StockActual >= g.Sum(x => x.Cantidad));

    private static void Consumir(List<RepuestoOrden> lineas)
    {
        foreach (var linea in lineas)
        {
            linea.Repuesto.StockActual -= linea.Cantidad;
            linea.Pendiente = false;
        }
    }

    public Task<Result<object?>> RegistrarRepuestosAsync(int id, RegistrarRepuestosDto dto) => repository.EjecutarAsync(async () =>
    {
        var orden = await BuscarAsync(id);
        if (orden.EstadoActual != E.EnEjecucion) throw new ConflictException("Solo se registran repuestos durante la ejecución.");
        if (dto.Repuestos.Select(x => x.RepuestoId).Distinct().Count() != dto.Repuestos.Count)
            throw new InvalidRequestException("No repita un repuesto en la misma solicitud.");
        var lineas = new List<RepuestoOrden>();
        foreach (var item in dto.Repuestos.OrderBy(x => x.RepuestoId))
        {
            ValidarDosDecimales(item.Cantidad);
            var repuesto = await repository.GetRepuestoAsync(item.RepuestoId)
                ?? throw new ResourceNotFoundException("No se encontró uno de los repuestos.");
            var precio = await repository.GetPrecioAsync(item.RepuestoId)
                ?? throw new ConflictException("El repuesto no tiene un precio de proveedor registrado.");
            if (precio > 0 && item.Cantidad > MaximoImporte / precio) throw new InvalidRequestException("El importe supera el máximo permitido.");
            ValidarImporte(item.Cantidad * precio);
            lineas.Add(new RepuestoOrden
            {
                OrdenMantenimientoId = id, RepuestoId = repuesto.Id, Repuesto = repuesto,
                Cantidad = item.Cantidad, CostoUnitario = precio, Pendiente = true
            });
        }
        var disponible = HayStock(lineas);
        if (disponible) Consumir(lineas);
        else RegistrarCambio(orden, E.EnEsperaDeRepuesto, "Stock insuficiente: no se descontó ningún repuesto del lote.");
        foreach (var linea in lineas) repository.Agregar(linea);
        return Result<object?>.Success(null, 200, "OK", disponible
            ? "Repuestos registrados y stock descontado." : "Orden en espera de repuesto. Lote pendiente, sin descuentos de stock.");
    });

    public Task<Result<object?>> AsignarTecnicoAsync(int id, AsignarTecnicoDto dto) => repository.EjecutarAsync(async () =>
    {
        var orden = await BuscarAsync(id);
        if (orden.EstadoActual != E.EnEjecucion) throw new ConflictException("Solo se registran técnicos y horas durante la ejecución.");
        ValidarDosDecimales(dto.HorasTrabajadas);
        var tecnico = await repository.GetTecnicoAsync(dto.TecnicoId)
            ?? throw new ResourceNotFoundException("No se encontró el técnico.");
        if (!tecnico.Disponible) throw new ConflictException("El técnico no está disponible.");
        if (orden.EspecialidadRequeridaId is null ||
            !await repository.TieneEspecialidadAsync(tecnico.Id, orden.EspecialidadRequeridaId.Value))
            throw new ConflictException("El técnico no posee la especialidad requerida.");
        if ((await repository.GetTecnicosAsync(id)).Any(x => x.TecnicoId == tecnico.Id))
            throw new ConflictException("El técnico ya está registrado en la orden.");
        ValidarImporte(tecnico.TarifaPorHora * dto.HorasTrabajadas);
        repository.Agregar(new TecnicoOrden
        {
            OrdenMantenimientoId = id, TecnicoId = tecnico.Id, HorasTrabajadas = dto.HorasTrabajadas,
            TarifaPorHora = tecnico.TarifaPorHora, FechaAsignacionUtc = DateTime.UtcNow
        });
        return Result<object?>.Success(null, 200, "OK", "Técnico y horas registrados.");
    });

    public async Task<Result<IReadOnlyList<RepuestoOrdenDto>>> GetRepuestosAsync(int id)
    {
        await consultas.GetByIdAsync(id); // Aplica también el control de propietario de Belén.
        var detalles = mapper.Map<List<RepuestoOrdenDto>>(await repository.GetRepuestosAsync(id));
        foreach (var detalle in detalles) detalle.Subtotal = decimal.Round(detalle.Cantidad * detalle.CostoUnitario, 2, MidpointRounding.AwayFromZero);
        return Result<IReadOnlyList<RepuestoOrdenDto>>.Success(detalles);
    }
    public async Task<Result<IReadOnlyList<TecnicoOrdenDto>>> GetTecnicosAsync(int id)
    {
        await consultas.GetByIdAsync(id);
        var detalles = mapper.Map<List<TecnicoOrdenDto>>(await repository.GetTecnicosAsync(id));
        foreach (var detalle in detalles) detalle.Subtotal = decimal.Round(detalle.HorasTrabajadas * detalle.TarifaPorHora, 2, MidpointRounding.AwayFromZero);
        return Result<IReadOnlyList<TecnicoOrdenDto>>.Success(detalles);
    }
    public async Task<Result<IReadOnlyList<HistorialEstadoDto>>> GetHistorialAsync(int id)
    {
        await consultas.GetByIdAsync(id);
        return Result<IReadOnlyList<HistorialEstadoDto>>.Success(mapper.Map<List<HistorialEstadoDto>>(await repository.GetHistorialAsync(id, null, null)));
    }
    public async Task<Result<IReadOnlyList<HistorialEstadoDto>>> GetHistorialActivoAsync(int id)
    {
        await activos.GetByIdAsync(id);
        var solicitante = user.Rol == NombresRol.SolicitanteConsulta ? UsuarioId : (int?)null;
        return Result<IReadOnlyList<HistorialEstadoDto>>.Success(mapper.Map<List<HistorialEstadoDto>>(await repository.GetHistorialAsync(null, id, solicitante)));
    }
}
