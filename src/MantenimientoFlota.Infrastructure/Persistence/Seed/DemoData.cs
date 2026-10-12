using MantenimientoFlota.Domain.Entities;
using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Infrastructure.Persistence.Seed;

internal static class DemoData
{
    public const string SeedMarkerOrderCode = "OM-DEMO-001";

    public static IReadOnlyList<CategoriaActivo> CreateCategorias() =>
    [
        new()
        {
            Nombre = "Vehículos livianos",
            Descripcion = "Automóviles y camionetas destinados al transporte operativo.",
            IntervaloMantenimientoKm = 5_000
        },
        new()
        {
            Nombre = "Vehículos pesados",
            Descripcion = "Camiones utilizados para transporte de carga y equipos.",
            IntervaloMantenimientoKm = 10_000
        },
        new()
        {
            Nombre = "Motocicletas",
            Descripcion = "Motocicletas para mensajería, inspección y desplazamientos cortos.",
            IntervaloMantenimientoKm = 3_000
        },
        new()
        {
            Nombre = "Maquinaria amarilla",
            Descripcion = "Maquinaria pesada empleada en construcción y movimiento de tierra.",
            IntervaloMantenimientoKm = 8_000
        },
        new()
        {
            Nombre = "Montacargas",
            Descripcion = "Equipos para manipulación y traslado interno de materiales.",
            IntervaloMantenimientoKm = 2_500
        },
        new()
        {
            Nombre = "Autobuses",
            Descripcion = "Unidades de transporte colectivo de personal.",
            IntervaloMantenimientoKm = 10_000
        },
        new()
        {
            Nombre = "Microbuses",
            Descripcion = "Unidades compactas para transporte de grupos pequeños.",
            IntervaloMantenimientoKm = 7_500
        },
        new()
        {
            Nombre = "Vehículos eléctricos",
            Descripcion = "Vehículos impulsados total o parcialmente por energía eléctrica.",
            IntervaloMantenimientoKm = 12_000
        },
        new()
        {
            Nombre = "Remolques",
            Descripcion = "Unidades sin motor destinadas al transporte complementario de carga.",
            IntervaloMantenimientoKm = 15_000
        },
        new()
        {
            Nombre = "Equipos especiales",
            Descripcion = "Vehículos adaptados para funciones técnicas y operativas especiales.",
            IntervaloMantenimientoKm = 6_000
        }
    ];

    public static IReadOnlyList<Especialidad> CreateEspecialidades() =>
    [
        new()
        {
            Nombre = "Mecánica general",
            Descripcion = "Diagnóstico y reparación de sistemas mecánicos del vehículo."
        },
        new()
        {
            Nombre = "Electricidad automotriz",
            Descripcion = "Mantenimiento de sistemas eléctricos, arranque, carga e iluminación."
        },
        new()
        {
            Nombre = "Sistemas diésel",
            Descripcion = "Diagnóstico y reparación de motores y sistemas de inyección diésel."
        },
        new()
        {
            Nombre = "Frenos y suspensión",
            Descripcion = "Servicio de frenos, dirección, amortiguación y componentes asociados."
        },
        new()
        {
            Nombre = "Aire acondicionado",
            Descripcion = "Mantenimiento de climatización, ventilación y refrigeración vehicular."
        },
        new()
        {
            Nombre = "Hidráulica",
            Descripcion = "Revisión y reparación de bombas, cilindros y circuitos hidráulicos."
        },
        new()
        {
            Nombre = "Transmisión",
            Descripcion = "Servicio de transmisiones manuales, automáticas y sistemas de embrague."
        },
        new()
        {
            Nombre = "Llantas y alineado",
            Descripcion = "Montaje, balanceo, alineado y control del desgaste de neumáticos."
        },
        new()
        {
            Nombre = "Carrocería y pintura",
            Descripcion = "Reparación estructural, acabado y protección de carrocerías."
        },
        new()
        {
            Nombre = "Movilidad eléctrica",
            Descripcion = "Diagnóstico de baterías de tracción, inversores y motores eléctricos."
        }
    ];

