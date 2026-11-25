# Revisión de bloques pendientes de Belén

## Alcance y procedencia

Se integra el commit `bc2a698` de `gitlab/feature/permisos-y-usuarios` conservando
su autoría e historial mediante un merge. Su trabajo aporta las políticas por rol,
`ICurrentUser`, el propietario de la orden y la migración correspondiente.
Los bloques pendientes y las pruebas se guardan en un commit posterior.

Se completaron las piezas pendientes del backend: administración de usuarios,
protección del login, proceso transaccional, historial y pruebas reproducibles.
No se agregaron pantallas nuevas ni reportes, dashboard o mantenimiento preventivo.
El frontend existente conserva su estructura y comportamiento.

## Capas y flujo

`Controller → Service de Application → Repository de Infrastructure → AppDbContext → SQL Server`.

Los contratos de entrada y salida permanecen en `Domain/DTO`. Mapster transforma
entidades y DTO; Application calcula importes y valida las transiciones.
Los controladores devuelven `Result` con `FromResult`; las excepciones de negocio
se transforman mediante el handler global. No se devuelven entidades de usuario,
hashes o contraseñas en las respuestas.

`OrdenProcesoRepository.EjecutarAsync` abre una transacción serializable y ejecuta
la operación del servicio usando el mismo DbContext. Guarda y confirma al final;
una excepción revierte la operación completa. En un deadlock se reintenta hasta
tres veces, limpiando el seguimiento y leyendo de nuevo los datos. Un conflicto
de persistencia que no se resuelve devuelve 409. No se reintenta un resultado ya
confirmado por el cliente.

## Bloque 5: permisos

| Acción | Administrador | Coordinador/Técnico | Solicitante/Consulta |
|---|---|---|---|
| Consultar activos | Sí | Sí | Sí |
| Administrar activos y catálogos | Sí | No | No |
| Consultar catálogos | Sí | Sí | No |
| Crear solicitudes | Sí | No | Sí |
| Consultar órdenes y sus detalles | Todas | Todas | Solo propias |
| Operar estados, técnicos y repuestos de órdenes | Sí | Sí | No |
| Administrar usuarios y consultar roles | Sí | No | No |

La documentación es anónima únicamente en Development. Los endpoints sin
`AllowAnonymous` requieren autenticación por defecto. Las imágenes continúan
como archivos públicos, igual que antes, para que puedan mostrarse en el frontend.

El propietario de la orden procede del token, nunca del cuerpo del cliente.
Las órdenes anteriores a la migración mantienen propietario nulo: las consultan
administradores y coordinadores, pero no se atribuyen a ningún solicitante.

## Bloque 6: usuarios

| Método | Ruta | Función |
|---|---|---|
| GET | `/api/Usuario` | Listar usuarios sin credenciales |
| GET | `/api/Usuario/{id}` | Consultar usuario |
| GET | `/api/Usuario/roles` | Consultar roles disponibles |
| POST | `/api/Usuario` | Crear usuario activo con BCrypt |
| PUT | `/api/Usuario/{id}` | Actualizar login, nombre, correo y rol |
| DELETE | `/api/Usuario/{id}` | Desactivar lógicamente |

Crear recibe `login`, `nombreCompleto`, `correo`, `rolId` y `password`.
Actualizar recibe los mismos campos salvo `password`. No existe eliminación física
ni restablecimiento de contraseña en este bloque. Una cuenta desactivada no puede
iniciar sesión ni seguir usando un JWT emitido previamente. Cambiar rol o login
también invalida los tokens previos. El último administrador activo no puede ser
desactivado ni perder su rol. Login y correo son únicos, incluso con concurrencia
gracias a los índices existentes. La contraseña admite al menos 12 caracteres y
como máximo 72 bytes UTF-8, para no truncar silenciosamente la entrada de BCrypt.

El login admite cinco peticiones por IP por minuto, sin cola; incluye intentos
correctos y fallidos. La sexta devuelve 429 con `Retry-After` y error `RATE_LIMITED`.
No se limita el resto de la API con esta política.

## Bloque 7: proceso transaccional

| Estado actual | Transiciones |
|---|---|
| Solicitada (0) | Diagnosticada (1), Cancelada (7) |
| Diagnosticada (1) | Aprobada (2), Rechazada (3), Cancelada (7) |
| Aprobada (2) | En ejecución (4), Cancelada (7) |
| En ejecución (4) | Completada (6), Cancelada (7); espera (5) automática por stock |
| En espera de repuesto (5) | En ejecución (4), cuando todo el lote tenga stock |
| Rechazada, Completada, Cancelada | Ninguna |

### Endpoints

Todas las rutas de esta lista usan `/api/OrdenMantenimiento/{id}` como base:

| Método | Sufijo | Cuerpo / respuesta |
|---|---|---|
| PATCH | `/estado` | `estado`, especialidad al diagnosticar, fecha al aprobar, observación opcional |
| POST | `/tecnicos` | `tecnicoId`, `horasTrabajadas` |
| GET | `/tecnicos` | Participaciones y costo de mano de obra |
| POST | `/repuestos` | `repuestos: [{ repuestoId, cantidad }]` |
| GET | `/repuestos` | Consumos, precios conservados, subtotales y pendientes |
| GET | `/historial` | Cambios de estado, fecha y usuario responsable |

