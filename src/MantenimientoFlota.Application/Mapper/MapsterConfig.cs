using MantenimientoFlota.Domain.DTO.OrdenMantenimiento;
using MantenimientoFlota.Domain.DTO.RepuestoProveedor;
using MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;
using MantenimientoFlota.Domain.DTO.Repuesto;
using MantenimientoFlota.Domain.DTO.Proveedor;
using MantenimientoFlota.Domain.DTO.Tecnico;
using MantenimientoFlota.Domain.DTO.Especialidad;
using MantenimientoFlota.Domain.DTO.CategoriaActivo;
using Mapster;
using MantenimientoFlota.Domain.DTO.Activo;
using MantenimientoFlota.Domain.Entities;

namespace MantenimientoFlota.Application.Mapper;

public static class MapsterConfig
{
    public static TypeAdapterConfig RegisterMaps()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<Activo, ActivoDto>()
            .Map(dest => dest.CategoriaActivoNombre,
                src => src.CategoriaActivo == null ? string.Empty : src.CategoriaActivo.Nombre);

        config.NewConfig<CategoriaActivo, CategoriaActivoResumenDto>();
        config.NewConfig<OrdenMantenimiento, OrdenMantenimientoResumenDto>();

        config.NewConfig<Activo, ActivoDetalleDto>()
            .Map(dest => dest.CategoriaActivoNombre,
                src => src.CategoriaActivo == null ? string.Empty : src.CategoriaActivo.Nombre)
            .Map(dest => dest.Categoria, src => src.CategoriaActivo)
            .Map(dest => dest.Ordenes, src => src.OrdenesMantenimiento
                .OrderByDescending(orden => orden.FechaSolicitud)
                .ThenByDescending(orden => orden.Id))
            .Ignore(dest => dest.CantidadOrdenes)
            // La fecha de referencia se captura una vez por solicitud en el servicio.
            .Ignore(dest => dest.AntiguedadAnios);