    public static IReadOnlyList<Tecnico> CreateTecnicos() =>
    [
        new()
        {
            NombreCompleto = "Carlos Méndez Rojas",
            Identificacion = "1-1111-1111",
            Telefono = "8888-1101",
            Correo = "carlos.mendez@flota.example",
            Disponible = true,
            TarifaPorHora = 18_500m
        },
        new()
        {
            NombreCompleto = "Laura Jiménez Solano",
            Identificacion = "2-2222-2222",
            Telefono = "8888-1102",
            Correo = "laura.jimenez@flota.example",
            Disponible = true,
            TarifaPorHora = 19_750m
        },
        new()
        {
            NombreCompleto = "Andrés Vargas Mora",
            Identificacion = "3-3333-3333",
            Telefono = "8888-1103",
            Correo = "andres.vargas@flota.example",
            Disponible = false,
            TarifaPorHora = 21_000m
        },
        new()
        {
            NombreCompleto = "Sofía Ramírez Castro",
            Identificacion = "4-4444-4444",
            Telefono = "8888-1104",
            Correo = "sofia.ramirez@flota.example",
            Disponible = true,
            TarifaPorHora = 20_500m
        },
        new()
        {
            NombreCompleto = "Daniel Chaves Araya",
            Identificacion = "1-5555-5555",
            Telefono = "8888-1105",
            Correo = "daniel.chaves@flota.example",
            Disponible = true,
            TarifaPorHora = 22_250m
        },
        new()
        {
            NombreCompleto = "María Fernanda León",
            Identificacion = "2-6666-6666",
            Telefono = "8888-1106",
            Correo = "maria.leon@flota.example",
            Disponible = false,
            TarifaPorHora = 19_000m
        },
        new()
        {
            NombreCompleto = "José Pablo Quesada",
            Identificacion = "3-7777-7777",
            Telefono = "8888-1107",
            Correo = "jose.quesada@flota.example",
            Disponible = true,
            TarifaPorHora = 23_500m
        },
        new()
        {
            NombreCompleto = "Valeria Alfaro Brenes",
            Identificacion = "4-8888-8888",
            Telefono = "8888-1108",
            Correo = "valeria.alfaro@flota.example",
            Disponible = true,
            TarifaPorHora = 20_750m
        },
        new()
        {
            NombreCompleto = "Miguel Salas Cordero",
            Identificacion = "1-9999-9999",
            Telefono = "8888-1109",
            Correo = "miguel.salas@flota.example",
            Disponible = true,
            TarifaPorHora = 18_750m
        },
        new()
        {
            NombreCompleto = "Natalia Zúñiga Herrera",
            Identificacion = "2-1010-1010",
            Telefono = "8888-1110",
            Correo = "natalia.zuniga@flota.example",
            Disponible = false,
            TarifaPorHora = 24_000m
        }
    ];

    public static IReadOnlyList<Repuesto> CreateRepuestos() =>
    [
        new()
        {
            Codigo = "REP-001",
            Nombre = "Filtro de aceite",
            UnidadMedida = "Unidad",
            StockActual = 35m,
            StockMinimo = 10m
        },
        new()
        {
            Codigo = "REP-002",
            Nombre = "Filtro de aire",
            UnidadMedida = "Unidad",
            StockActual = 24m,
            StockMinimo = 8m
        },
        new()
        {
            Codigo = "REP-003",
            Nombre = "Pastillas de freno delanteras",
            UnidadMedida = "Juego",
            StockActual = 18m,
            StockMinimo = 6m
        },
        new()
        {
            Codigo = "REP-004",
            Nombre = "Aceite de motor 15W-40",
            UnidadMedida = "Litro",
            StockActual = 120m,
            StockMinimo = 40m
        },
        new()
        {
            Codigo = "REP-005",
            Nombre = "Batería de 12 voltios",
            UnidadMedida = "Unidad",
            StockActual = 12m,
            StockMinimo = 4m
        },
        new()
        {
            Codigo = "REP-006",
            Nombre = "Correa de accesorios",
            UnidadMedida = "Unidad",
            StockActual = 16m,
            StockMinimo = 5m
        },
        new()
        {
            Codigo = "REP-007",
            Nombre = "Refrigerante de larga duración",
            UnidadMedida = "Litro",
            StockActual = 75m,
            StockMinimo = 25m
        },
        new()
        {
            Codigo = "REP-008",
            Nombre = "Bombillo LED H4",
            UnidadMedida = "Unidad",
            StockActual = 30m,
            StockMinimo = 10m
        },
        new()
        {
            Codigo = "REP-009",
            Nombre = "Amortiguador delantero",
            UnidadMedida = "Unidad",
            StockActual = 10m,
            StockMinimo = 4m
        },
        new()
        {
            Codigo = "REP-010",
            Nombre = "Llanta 225/75 R16",
            UnidadMedida = "Unidad",
            StockActual = 20m,
            StockMinimo = 8m
        }
    ];