Además: `GET /api/Activo/{id}/historial`, de solo lectura.

### Secuencia de ejemplo

Usar IDs obtenidos en los listados; nunca asumir que coinciden entre computadoras.

1. Crear una solicitud mediante el endpoint existente. Se registra estado 0 y usuario.
2. Diagnosticar: `PATCH .../estado` con
   `{ "estado": 1, "especialidadRequeridaId": 1 }` (sustituir el ID).
3. Aprobar: `{ "estado": 2, "fechaProgramada": "2026-12-01T14:00:00Z" }`.
   La fecha debe ser UTC y no anterior a la solicitud; ajustar el ejemplo al día de prueba.
4. Iniciar: `{ "estado": 4 }`. El activo pasa automáticamente a En mantenimiento.
5. Registrar técnico: `{ "tecnicoId": 1, "horasTrabajadas": 2 }`.
   Debe estar disponible y poseer la especialidad indicada en el diagnóstico.
6. Registrar repuestos: `{ "repuestos": [{ "repuestoId": 1, "cantidad": 2 }] }`.
7. Completar: `{ "estado": 6 }`. Requiere al menos un técnico y ningún repuesto pendiente.

El tipo de mantenimiento sigue siendo texto, como en el contrato anterior. El
diagnóstico indica explícitamente la especialidad necesaria; no se intenta inferir
una especialidad a partir de ese texto.

El costo unitario se obtiene del menor precio registrado entre proveedores, con
desempate por plazo e ID. No lo envía el cliente. Se conserva el precio y la tarifa
del técnico para que cambios posteriores del catálogo no alteren costos anteriores.
Cantidades y horas admiten dos decimales; no se redondean silenciosamente.
El total al cierre suma los subtotales redondeados a dos decimales.

Si un lote no tiene stock completo, ningún repuesto del lote se descuenta:
se conservan las líneas pendientes y la orden cambia a espera. La respuesta 200
explica explícitamente que quedó pendiente. Reanudar valida nuevamente el lote,
descuenta sus cantidades una sola vez y cambia a ejecución, todo en una transacción.
La reposición utiliza el CRUD de repuestos que ya existía.

Se evita ejecutar simultáneamente dos órdenes sobre el mismo activo. Finalizar,
o cancelar una ejecución, devuelve el activo a Operativo y registra ambos historiales.
Cancelar no devuelve al inventario repuestos ya consumidos. Una orden en espera
solo admite reanudación, de acuerdo con la matriz del enunciado.

La solicitud, las transiciones de órdenes y los cambios administrativos de activos
generan historial automáticamente. Reenviar un estado administrativo idéntico no
duplica historial. Los registros anteriores a esta implementación no se reconstruyen
con fechas o responsables inventados.

## Migraciones

1. `20261123070130_AddUsuarioSolicitanteAOrden`: trabajo de Belén, conserva órdenes anteriores.
2. `20261125034234_AgregarProcesoMantenimiento`: especialidad requerida, participaciones,
   repuestos e historial con claves foráneas restrictivas.

No se aplicaron estas migraciones sobre la base principal durante la revisión.
El próximo arranque en Development aplica las migraciones configuradas en Program.
La API que ya estaba abierta sigue ejecutando su versión anterior hasta reiniciarla.

## Bloque 8: pruebas reproducibles

Desde la raíz del proyecto:

```powershell
dotnet build MantenimientoFlota.sln -c Release
dotnet build tests/SqlQa/SqlQa.csproj -c Release
py -3 tests/verificar_bloques_belen.py
```

Requiere Python 3, SQL Server disponible y la conexión local en user-secrets del
proyecto API. El script usa solo la biblioteca estándar de Python. El auxiliar SQL
usa el mismo proveedor .NET que el proyecto; no necesita sqlcmd.

Cada ejecución crea una base nueva `MantenimientoFlotaDb_QA_Belen_<fecha_hora>`.
Lee la conexión local, sustituye el catálogo y genera credenciales de prueba solo
en memoria. Arranca su API en `http://127.0.0.1:5399` y la detiene al finalizar.
No cambia user-secrets, .env ni la conexión del servidor habitual; no borra bases.
Los resultados y logs locales se excluyen de Git en `tests/resultados-belen/`.

Se verifican permisos 401/403; aislamiento entre dos solicitantes; creación,
duplicados, roles y desactivación de usuarios; protección del último administrador;
invalidación de JWT anteriores; validaciones; estados e historial; tarifas, stock
y costos; lote en espera y reanudación; y consumos concurrentes sobre un mismo
repuesto. También se induce un fallo de persistencia en la base aislada para
comprobar que stock, detalles e historial quedan intactos tras el rollback.

El auxiliar de fallo rechaza cualquier conexión cuyo catálogo no tenga el prefijo
de QA. El trigger de prueba se elimina en `finally`; nunca forma parte de las
migraciones ni se instala en la base principal.

## Control de versiones

La integración conserva el commit original de Belén. La implementación adicional
se guarda por separado, con pruebas y documentación. Los logs y resultados locales
quedan fuera de Git. El tag del primer avance no se modifica.