        config.NewConfig<CrearActivoDto, Activo>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CategoriaActivo)
            .Ignore(dest => dest.OrdenesMantenimiento)
            .Map(dest => dest.Codigo, src => src.Codigo.Trim())
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Descripcion, src => src.Descripcion ?? string.Empty)
            .Map(dest => dest.ImagenUrl, src => src.ImagenUrl ?? string.Empty)
            .Map(dest => dest.Placa, src => src.Placa ?? string.Empty);

        config.NewConfig<ActualizarActivoDto, Activo>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.ImagenUrl)
            .Ignore(dest => dest.CategoriaActivo)
            .Ignore(dest => dest.OrdenesMantenimiento)
            .Map(dest => dest.Codigo, src => src.Codigo.Trim())
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Descripcion, src => src.Descripcion ?? string.Empty)
            .Map(dest => dest.Placa, src => src.Placa ?? string.Empty);

        config.NewConfig<CategoriaActivo, CategoriaActivoDto>();

        config.NewConfig<Especialidad, EspecialidadDto>();

        config.NewConfig<Tecnico, TecnicoDto>();

        config.NewConfig<Proveedor, ProveedorDto>();

        config.NewConfig<Repuesto, RepuestoDto>();

        config.NewConfig<TecnicoEspecialidad, TecnicoEspecialidadDto>()
            .Map(dest => dest.TecnicoNombreCompleto, src => src.Tecnico == null ? string.Empty : src.Tecnico.NombreCompleto)
            .Map(dest => dest.EspecialidadNombre, src => src.Especialidad == null ? string.Empty : src.Especialidad.Nombre);

        config.NewConfig<RepuestoProveedor, RepuestoProveedorDto>()
            .Map(dest => dest.RepuestoNombre, src => src.Repuesto == null ? string.Empty : src.Repuesto.Nombre)
            .Map(dest => dest.ProveedorNombre, src => src.Proveedor == null ? string.Empty : src.Proveedor.Nombre);

        config.NewConfig<OrdenMantenimiento, OrdenMantenimientoDto>()
            .Map(dest => dest.ActivoCodigo, src => src.Activo == null ? string.Empty : src.Activo.Codigo)
            .Map(dest => dest.ActivoNombre, src => src.Activo == null ? string.Empty : src.Activo.Nombre);

        config.NewConfig<Activo, ActivoResumenDto>();

        config.NewConfig<CategoriaActivo, CategoriaActivoDetalleDto>()
            .Ignore(dest => dest.CantidadActivos)
            .Map(dest => dest.Activos, src => src.Activos.OrderBy(activo => activo.Codigo));

        config.NewConfig<TecnicoEspecialidad, TecnicoEspecialidadResumenDto>()
            .Map(dest => dest.NombreCompleto, src => src.Tecnico.NombreCompleto)
            .Map(dest => dest.Disponible, src => src.Tecnico.Disponible);

        config.NewConfig<Especialidad, EspecialidadDetalleDto>()
            .Map(dest => dest.Tecnicos, src => src.TecnicoEspecialidades.OrderBy(relacion => relacion.Tecnico.NombreCompleto).ThenBy(relacion => relacion.TecnicoId));

        config.NewConfig<TecnicoEspecialidad, EspecialidadTecnicoResumenDto>()
            .Map(dest => dest.Nombre, src => src.Especialidad.Nombre)
            .Ignore(dest => dest.AntiguedadAnios);

        config.NewConfig<Tecnico, TecnicoDetalleDto>()
            .Ignore(dest => dest.CantidadEspecialidades)
            .Map(dest => dest.Especialidades, src => src.TecnicoEspecialidades.OrderBy(relacion => relacion.Especialidad.Nombre).ThenBy(relacion => relacion.EspecialidadId));

        config.NewConfig<RepuestoProveedor, RepuestoProveedorResumenDto>()
            .Map(dest => dest.Codigo, src => src.Repuesto.Codigo)
            .Map(dest => dest.Nombre, src => src.Repuesto.Nombre);

        config.NewConfig<Proveedor, ProveedorDetalleDto>()
            .Map(dest => dest.Repuestos, src => src.RepuestoProveedores.OrderBy(relacion => relacion.Repuesto.Codigo).ThenBy(relacion => relacion.RepuestoId));

        config.NewConfig<RepuestoProveedor, ProveedorRepuestoResumenDto>()
            .Map(dest => dest.Nombre, src => src.Proveedor.Nombre);

        config.NewConfig<Repuesto, RepuestoDetalleDto>()
            .Map(dest => dest.StockBajo, src => src.StockActual < src.StockMinimo)
            .Map(dest => dest.FaltanteStockMinimo, src => src.StockActual < src.StockMinimo ? src.StockMinimo - src.StockActual : 0m)
            .Ignore(dest => dest.CantidadProveedores)
            .Map(dest => dest.Proveedores, src => src.RepuestoProveedores.OrderBy(relacion => relacion.Proveedor.Nombre).ThenBy(relacion => relacion.ProveedorId));

        config.NewConfig<CategoriaActivo, CategoriaOrdenResumenDto>();

        config.NewConfig<Activo, ActivoOrdenResumenDto>()
            .Map(dest => dest.Categoria, src => src.CategoriaActivo);

        config.NewConfig<OrdenMantenimiento, OrdenMantenimientoDetalleDto>()
            .Map(dest => dest.ActivoCodigo, src => src.Activo.Codigo)
            .Map(dest => dest.ActivoNombre, src => src.Activo.Nombre)
            .Ignore(dest => dest.DiasTranscurridosDesdeSolicitud);

        config.NewConfig<CrearCategoriaActivoDto, CategoriaActivo>()
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Descripcion, src => src.Descripcion ?? string.Empty)
            .Ignore(dest => dest.Activos)
            .Ignore(dest => dest.Id);

        config.NewConfig<ActualizarCategoriaActivoDto, CategoriaActivo>()
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Descripcion, src => src.Descripcion ?? string.Empty)
            .Ignore(dest => dest.Activos)
            .Ignore(dest => dest.Id);

        config.NewConfig<CrearEspecialidadDto, Especialidad>()
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Descripcion, src => src.Descripcion ?? string.Empty)
            .Ignore(dest => dest.TecnicoEspecialidades)
            .Ignore(dest => dest.Id);

        config.NewConfig<ActualizarEspecialidadDto, Especialidad>()
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Descripcion, src => src.Descripcion ?? string.Empty)
            .Ignore(dest => dest.TecnicoEspecialidades)
            .Ignore(dest => dest.Id);

        config.NewConfig<CrearTecnicoDto, Tecnico>()
            .Map(dest => dest.NombreCompleto, src => src.NombreCompleto.Trim())
            .Map(dest => dest.Identificacion, src => src.Identificacion.Trim())
            .Map(dest => dest.Telefono, src => src.Telefono == null ? string.Empty : src.Telefono.Trim())
            .Map(dest => dest.Correo, src => src.Correo == null ? string.Empty : src.Correo.Trim())
            .Ignore(dest => dest.TecnicoEspecialidades)
            .Ignore(dest => dest.Id);

        config.NewConfig<ActualizarTecnicoDto, Tecnico>()
            .Map(dest => dest.NombreCompleto, src => src.NombreCompleto.Trim())
            .Map(dest => dest.Identificacion, src => src.Identificacion.Trim())
            .Map(dest => dest.Telefono, src => src.Telefono == null ? string.Empty : src.Telefono.Trim())
            .Map(dest => dest.Correo, src => src.Correo == null ? string.Empty : src.Correo.Trim())
            .Ignore(dest => dest.TecnicoEspecialidades)
            .Ignore(dest => dest.Id);

        config.NewConfig<CrearProveedorDto, Proveedor>()
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Contacto, src => src.Contacto == null ? string.Empty : src.Contacto.Trim())
            .Map(dest => dest.Direccion, src => src.Direccion == null ? string.Empty : src.Direccion.Trim())
            .Map(dest => dest.CondicionesGenerales, src => src.CondicionesGenerales == null ? string.Empty : src.CondicionesGenerales.Trim())
            .Ignore(dest => dest.RepuestoProveedores)
            .Ignore(dest => dest.Id);

        config.NewConfig<ActualizarProveedorDto, Proveedor>()
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.Contacto, src => src.Contacto == null ? string.Empty : src.Contacto.Trim())
            .Map(dest => dest.Direccion, src => src.Direccion == null ? string.Empty : src.Direccion.Trim())
            .Map(dest => dest.CondicionesGenerales, src => src.CondicionesGenerales == null ? string.Empty : src.CondicionesGenerales.Trim())
            .Ignore(dest => dest.RepuestoProveedores)
            .Ignore(dest => dest.Id);

        config.NewConfig<CrearRepuestoDto, Repuesto>()
            .Map(dest => dest.Codigo, src => src.Codigo.Trim())
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.UnidadMedida, src => src.UnidadMedida.Trim())
            .Ignore(dest => dest.RepuestoProveedores)
            .Ignore(dest => dest.Id);

        config.NewConfig<ActualizarRepuestoDto, Repuesto>()
            .Map(dest => dest.Codigo, src => src.Codigo.Trim())
            .Map(dest => dest.Nombre, src => src.Nombre.Trim())
            .Map(dest => dest.UnidadMedida, src => src.UnidadMedida.Trim())
            .Ignore(dest => dest.RepuestoProveedores)
            .Ignore(dest => dest.Id);

        config.NewConfig<CrearTecnicoEspecialidadDto, TecnicoEspecialidad>()
            .Ignore(dest => dest.TecnicoId)
            .Map(dest => dest.NivelCertificacion, src => src.NivelCertificacion.Trim())
            .Ignore(dest => dest.Tecnico)
            .Ignore(dest => dest.Especialidad);

        config.NewConfig<ActualizarTecnicoEspecialidadDto, TecnicoEspecialidad>()
            .Map(dest => dest.NivelCertificacion, src => src.NivelCertificacion.Trim())
            .Ignore(dest => dest.Tecnico)
            .Ignore(dest => dest.Especialidad)
            .Ignore(dest => dest.TecnicoId)
            .Ignore(dest => dest.EspecialidadId);

        config.NewConfig<CrearRepuestoProveedorDto, RepuestoProveedor>()
            .Ignore(dest => dest.RepuestoId)
            .Ignore(dest => dest.Repuesto)
            .Ignore(dest => dest.Proveedor);

        config.NewConfig<ActualizarRepuestoProveedorDto, RepuestoProveedor>()
            .Ignore(dest => dest.Repuesto)
            .Ignore(dest => dest.Proveedor)
            .Ignore(dest => dest.RepuestoId)
            .Ignore(dest => dest.ProveedorId);

        config.NewConfig<CrearOrdenMantenimientoDto, OrdenMantenimiento>()
            .Map(dest => dest.Motivo, src => src.Motivo.Trim())
            .Map(dest => dest.TipoMantenimiento, src => src.TipoMantenimiento.Trim())
            .Map(dest => dest.Prioridad, src => src.Prioridad.Trim())
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Codigo)
            .Ignore(dest => dest.FechaSolicitud)
            .Ignore(dest => dest.FechaProgramada)
            .Ignore(dest => dest.FechaCierre)
            .Ignore(dest => dest.CostoTotal)
            .Ignore(dest => dest.EstadoActual)
            .Ignore(dest => dest.Activo);

        config.Compile();
        return config;
    }

    public static int CalcularAntiguedadAnios(DateOnly fechaAdquisicion, DateOnly fechaReferencia)
    {
        if (fechaAdquisicion > fechaReferencia)
            return 0;

        var anios = fechaReferencia.Year - fechaAdquisicion.Year;
        if (fechaAdquisicion.AddYears(anios) > fechaReferencia)
            anios--;

        return anios;
    }
}