    public static IReadOnlyList<Proveedor> CreateProveedores() =>
    [
        new()
        {
            Nombre = "Repuestos del Valle S.A.",
            Contacto = "Ana Mora | 2222-3101 | ventas@repuestos.example",
            Direccion = "San José, Barrio Cuba, 200 metros al sur del parque.",
            CondicionesGenerales = "Crédito a 30 días y entrega sin costo en el GAM."
        },
        new()
        {
            Nombre = "Autopartes La Sabana Ltda.",
            Contacto = "Luis Rojas | 2222-3102 | pedidos@autopartes.example",
            Direccion = "San José, Mata Redonda, costado oeste del estadio.",
            CondicionesGenerales = "Pago de contado con descuento del cinco por ciento."
        },
        new()
        {
            Nombre = "Distribuidora Motor Tico",
            Contacto = "Paola Vega | 2222-3103 | comercial@motortico.example",
            Direccion = "Heredia, La Valencia, zona industrial.",
            CondicionesGenerales = "Crédito a 45 días sujeto a aprobación."
        },
        new()
        {
            Nombre = "Frenos y Más Costa Rica",
            Contacto = "Marco Solís | 2222-3104 | servicio@frenos.example",
            Direccion = "Alajuela centro, 150 metros al este de la terminal.",
            CondicionesGenerales = "Garantía de seis meses en componentes de fricción."
        },
        new()
        {
            Nombre = "Lubricantes Centroamericanos",
            Contacto = "Elena Castro | 2222-3105 | ventas@lubricantes.example",
            Direccion = "Cartago, Ochomogo, parque industrial.",
            CondicionesGenerales = "Pedido mínimo de veinte litros; entrega en tres días."
        },
        new()
        {
            Nombre = "ElectroAuto Nacional",
            Contacto = "Javier Arias | 2222-3106 | cotizaciones@electroauto.example",
            Direccion = "San José, La Uruca, frente a la plaza de deportes.",
            CondicionesGenerales = "Garantía de doce meses en baterías y alternadores."
        },
        new()
        {
            Nombre = "Importadora Diésel CR",
            Contacto = "Andrea Núñez | 2222-3107 | repuestos@diesel.example",
            Direccion = "Heredia, Santo Domingo, ruta 32.",
            CondicionesGenerales = "Piezas importadas bajo pedido con anticipo del cincuenta por ciento."
        },
        new()
        {
            Nombre = "Llantas Seguras S.A.",
            Contacto = "Ricardo Campos | 2222-3108 | flotas@llantas.example",
            Direccion = "Alajuela, El Coyol, complejo logístico oeste.",
            CondicionesGenerales = "Montaje y balanceo incluidos para compras mayores a ocho unidades."
        },
        new()
        {
            Nombre = "Suministros Hidráulicos del Este",
            Contacto = "Gabriela Soto | 2222-3109 | pedidos@hidraulica.example",
            Direccion = "Cartago, La Lima, 300 metros al norte de la zona franca.",
            CondicionesGenerales = "Fabricación de mangueras a medida con entrega en dos días."
        },
        new()
        {
            Nombre = "Movilidad Eléctrica Técnica",
            Contacto = "Esteban Ruiz | 2222-3110 | soporte@movilidad.example",
            Direccion = "San José, Curridabat, centro empresarial del este.",
            CondicionesGenerales = "Componentes especializados con diagnóstico técnico previo."
        }
    ];

