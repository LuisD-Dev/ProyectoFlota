# Bloque 5: pruebas de políticas y permisos

Preparación: aplicar la migración, ejecutar `Bloque5_usuarios_prueba.sql` y obtener un token por usuario
con `POST /api/Auth/login` (`admin`, `coord1`, `solic1`, `solic2`). En Scalar, el token se envía como Bearer.

| # | Usuario | Petición | Resultado esperado |
|---|---------|----------|--------------------|
| 1 | sin token | `GET /api/Activo` | 401, `errorCode = UNAUTHORIZED` |
| 2 | sin token | `POST /api/Auth/login` | 200 (sigue anónimo) |
| 3 | sin token | `GET /scalar` y `/openapi/v1.json` (Development) | 200 |
| 4 | solic1 | `POST /api/Activo` | 403, `errorCode = FORBIDDEN` |
| 5 | admin | `POST /api/Activo` (datos válidos) | 201 |
| 6 | coord1 | `GET /api/Tecnico` | 200 |
| 7 | coord1 | `POST /api/Tecnico` | 403 |
| 8 | solic1 | `GET /api/Tecnico` (y Categoría, Repuesto, Proveedor, Especialidad) | 403 |
| 9 | coord1 | `GET /api/Activo` | 200 |
| 10 | coord1 | `POST /api/OrdenMantenimiento` | 403 (solo Administrador y Solicitante crean) |
| 11 | solic1 | `POST /api/OrdenMantenimiento` | 201 y `UsuarioSolicitanteId` = id de solic1 en la base |
| 12 | solic1 | `GET /api/OrdenMantenimiento` | solo aparece su orden |
| 13 | solic2 | `GET /api/OrdenMantenimiento` | lista vacía |
| 14 | solic2 | `GET /api/OrdenMantenimiento/{id de la orden de solic1}` | 403 |
| 15 | solic1 | `GET /api/OrdenMantenimiento/{id}` (la suya) | 200 |
| 16 | coord1 / admin | `GET /api/OrdenMantenimiento` | todas las órdenes |
| 17 | solic2 | `GET /api/Activo/{id}` del activo de la orden de solic1 | 200, pero `ordenes` no incluye la de solic1 |

Verificación en la base:
`SELECT Id, Codigo, UsuarioSolicitanteId FROM OrdenesMantenimiento ORDER BY Id DESC;`
