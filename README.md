# Mantenimiento de Flota Vehicular

API REST del Avance 1 de ISW-621, desarrollada por el **Equipo 7**: Luis Diego Campos Murillo y Oscar Rojas Garcia. Gestiona activos, categorías, técnicos, especialidades, repuestos, proveedores y solicitudes de mantenimiento.

## Requisitos

- SDK de .NET 10.
- SQL Server y credenciales con permisos para crear y migrar la base de desarrollo.
- Visual Studio o una terminal situada en la carpeta que contiene `MantenimientoFlota.sln`.

## Configuración y ejecución

1. Revisar `ConnectionStrings:DefaultConnection` en `src/MantenimientoFlota.Api/appsettings.Development.json`. La configuración de desarrollo utiliza SQL Server con autenticación SQL; ajustar servidor, base y credenciales al entorno local. Para otros ambientes, proporcionar la conexión mediante configuración externa.
2. Configurar la contraseña inicial del administrador con **al menos 12 caracteres**. En Visual Studio: clic derecho en el proyecto API → **Administrar secretos de usuario**. Agregar esta estructura y sustituir el valor de ejemplo:

   ```json
   {
     "Seed": {
       "Admin": {
         "Password": "SUSTITUIR_POR_UNA_CLAVE_LOCAL"
       }
     }
   }
   ```

   La contraseña se almacena como hash. Cambiar este secreto después de crear el administrador no actualiza su contraseña existente. El usuario inicial sirve como dato maestro; esta versión no expone un inicio de sesión.

3. Restaurar, compilar e iniciar:

   ```powershell
   dotnet restore MantenimientoFlota.sln
   dotnet build MantenimientoFlota.sln --no-restore
   dotnet run --project src/MantenimientoFlota.Api --launch-profile http --no-build --no-restore
   ```

4. Abrir **http://localhost:5295/scalar**. El documento OpenAPI está disponible en **http://localhost:5295/openapi/v1.json**. Mantener la terminal abierta; `Ctrl+C` detiene la API.

El perfil `http` establece `Development`. Si se configuraron variables de entorno previamente, comprobar que no estén sustituyendo la conexión o el ambiente esperado.

### Inicialización de datos

En Development, el arranque aplica las migraciones pendientes y ejecuta los callbacks de inicialización. Incluye catálogos, el administrador inicial y datos de demostración: diez registros por recurso principal y relaciones con atributos. `REP-001` tiene stock 5 y mínimo 10 para demostrar stock bajo.

El seeder de demostración usa una transacción y el marcador `OM-DEMO-001`: los arranques posteriores omiten la carga si el marcador existe. No reconcilia cambios ni repara datos eliminados individualmente. Para una demostración desde cero, utilizar una base nueva y separada de la base habitual. Production no ejecuta la migración automática de arranque ni la carga de demostración.

## API y contratos

Scalar documenta **47 operaciones** en siete grupos. Permite consultar esquemas, ejemplos, parámetros y respuestas, y ejecutar solicitudes.

| Recurso | Ruta base | Operaciones |
| --- | --- | --- |
| Activos | `/api/Activo` | Listar, buscar, consultar, crear, actualizar, dar de baja, cambiar estado y cargar imagen |
| Categorías | `/api/CategoriaActivo` | Listar, consultar, crear, actualizar y eliminar |
| Técnicos | `/api/Tecnico` | CRUD y administración de especialidades |
| Especialidades | `/api/Especialidad` | CRUD |
| Repuestos | `/api/Repuesto` | CRUD, búsqueda y administración de proveedores |
| Proveedores | `/api/Proveedor` | CRUD |
| Órdenes | `/api/OrdenMantenimiento` | Listar, consultar y crear |

Las asociaciones conservan sus atributos y se administran desde el recurso principal:

- `/api/Tecnico/{tecnicoId}/especialidades` y `/{especialidadId}`: nivel de certificación y fecha de obtención.
- `/api/Repuesto/{repuestoId}/proveedores` y `/{proveedorId}`: precio ofrecido y plazo de entrega.

Las rutas de colección admiten GET/POST; las de una asociación admiten GET/PUT/DELETE. El identificador del propietario se toma de la ruta. Seleccionar IDs reales desde los listados antes de enviar solicitudes.