    public static IReadOnlyList<Activo> CreateActivos(
        IReadOnlyDictionary<string, int> categoriaIdsByNombre) =>
    [
        new()
        {
            Codigo = "VEH-001",
            Nombre = "Toyota Hilux operativa",
            Descripcion = "Camioneta doble cabina para supervisión de operaciones.",
            FechaAdquisicion = new DateOnly(2022, 3, 15),
            ValorReferencia = 24_500_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-001.jpg",
            Placa = "ABC-101",
            KilometrajeActual = 48_250,
            CategoriaActivoId = categoriaIdsByNombre["Vehículos livianos"]
        },
        new()
        {
            Codigo = "VEH-002",
            Nombre = "Isuzu NPR de carga",
            Descripcion = "Camión de carga mediana para distribución regional.",
            FechaAdquisicion = new DateOnly(2021, 7, 8),
            ValorReferencia = 38_900_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-002.jpg",
            Placa = "CL-2020",
            KilometrajeActual = 96_400,
            CategoriaActivoId = categoriaIdsByNombre["Vehículos pesados"]
        },
        new()
        {
            Codigo = "VEH-003",
            Nombre = "Honda XR190 de inspección",
            Descripcion = "Motocicleta para inspecciones de campo y mensajería.",
            FechaAdquisicion = new DateOnly(2023, 1, 20),
            ValorReferencia = 2_850_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-003.jpg",
            Placa = "M-3030",
            KilometrajeActual = 18_750,
            CategoriaActivoId = categoriaIdsByNombre["Motocicletas"]
        },
        new()
        {
            Codigo = "VEH-004",
            Nombre = "Retroexcavadora Caterpillar 416F2",
            Descripcion = "Equipo para excavación y carga en proyectos de infraestructura.",
            FechaAdquisicion = new DateOnly(2020, 11, 5),
            ValorReferencia = 72_000_000m,
            Estado = EstadoActivo.EnMantenimiento,
            ImagenUrl = "https://flota.example/vehiculos/veh-004.jpg",
            Placa = "EE-4040",
            KilometrajeActual = 12_600,
            CategoriaActivoId = categoriaIdsByNombre["Maquinaria amarilla"]
        },
        new()
        {
            Codigo = "VEH-005",
            Nombre = "Montacargas Toyota 8FG25",
            Descripcion = "Montacargas de combustión para operaciones de bodega.",
            FechaAdquisicion = new DateOnly(2022, 9, 12),
            ValorReferencia = 16_800_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-005.jpg",
            Placa = "INT-005",
            KilometrajeActual = 7_300,
            CategoriaActivoId = categoriaIdsByNombre["Montacargas"]
        },
        new()
        {
            Codigo = "VEH-006",
            Nombre = "Autobús Mercedes-Benz OF-1721",
            Descripcion = "Autobús para traslado diario de personal operativo.",
            FechaAdquisicion = new DateOnly(2019, 6, 18),
            ValorReferencia = 58_500_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-006.jpg",
            Placa = "BUS-606",
            KilometrajeActual = 184_900,
            CategoriaActivoId = categoriaIdsByNombre["Autobuses"]
        },
        new()
        {
            Codigo = "VEH-007",
            Nombre = "Hyundai H-1 de transporte",
            Descripcion = "Microbús para visitas técnicas y transporte administrativo.",
            FechaAdquisicion = new DateOnly(2021, 2, 25),
            ValorReferencia = 21_300_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-007.jpg",
            Placa = "MB-7070",
            KilometrajeActual = 67_100,
            CategoriaActivoId = categoriaIdsByNombre["Microbuses"]
        },
        new()
        {
            Codigo = "VEH-008",
            Nombre = "BYD T3 eléctrica",
            Descripcion = "Furgoneta eléctrica para entregas urbanas de última milla.",
            FechaAdquisicion = new DateOnly(2024, 4, 10),
            ValorReferencia = 29_750_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-008.jpg",
            Placa = "ELE-808",
            KilometrajeActual = 14_450,
            CategoriaActivoId = categoriaIdsByNombre["Vehículos eléctricos"]
        },
        new()
        {
            Codigo = "VEH-009",
            Nombre = "Remolque Great Dane",
            Descripcion = "Remolque cerrado de dos ejes para transporte de carga seca.",
            FechaAdquisicion = new DateOnly(2020, 8, 30),
            ValorReferencia = 12_400_000m,
            Estado = EstadoActivo.Operativo,
            ImagenUrl = "https://flota.example/vehiculos/veh-009.jpg",
            Placa = "REM-909",
            KilometrajeActual = 88_000,
            CategoriaActivoId = categoriaIdsByNombre["Remolques"]
        },
        new()
        {
            Codigo = "VEH-010",
            Nombre = "Camión taller móvil",
            Descripcion = "Unidad adaptada con herramientas para asistencia mecánica en campo.",
            FechaAdquisicion = new DateOnly(2022, 12, 1),
            ValorReferencia = 44_600_000m,
            Estado = EstadoActivo.FueraDeServicio,
            ImagenUrl = "https://flota.example/vehiculos/veh-010.jpg",
            Placa = "TAL-010",
            KilometrajeActual = 52_800,
            CategoriaActivoId = categoriaIdsByNombre["Equipos especiales"]
        }
    ];

    public static IReadOnlyList<TecnicoEspecialidad> CreateTecnicoEspecialidades(
        IReadOnlyDictionary<string, int> tecnicoIdsByIdentificacion,
        IReadOnlyDictionary<string, int> especialidadIdsByNombre)
    {
        TecnicoEspecialidad CreateTecnicoEspecialidad(
            string identificacion,
            string especialidad,
            string nivelCertificacion,
            int year,
            int month,
            int day) =>
            new()
            {
                TecnicoId = tecnicoIdsByIdentificacion[identificacion],
                EspecialidadId = especialidadIdsByNombre[especialidad],
                NivelCertificacion = nivelCertificacion,
                FechaObtencion = new DateOnly(year, month, day)
            };

        return
        [
        CreateTecnicoEspecialidad("1-1111-1111", "Mecánica general", "Avanzado", 2018, 2, 10),
        CreateTecnicoEspecialidad("1-1111-1111", "Frenos y suspensión", "Intermedio", 2019, 8, 22),
        CreateTecnicoEspecialidad("2-2222-2222", "Electricidad automotriz", "Avanzado", 2017, 5, 14),
        CreateTecnicoEspecialidad("2-2222-2222", "Aire acondicionado", "Intermedio", 2020, 3, 7),
        CreateTecnicoEspecialidad("3-3333-3333", "Sistemas diésel", "Avanzado", 2016, 11, 18),
        CreateTecnicoEspecialidad("3-3333-3333", "Transmisión", "Avanzado", 2019, 6, 25),
        CreateTecnicoEspecialidad("4-4444-4444", "Frenos y suspensión", "Avanzado", 2018, 9, 30),
        CreateTecnicoEspecialidad("1-5555-5555", "Hidráulica", "Avanzado", 2015, 4, 12),
        CreateTecnicoEspecialidad("1-5555-5555", "Sistemas diésel", "Intermedio", 2021, 7, 19),
        CreateTecnicoEspecialidad("2-6666-6666", "Llantas y alineado", "Avanzado", 2019, 1, 26),
        CreateTecnicoEspecialidad("3-7777-7777", "Carrocería y pintura", "Avanzado", 2014, 10, 9),
        CreateTecnicoEspecialidad("4-8888-8888", "Movilidad eléctrica", "Avanzado", 2022, 6, 16),
        CreateTecnicoEspecialidad("1-9999-9999", "Mecánica general", "Intermedio", 2020, 2, 28),
        CreateTecnicoEspecialidad("1-9999-9999", "Transmisión", "Intermedio", 2021, 9, 11),
            CreateTecnicoEspecialidad("2-1010-1010", "Aire acondicionado", "Avanzado", 2018, 12, 3)
        ];
    }

    public static IReadOnlyList<RepuestoProveedor> CreateRepuestoProveedores(
        IReadOnlyDictionary<string, int> repuestoIdsByCodigo,
        IReadOnlyDictionary<string, int> proveedorIdsByNombre)
    {
        RepuestoProveedor CreateRepuestoProveedor(
            string codigoRepuesto,
            string proveedor,
            decimal precioOfrecido,
            int tiempoEntregaDias) =>
            new()
            {
                RepuestoId = repuestoIdsByCodigo[codigoRepuesto],
                ProveedorId = proveedorIdsByNombre[proveedor],
                PrecioOfrecido = precioOfrecido,
                TiempoEntregaDias = tiempoEntregaDias
            };

        return
        [
        CreateRepuestoProveedor("REP-001", "Repuestos del Valle S.A.", 4_850m, 2),
        CreateRepuestoProveedor("REP-001", "Autopartes La Sabana Ltda.", 5_100m, 1),
        CreateRepuestoProveedor("REP-002", "Repuestos del Valle S.A.", 7_250m, 2),
        CreateRepuestoProveedor("REP-003", "Frenos y Más Costa Rica", 32_500m, 3),
        CreateRepuestoProveedor("REP-003", "Autopartes La Sabana Ltda.", 34_900m, 2),
        CreateRepuestoProveedor("REP-004", "Lubricantes Centroamericanos", 4_200m, 2),
        CreateRepuestoProveedor("REP-005", "ElectroAuto Nacional", 68_500m, 2),
        CreateRepuestoProveedor("REP-005", "Distribuidora Motor Tico", 71_000m, 3),
        CreateRepuestoProveedor("REP-006", "Importadora Diésel CR", 24_750m, 7),
        CreateRepuestoProveedor("REP-007", "Lubricantes Centroamericanos", 3_900m, 2),
        CreateRepuestoProveedor("REP-008", "ElectroAuto Nacional", 8_600m, 1),
        CreateRepuestoProveedor("REP-009", "Repuestos del Valle S.A.", 56_000m, 4),
        CreateRepuestoProveedor("REP-009", "Suministros Hidráulicos del Este", 58_500m, 3),
        CreateRepuestoProveedor("REP-010", "Llantas Seguras S.A.", 92_000m, 2),
            CreateRepuestoProveedor("REP-010", "Distribuidora Motor Tico", 95_500m, 4)
        ];
    }