### Búsquedas paginadas

Los listados originales conservan sus filtros. Las nuevas búsquedas están en:

```text
GET /api/Activo/buscar?texto=VEH&pageNumber=1&pageSize=10&sortBy=nombre&sortDirection=asc
GET /api/Repuesto/buscar?stockBajo=true&pageNumber=1&pageSize=10&sortBy=stock&sortDirection=asc
```

`pageNumber` comienza en 1 y `pageSize` admite 1–50 (predeterminado: 10). `sortDirection` admite `asc` o `desc`. Activos permite ordenar por `nombre`, `codigo`, `fecha` o `kilometraje`; Repuestos por `nombre`, `codigo`, `stock` o `minimo`. Los filtros adicionales aparecen en Scalar. La respuesta incluye metadatos de paginación; una búsqueda sin coincidencias devuelve 200 y una lista vacía.

### Respuestas y validación

Las respuestas JSON utilizan `Result`: `isSuccess`, `status`, `title`, `detail` y `data`, además de metadatos cuando corresponda.

| Código | Significado |
| --- | --- |
| 200 | Consulta o modificación completada; incluye Result |
| 201 | Creación completada; incluye Result y cabecera Location |
| 400 | Validación de entrada o solicitud no válida |
| 404 | Recurso inexistente |
| 409 | Duplicado o conflicto con una regla de negocio |
| 413 / 415 | Tamaño o tipo de contenido no admitido |
| 500 | Error inesperado, con mensaje genérico |

Los errores incluyen `traceId`, `errorCode`, `timestampUtc` y `errors`. Las excepciones propias de negocio se conservan para cumplir A5 y se traducen de forma centralizada. Los IDs deben ser positivos; cero es válido cuando representa un estado, stock o intervalo permitido por el contrato.

Estados de Activo: **0 Operativo**, **1 EnMantenimiento**, **2 FueraDeServicio**, **3 DadoDeBaja**. El DELETE de Activo realiza una baja lógica. Las órdenes se crean en **Solicitada (0)**; el flujo posterior queda fuera de este avance.

### Imágenes

La carga de Activo utiliza `multipart/form-data`, campo **Archivo**, con JPEG o PNG de hasta **2 MiB**. Se valida la firma del archivo. Las imágenes se almacenan en `App_Data/imagenes-activos`, configurable mediante `ActivoImagenes:Directorio`, y se consultan mediante la URL devuelta por la API.

## Organización del código

| Proyecto | Responsabilidad |
| --- | --- |
| Domain | Entidades, DTO, constantes, validaciones y excepciones de negocio |
| Infrastructure | AppDbContext, configuración de relaciones, migraciones, seeders e interfaces e implementaciones de repositorios |
| Application | Interfaces e implementaciones de servicios, reglas de negocio, Mapster y almacenamiento de imágenes |
| Api | Controladores, HTTP, OpenAPI/Scalar, validación y manejo global de errores y logs |

Los repositorios consultan y persisten entidades; Application realiza el mapeo y los cálculos de presentación. Los conteos de los detalles y las búsquedas se resuelven en la consulta de base de datos. Las operaciones de EF Core utilizan métodos asíncronos.

## Logs y comprobación

Serilog escribe en consola y en `src/MantenimientoFlota.Api/logs` al ejecutar con el perfil indicado. La ubicación puede cambiarse mediante `ApiLogs:Directorio`. Los archivos rotan diariamente o al alcanzar 10 MiB; se conservan hasta 14 archivos.

El cierre de cada solicitud registra método, ruta, estado, duración y `traceId`: Information para éxitos, Warning para rechazos y Error para fallos inesperados. Development también muestra los comandos SQL. Para comprobar la correlación, copiar el `traceId` de una respuesta de error y localizar el cierre con ese valor en la terminal o el archivo de log.

Antes de preparar la entrega, compilar, comprobar un arranque y reinicio con una base SQL Server de prueba, y verificar en Scalar consultas, escrituras, validación, conflictos, imágenes y logs. La tabla, el ZIP y el video deben corresponder a la misma versión del repositorio.