    public static IReadOnlyList<OrdenMantenimiento> CreateOrdenesMantenimiento(
        IReadOnlyDictionary<string, int> activoIdsByCodigo)
    {
        OrdenMantenimiento CreateOrden(
            string codigo,
            string codigoActivo,
            string motivo,
            string tipoMantenimiento,
            DateTime fechaSolicitud,
            string prioridad) =>
            new()
            {
                Codigo = codigo,
                ActivoId = activoIdsByCodigo[codigoActivo],
                Motivo = motivo,
                TipoMantenimiento = tipoMantenimiento,
                FechaSolicitud = fechaSolicitud,
                FechaProgramada = null,
                FechaCierre = null,
                Prioridad = prioridad,
                CostoTotal = 0m,
                EstadoActual = EstadoOrdenMantenimiento.Solicitada
            };

        return
        [
        CreateOrden(
            SeedMarkerOrderCode,
            "VEH-001",
            "Cambio de aceite y revisión general por kilometraje.",
            "Preventivo",
            new DateTime(2026, 10, 1, 8, 15, 0),
            "Media"),
        CreateOrden(
            "OM-DEMO-002",
            "VEH-002",
            "Inspección del sistema de frenos por ruido durante la operación.",
            "Correctivo",
            new DateTime(2026, 10, 2, 9, 30, 0),
            "Alta"),
        CreateOrden(
            "OM-DEMO-003",
            "VEH-003",
            "Mantenimiento periódico de motor, cadena y sistema eléctrico.",
            "Preventivo",
            new DateTime(2026, 10, 3, 10, 0, 0),
            "Baja"),
        CreateOrden(
            "OM-DEMO-004",
            "VEH-004",
            "Diagnóstico de pérdida de presión en el circuito hidráulico.",
            "Correctivo",
            new DateTime(2026, 10, 4, 7, 45, 0),
            "Alta"),
        CreateOrden(
            "OM-DEMO-005",
            "VEH-005",
            "Revisión preventiva de horquillas, mástil y sistema de elevación.",
            "Preventivo",
            new DateTime(2026, 10, 5, 11, 20, 0),
            "Media"),
        CreateOrden(
            "OM-DEMO-006",
            "VEH-006",
            "Revisión de suspensión y alineado por vibración en carretera.",
            "Correctivo",
            new DateTime(2026, 10, 6, 13, 10, 0),
            "Alta"),
        CreateOrden(
            "OM-DEMO-007",
            "VEH-007",
            "Servicio de aire acondicionado y sustitución del filtro de cabina.",
            "Preventivo",
            new DateTime(2026, 10, 7, 14, 25, 0),
            "Media"),
        CreateOrden(
            "OM-DEMO-008",
            "VEH-008",
            "Diagnóstico preventivo de batería de tracción y sistema de carga.",
            "Preventivo",
            new DateTime(2026, 10, 8, 8, 40, 0),
            "Media"),
        CreateOrden(
            "OM-DEMO-009",
            "VEH-009",
            "Inspección de luces, rodamientos y sistema de acople.",
            "Preventivo",
            new DateTime(2026, 10, 9, 15, 5, 0),
            "Baja"),
            CreateOrden(
                "OM-DEMO-010",
                "VEH-010",
                "Diagnóstico de falla de arranque reportada durante asistencia en campo.",
                "Correctivo",
                new DateTime(2026, 10, 10, 6, 50, 0),
                "Alta")
        ];
    }
}
