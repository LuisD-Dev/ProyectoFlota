## Revisión de cumplimiento del Avance 1 - 22/10/2026

La entrega permanece abierta para cambios. No se movió el tag avance-1 ni se
preparó una versión definitiva del ZIP o del video. Después del último commit,
actualizar el hash de la tabla y hacer coincidir tag, repositorio, ZIP y video.

La revisión de los dos documentos de entrega detectó y corrigió dos diferencias:
la jerarquía de excepciones de negocio requerida en A5 y los conteos SQL de
C16, D9, E18 y F20. Se conservan DTO en Domain, Mapster, Result, estados sin
enums y relaciones anidadas en Técnico/Repuesto. No cambian las migraciones.

La comprobación funcional automatizada usa SQLite en memoria; no reemplaza
la demostración de arranque y persistencia sobre SQL Server. Las evidencias
funcionales de la tabla deben completarse con los minutos del nuevo video.
El historial de contribuciones también debe revisarse con ambos integrantes
según la sección 14 del enunciado; no se modifican autores ni fechas históricas.

# Sistema de Gestión de Mantenimiento de Flota Vehicular

## Relaciones integradas en Técnico y Repuesto (vigente)

Las tablas intermedias se mantienen en EF con sus claves compuestas y atributos,
pero ya no tienen controladores propios. TecnicoController administra sus
especialidades y RepuestoController sus proveedores, como en la organización
mostrada en clase. Los servicios y repositorios internos de asociaciones
conservan su lógica y responsabilidades; no se reemplazan las relaciones por
listas de IDs que perderían sus atributos.

| Operación | Técnico | Repuesto |
| --- | --- | --- |
| Listar del propietario | GET /api/Tecnico/{tecnicoId}/especialidades | GET /api/Repuesto/{repuestoId}/proveedores |
| Asociar | POST /api/Tecnico/{tecnicoId}/especialidades | POST /api/Repuesto/{repuestoId}/proveedores |
| Consultar una relación | GET /api/Tecnico/{tecnicoId}/especialidades/{especialidadId} | GET /api/Repuesto/{repuestoId}/proveedores/{proveedorId} |
| Actualizar atributos | PUT /api/Tecnico/{tecnicoId}/especialidades/{especialidadId} | PUT /api/Repuesto/{repuestoId}/proveedores/{proveedorId} |
| Desasociar | DELETE /api/Tecnico/{tecnicoId}/especialidades/{especialidadId} | DELETE /api/Repuesto/{repuestoId}/proveedores/{proveedorId} |

POST de especialidad (el técnico se toma exclusivamente de la ruta):

```json
{
  "especialidadId": 3,
  "nivelCertificacion": "Avanzado",
  "fechaObtencion": "2020-01-15"
}
```

POST de proveedor (el repuesto se toma exclusivamente de la ruta):

```json
{
  "proveedorId": 2,
  "precioOfrecido": 1250,
  "tiempoEntregaDias": 3
}
```

Los números son ejemplos: seleccionar identificadores existentes. PUT recibe
solo certificación/fecha o precio/plazo, como antes. POST conserva 201 y un
Location que apunta al GET anidado; PUT y DELETE conservan 204. Se mantienen
400 por campo, 404 por inexistencia y 409 por duplicado/asociaciones. GET de
colección distingue propietario inexistente (404) de existente sin relaciones
(200 con data=[]). Los detalles de Técnico, Especialidad, Repuesto y Proveedor
siguen mostrando las relaciones y sus cálculos.

Las rutas antiguas /api/TecnicoEspecialidad y /api/RepuestoProveedor se retiran;
no quedan aliases ni grupos de controladores intermedios en Scalar. El documento
OpenAPI conserva 45 operaciones agrupadas en siete controladores. Esta sección
reemplaza las rutas de asociaciones mencionadas en los guiones históricos.

Verificación: 119 comprobaciones HTTP específicas de rutas anidadas y otras 308
de regresión, todas correctas sobre SQLite en memoria con controladores y
repositorios reales. Incluyen Location, aislamiento por propietario, protección
del ID de ruta, atributos, duplicados, bloqueos y desasociación. El esquema EF
no cambia por esta reorganización y no requiere una nueva migración. Pendiente
regresión SQL Server QA. Actualizar esos dos bloques en tabla y video antes de
entregar; no cambian sus requisitos de tablas puente ni atributos.

## Cierre del código acordado — 22/10/2026

Implementados: DTO en Domain, interfaces distribuidas como en el ejercicio,
Mapster en nueve módulos, Result, estados sin enums, catálogos HasData,
Usuario/Rol, seeders maestro y demo, IExceptionHandler, Serilog, escrituras
asíncronas y entidades con seguimiento. AppDbContext está en Infrastructure/Data,
con Seed y Migrations dentro de Data; ModelBuilder configura las 13 entidades
directamente en OnModelCreating. Los IDs y operaciones Up/Down de las seis
migraciones se conservan, aunque se actualizaron rutas y namespaces.

La variante síncrona de DevelopmentSeeder ahora utiliza operaciones síncronas
reales, sin bloquear sobre tareas. Result incluye el método reutilizable Paged
y sus metadatos opcionales, omitidos en respuestas no paginadas; los listados
actuales no cambian a paginación. Los nueve servicios reciben ILogger y registran
operaciones de negocio sin datos personales; las lecturas usan Debug y las
escrituras exitosas Information. El inicio/cierre HTTP conserva su propietario
único y los eventos de servicio no son cierres adicionales.

Regresión del código final: 308 comprobaciones sobre HTTP real contra los nueve
controladores con repositorios reales y SQLite en memoria. Incluye CRUD,
Location, validación, relaciones, calculados, filtros, estado inicial,
protección de campos, baja lógica y carga/reemplazo/lectura de imágenes PNG.
OpenAPI generado: 45 operaciones con summaries, esquemas de Result, respuestas
y ausencia de logger como entrada; calculados ausentes de DTO de escritura.
También pasan 23 comprobaciones de seeding, 57 de persistencia/seguimiento y
49 de estados, modelo y Result.Paged. El handler y los logs cuentan además con
pruebas HTTP aisladas de errores técnicos y respuestas iniciadas.

Estos resultados NO equivalen a migraciones ni pruebas sobre SQL Server. Quedan
pendientes en QA: base nueva y copia de base anterior, ambas variantes de
seeding, regresión sobre SQL Server y revisión visual de Scalar. La tabla, ZIP,
hash/tag de entrega y video todavía deben actualizarse tras validar esa versión.
No se ejecutaron migraciones en SQL Server ni se crearon commits o push.

## DbContext y ModelBuilder como en el ejercicio (vigente)

El DbContext se llama `AppDbContext` y está en
`Infrastructure/Data/AppDbContext.cs`, con Seed dentro de Data, como en el ejercicio.
No es una clase AppDbConfig adicional. Contiene los DbSet y configura las 13
entidades directamente dentro de `OnModelCreating`, con `modelBuilder.Entity<T>`.
Allí se declaran tablas, claves, índices, propiedades, relaciones, restricciones
CHECK y reglas de borrado, igual que en el ejemplo de la profesora.

Se retiraron los archivos IEntityTypeConfiguration separados y su registro
ApplyConfigurationsFromAssembly. `modelBuilder.SeedCatalogs()` permanece al
final de OnModelCreating, como en el ejercicio. El traslado conserva el modelo:
la comparación contra el snapshot vigente produce cero operaciones de migración.
No se renombraron tablas, columnas ni valores de estados, y no se generó una
migración adicional para este traslado. La migración de catálogos previamente
generada sigue pendiente de aplicación en SQL Server.

## Escrituras asíncronas y seguimiento (vigente)

Los repositorios ejecutan las escrituras con SaveChangesAsync; los servicios
esperan UpdateAsync, UpdateEstadoAsync y DeleteAsync. Los borrados por ID usan
FindAsync. No quedan SaveChanges ni Find síncronos en los repositorios de negocio.
Las variantes síncronas de los seeders se conservan para las herramientas EF.

GetByIdAsync y los detalles mantienen lecturas sin seguimiento. GetForUpdateAsync
recupera con seguimiento las entidades que se van a modificar o eliminar. Los
PUT mapean sobre esa misma entidad y protegen claves y navegaciones. Los campos
que no cambian no se marcan como modificados por una actualización del grafo
completo. Se conserva una ruta de compatibilidad para entidades desconectadas
en UpdateAsync. PATCH/baja de Activo persisten solo su propiedad Estado.

Activo conserva su baja lógica, imagen administrada y restricciones por órdenes;
los demás borrados mantienen sus reglas anteriores. Las asociaciones conservan
ambas claves y actualizan sus atributos. No se añade borrado lógico general ni
flujo de estados de órdenes fuera del alcance del Avance 1.

Verificación: 57 comprobaciones SQLite en memoria de lecturas, seguimiento,
actualización y persistencia en los ocho módulos, claves, imagen, borrados y
baja lógica. Un interceptor rechaza SaveChanges síncrono durante la prueba.
Además pasan 40 comprobaciones de estados y modelo. Queda pendiente la
regresión con SQL Server en una base QA aislada.

## Catálogos y seeders según el ejercicio (vigente)

Se separan tres responsabilidades:

- `CatalogSeeder.SeedCatalogs`: HasData de estados de Activo (4), estados de
  Orden (8) y roles (3). IDs técnicos fijos y no cero. Los códigos públicos de
  estado conservan 0–3 y 0–7. Las columnas existentes Estado/EstadoActual ahora
  referencian Codigo mediante FK, sin renumerar ni sustituir sus valores.
- `MasterSeeder`: crea una única cuenta `admin` con rol Administrador, hash
  PBKDF2 con salt aleatorio mediante PasswordHasher, estado activo y fecha UTC.
  Tiene variantes síncrona y asíncrona. Si el login ya existe, no modifica la
  contraseña, el rol, las fechas ni el estado de la cuenta.
- `DevelopmentSeeder`: conserva los diez registros por tabla principal, las
  asociaciones, el stock bajo, la transacción y el marcador existentes. Solo
  se invoca en Development, después del seeder maestro.

Los callbacks UseSeeding/UseAsyncSeeding registran MasterSeeder en todos los
ambientes; los datos demo se agregan únicamente en Development. El arranque
sigue ejecutando MigrateAsync solo en Development. En Production las migraciones
se aplican mediante despliegue controlado; registrar callbacks no las ejecuta.

Los roles son los del enunciado: Administrador, Coordinador/Técnico y
Solicitante/Consulta. Usuario/Rol constituyen la base del administrador inicial;
este lote no incorpora login, JWT, autorización ni CRUD de usuarios del Avance 2.
Las categorías y especialidades editables siguen siendo datos demo, no HasData,
para no convertir el CRUD del Avance 1 en catálogos fijos.

### Configuración del administrador antes del primer arranque

Se requiere `Seed:Admin:Password` (variable de entorno `Seed__Admin__Password`)
con al menos 12 caracteres cuando todavía no existe admin. No hay contraseña
predeterminada ni contraseña/hash fijo en código, configuración o migraciones.
Si ya existe la cuenta, la clave de bootstrap puede retirarse. No restaura una
cuenta desactivada ni cambia credenciales de una cuenta existente.

Para cargar una contraseña propia en una terminal PowerShell dedicada, sin
mostrarla en pantalla ni escribir su contenido en el historial:

```powershell
$seedPasswordInput = Read-Host "Contraseña inicial del administrador (mínimo 12 caracteres)" -AsSecureString
$seedPasswordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($seedPasswordInput)
try {
    $env:Seed__Admin__Password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($seedPasswordPointer)
} finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($seedPasswordPointer)
    $seedPasswordInput.Dispose()
    Remove-Variable seedPasswordInput, seedPasswordPointer
}
```

Después, en esa misma terminal, configurar Development y la conexión de una
base QA aislada según las instrucciones de ejecución. Esta variable complementa
las de entorno y conexión; no arranca la API. Tras finalizar las pruebas y
detener la aplicación, se puede retirar con `Remove-Item Env:Seed__Admin__Password`.
Usar un gestor de secretos en despliegues. No guardar claves en archivos versionados.

### Migración y validación pendientes de SQL Server

Se generó `20261022195435_AddCatalogosYAdministrador`: añade cuatro tablas,
15 filas de catálogo, índices y claves foráneas. No cambia las columnas ni los
datos de negocio existentes. Las cinco migraciones anteriores se conservan.
No contiene la cuenta admin ni hashes: estos se generan en tiempo de ejecución.

La migración no se ha aplicado a ninguna base SQL Server. Antes de arrancar la
nueva versión, seleccionar una base QA aislada y configurar el secreto. En una
copia de una base anterior, los valores de estado deben pertenecer a los
catálogos; un valor desconocido hará fallar la creación de la FK y deberá
investigarse, sin corregir ni borrar datos automáticamente.

Pruebas realizadas: 23 comprobaciones en SQLite en memoria, incluidas ambas
variantes de los callbacks, idempotencia del admin y demo, hash verificable,
rechazo de claves ausentes/cortas, ausencia de reactivación y FK de estado.
Además se revisaron las operaciones de migración y se generó su script SQL
sin conectarse a SQL Server. Estas pruebas no equivalen a aplicar migraciones
en SQL Server: quedan pendientes base nueva, copia de base anterior y HTTP.

Impacto en entrega: actualizar evidencias de inicialización, catálogos, roles y
administrador, sin mostrar la contraseña ni su hash. Mantener pendientes esas
comprobaciones en la tabla hasta ejecutarlas sobre SQL Server.

## Estados numéricos sin enums (contrato vigente)

Las entidades y DTO utilizan `int` o `int?` para los estados. Las constantes de
`Domain/Constants` conservan los valores persistidos y evitan números dispersos:

| Activo | Valor | Orden de mantenimiento | Valor |
| --- | --- | --- | --- |
| Operativo | 0 | Solicitada | 0 |
| EnMantenimiento | 1 | Diagnosticada | 1 |
| FueraDeServicio | 2 | Aprobada | 2 |
| DadoDeBaja | 3 | Rechazada | 3 |
| | | EnEjecucion | 4 |
| | | EnEsperaDeRepuesto | 5 |
| | | Completada | 6 |
| | | Cancelada | 7 |

En JSON y filtros se envían números, no nombres de estado. Crear/actualizar
Activo exige estado entre 0 y 3 mediante Required y Range; el filtro admite
omisión y conserva `estado=0`. PATCH conserva su regla adicional: solo 2 o 3.
Crear una orden sigue asignando Solicitada (0) en el servidor. No se agregan
operaciones de cambio de estado de órdenes al alcance del Avance 1.

El marcador interno de logging también usa constantes de texto, sin enum.
Verificación: 40 comprobaciones de tipos, validación, JSON y modelo EF. La
comparación con el snapshot existente produjo cero operaciones de migración;
las dos columnas siguen siendo SQL int. No se ejecutaron consultas SQL,
migraciones ni cambios de datos. Las 140 comparaciones de servicios y 93
comprobaciones de controladores/logging también pasan con estados numéricos.

La tabla debe reflejar constantes y campos numéricos en vez de enums. Los
números usados en la guía del video se mantienen. Los catálogos y seeders de
clase se incorporan en el apartado vigente anterior.

## Result según el ejercicio de clase (contrato vigente)

`Domain/DTO/PatternResult` contiene `BaseResult` y `Result<T>`. Los nueve servicios
retornan Result en los éxitos. Los rechazos esperados lanzan las excepciones
de Domain/Exceptions: InvalidRequestException (400), ResourceNotFoundException
(404) y ConflictException (409), derivadas de BusinessException. Las reglas y su
orden se conservan. GlobalExceptionHandler traduce la jerarquía al contrato
ApiErrorResponse, que hereda de Result<object?>. Así se conserva Result y se
cumple la jerarquía de excepciones exigida por A5.

Las respuestas JSON exitosas 200 y 201 ahora contienen `isSuccess`, `status`,
`title`, `detail` y `data`. Las listas están en `data`; el identificador creado
se consulta en `data.id`. POST conserva 201 y Location; PUT/PATCH/DELETE
conservan 204 sin cuerpo. Scalar declara el envoltorio Result en sus esquemas.

Los errores HTTP son `ApiErrorResponse : Result<object?>`: conservan traceId,
errors por campo, errorCode, timestampUtc y data null. La fábrica enriquece los
fallos de servicio en la frontera HTTP y mantiene el cierre Warning único de
solicitud. Las excepciones inesperadas se clasifican en el handler global;
la limpieza de imágenes ante rechazo de persistencia no cambia.

Verificación: 140 comparaciones en memoria con los servicios previos a Result,
y comprobaciones directas de controladores y logging. Pendientes las pruebas
HTTP/SQL y la inspección de OpenAPI generado. Estas pruebas no ejecutan SQL.
A5 utiliza la jerarquía vigente de Domain y un único punto de traducción
de excepciones en GlobalExceptionHandler.

Impacto en entrega: actualizar la tabla y las capturas del video para mostrar
`data`, el Result de los servicios y las excepciones de negocio de Domain.
No se declara la entrega revalidada hasta completar las pruebas en ejecución.

IExceptionHandler y Serilog están integrados; su comportamiento vigente se
describe en el apartado de logging. Pendiente regresión completa en SQL Server.

## Mapper de los nueve módulos

Application registra Mapster 10.0.13 e `IMapper` por inyección de dependencias,
como en el ejercicio. `Mapper/MapsterConfig.cs` centraliza 42 configuraciones
para Activo, Categoría, Especialidad, Técnico, Proveedor, Repuesto, Orden de
Mantenimiento y las dos asociaciones. Los servicios realizan la transformación
entre entidades y DTO. Los repositorios devuelven entidades para escrituras y
lecturas generales, y proyecciones de detalle para los cuatro conteos exigidos.

Los detalles de Activo, Categoría, Técnico y Repuesto proyectan los campos y
resúmenes necesarios directamente en LINQ. Sus Count() se traducen a COUNT(*)
en SQL Server antes de materializar los DTO, sin contar entidades completas
en el mapper. Se verificó la traducción usando el proveedor SQL Server sin
abrir conexiones. Los demás detalles mantienen las relaciones y mapeos de Mapster.

Los mapeos de actualización protegen IDs y navegaciones; las claves compuestas
se conservan explícitamente. Activo mantiene la gestión de ImagenUrl y la
compensación de archivos. Orden conserva la generación de código, fecha UTC y
estado inicial en el servicio. Antigüedades y días transcurridos usan una fecha
de referencia capturada por operación. Stock bajo, faltantes y conteos conservan
sus cálculos.

Verificación del lote: 110 comparaciones en memoria de los ocho módulos
migrados frente a las implementaciones anteriores. Incluyen detalles anidados,
normalización, campos protegidos y rechazos de servicio. No sustituyen las
pruebas HTTP ni la comprobación de consultas y ordenación en SQL Server.

## Organización de capas según el ejercicio de clase

Los DTO de negocio se encuentran en `MantenimientoFlota.Domain/DTO` y las
validaciones reutilizables en `Domain/Validations`. Las interfaces de servicios
viven en `Application/Interfaces`; las de repositorios, en
`Infrastructure/Interfaces`. Application referencia Domain e Infrastructure;
Infrastructure referencia únicamente Domain. La API compone las dependencias.

El almacenamiento local de imágenes y su interfaz están en Application, como en
el ejercicio. Se conservan la configuración de directorio, las rutas públicas,
la validación del archivo y la compensación ante fallos de persistencia.
`ImagenUpdateRejectedException` pertenece a `Infrastructure/Exceptions`, ya que
expresa un rechazo de persistencia que Application distingue al compensar.

Los contratos específicos de HTTP (`ApiErrorResponse` y el formulario con
`IFormFile`) permanecen en API; no introducen dependencias de ASP.NET Core en
Domain. Esta reorganización no cambia propiedades JSON, rutas ni códigos HTTP.
Mapster, estados numéricos y seeders están integrados. Las nuevas migraciones
quedan pendientes de aplicación y validación en SQL Server. Las migraciones existentes y los datos no cambian en esta etapa.


## Curso

ISW-621 Programación en Ambiente Web II

## Contexto

Flota vehicular

## Tecnologías previstas

- ASP.NET Core 10 Web API
- Entity Framework Core
- SQL Server
- React + JavaScript
- Clean Architecture
- GitLab

## Arquitectura

- **Domain** contiene las reglas y entidades centrales.
- **Application** contiene casos de uso, interfaces y DTOs.
- **Infrastructure** implementará persistencia y servicios externos.
- **Api** expone los endpoints HTTP.

## Documentación, anotaciones y logging por operación

Las 45 acciones de los nueve controladores tienen EndpointSummary, esquemas de
éxito/error y metadatos de respuesta; 204 no tiene cuerpo. Las escrituras también
documentan 415 por contenido no admitido, sin prometer que ese rechazo externo
use el contrato común. La imagen conserva el 413 posible antes del controlador.

Cada acción recibe [FromServices] ILogger de su controlador y emite Information
con Operation como nombre fijo de operación. Ese evento no registra parámetros,
cuerpos, URLs, cabeceras ni datos personales y no representa inicio/cierre. El
ciclo se registra mediante Serilog y RequestLoggingMiddleware, con un cierre único. FromServices declara
binding de DI, no entrada del cliente; verificar en OpenAPI que logger no aparece
como parámetro ni esquema de solicitud. La validación automática puede impedir
que se ejecute la acción y su evento de operación, conservando el ciclo del middleware.

IDs de ruta y claves compuestas usan Range(1, int.MaxValue). Los filtros opcionales
activoId y especialidadId conservan nulabilidad y usan el mismo rango. Los cinco
IDs nullable de creación de OrdenMantenimiento, TecnicoEspecialidad y
RepuestoProveedor conservan Required y agregan Range. Cero/negativo produce ahora
400 VALIDATION_ERROR con errors por campo y cierre Warning / Validation. Un ID
positivo inexistente mantiene 404. Esto reemplaza los antiguos rechazos manuales
400 INVALID_REQUEST y el 404 anterior de claves no positivas de TecnicoEspecialidad.
Los textos no convertibles en rutas con restricción :int pueden no coincidir y
producir un 404 de enrutamiento fuera del contrato común. Los tipos no convertibles
en query o cuerpo conservan validación automática.

Estado=0 y disponible=false siguen siendo filtros efectivos. Stock e intervalo
permiten cero y omisión con su valor predeterminado, tanto en creación como en
actualización; negativos siguen siendo inválidos. Unicidad permanece en
Application con 409, sin SQL en atributos. La validación de contenido binario
(firma, extensión y tamaño real) complementa las anotaciones de entrada y conserva
errors.Archivo; no es una regla de negocio ni se reduce a DataAnnotations.

### Guion único de pruebas manuales sobre QA (pendiente)

1. Usar exclusivamente MantenimientoFlotaDb_Avance1_QA con las variables temporales
   de la sección siguiente. No iniciar con la conexión de la base habitual.
   Este guion incluye escrituras deliberadas solo en QA; no ha sido ejecutado.
2. Abrir Scalar y /openapi/v1.json. Contar 45 operaciones; comprobar summary no
   vacío, rutas/verbo, entrada JSON o multipart, esquema de éxito, 400/404/409 donde
   corresponda y 500 heredado. logger no debe figurar entre parámetros ni schemas
   de entrada; comprobar los campos calculados de los detalles y de sus resúmenes.
3. Consultar los nueve listados para elegir IDs reales y pares asociados/no asociados.
   Para cada ruta con ID, probar cero, negativo, positivo inexistente y texto.
   Probar cada componente de claves compuestas por separado. Para PUT usar un
   cuerpo válido obtenido de sus campos de entrada, no copiar campos calculados.
   Esperar 400 por campo para cero/negativo, 404 para positivo inexistente y el
   posible 404 de enrutamiento para texto. Comprobar errors, VALIDATION_ERROR,
   timestampUtc con offset cero y traceId idéntico al log.
4. Comparar /api/Activo sin filtros, con categoriaActivoId y estado=0 y ambos;
   /api/Tecnico con disponible=false, true, especialidadId y combinados;
   /api/OrdenMantenimiento con/sin activoId; /api/Repuesto con stockBajo=true/false.
   Probar filtros positivos sin coincidencias (200 []), cero/negativo (solo IDs),
   y valores no convertibles (400 automático). No consultar existencia previa.
5. Probar POST de órdenes con el cuerpo siguiente, sustituyendo activoId por un
   ID real: {"activoId":1,"motivo":"Revisión QA","tipoMantenimiento":"Preventivo","prioridad":"Alta"}.
   Probar omisión/null/cero/negativo (400 por campo), positivo inexistente (404)
   y válido (201 con Location y Solicitada). No intentar PUT/DELETE de órdenes.
6. Probar POST /api/TecnicoEspecialidad con
   {"tecnicoId":1,"especialidadId":1,"nivelCertificacion":"Avanzado","fechaObtencion":"2025-01-15"}.
   Usar IDs reales: faltante/null/cero/negativo en cada ID da 400; técnico o
   especialidad inexistente da 404 en ese orden; par existente da 409. Un par
   nuevo da 201 y Location. PUT del par con
   {"nivelCertificacion":"Intermedio","fechaObtencion":"2025-02-01"} da 204;
   consultar y después desasociar solo el par QA creado (DELETE 204).
7. Repetir en /api/RepuestoProveedor con
   {"repuestoId":1,"proveedorId":1,"precioOfrecido":125.50,"tiempoEntregaDias":3}.
   Se comprueba primero repuesto, luego proveedor, luego duplicado. PUT del par:
   {"precioOfrecido":130.25,"tiempoEntregaDias":5}. Verificar GET, 201/Location,
   204 y 400/404/409; DELETE solo del par QA creado. Los IDs deben elegirse de
   listados y no suponerse iguales a los ejemplos.
8. En /api/Repuesto probar POST con
   {"codigo":"REP-QA-FINAL","nombre":"Repuesto QA","unidadMedida":"Unidad"}:
   stocks omitidos quedan cero. Reenviar en PUT agregando stocks cero y luego
   negativos, cada campo por separado: cero/omisión admitidos, negativo 400 por
   campo. En /api/CategoriaActivo repetir con {"nombre":"Categoría QA final"}
   e intervaloMantenimientoKm omitido, cero y negativo. Probar duplicado 409 y
   conservar código/nombre propio en PUT. Guardar IDs creados y no eliminar datos
   iniciales para limpiar estas pruebas.
9. Verificar por GET los detalles de activos, categorías, técnicos, especialidades,
   repuestos, proveedores y órdenes. Comparar conteos con colecciones; revisar
   antigüedad, días, stockBajo y faltante decimal, sin campos calculados en entradas.
10. Regresión de Activo: usar registros QA sin/con órdenes para baja lógica,
    repetición y restricciones administrativas ya documentadas. Cargar/reemplazar
    JPEG/PNG mediante Archivo; probar vacío, firma incompatible y exceso de 2 MiB.
    Debe conservar 400 con errors.Archivo y INVALID_REQUEST. PUT conserva la ruta
    administrada, y listado/detalle y URL pública conservan su comportamiento.
11. Para acciones válidas que se ejecutan, observar un evento de operación y un
    solo inicio/cierre del middleware. Rechazos automáticos pueden no tener evento
    de acción. Comprobar niveles y que los eventos de operación solo contienen el
    nombre fijo. No provocar fallos de infraestructura para probar Error; ese
    recorrido y Response.HasStarted requieren una prueba aislada posterior.

## Inicialización de Development y prueba desde cero

Antes de aceptar solicitudes, únicamente en Development, Program crea un scope
y ejecuta MigrateAsync sobre AppDbContext. Esto crea la base si
no existe, aplica las migraciones pendientes e invoca el callback UseAsyncSeeding
ya registrado. No se llama además al seeder ni se usa EnsureCreated/EnsureDeleted.
Production no ejecuta migraciones ni datos demo automáticamente.

Si la inicialización falla, el arranque se detiene. El registro propio informa
únicamente el tipo de excepción, sin mensaje interno, cadena de conexión ni
credenciales. Un nuevo arranque podrá aplicar las migraciones pendientes: las
migraciones y el lote del seeder no constituyen una única transacción global.

El seeder conserva su transacción y marcador OM-DEMO-001. En un segundo arranque,
con el marcador presente, no reinserta ni actualiza los datos. No reconcilia bases
existentes: si falta el marcador en una base parcialmente poblada puede intentar
insertar duplicados y fallar; si existe, no repone otros registros faltantes.
Ante esos casos hay que detenerse y revisar, sin borrar, renombrar ni fusionar
datos automáticamente. Cambiar DemoData no actualiza registros ya sembrados.
REP-001 se inicializa con StockActual=5 y StockMinimo=10; códigos, asociaciones,
precios y cantidades de registros permanecen iguales.

Prueba manual pendiente, desde la raíz del repositorio, en una terminal PowerShell
dedicada. Primero comprobar mediante una conexión de solo lectura al servidor
que MantenimientoFlotaDb_Avance1_QA no existe; si ya existe, detenerse. No se
propone reutilizarla, eliminarla ni sobrescribir MantenimientoFlotaDb.

Configurar variables temporales del proceso, sin editar appsettings. El ejemplo
usa autenticación integrada y no contiene credenciales; ajustar Server a la
instancia de pruebas si corresponde:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "Server=.;Database=MantenimientoFlotaDb_Avance1_QA;Integrated Security=True;TrustServerCertificate=True;"
$env:ActivoImagenes__Directorio = "App_Data/imagenes-activos-avance1-qa"
dotnet build MantenimientoFlota.sln --no-restore
```

Las variables no modifican bases y la compilación solo genera archivos locales.
Si faltan dependencias o hay archivos bloqueados, detenerse. El siguiente comando
sí inicia la API y crea/modifica exclusivamente la base QA configurada mediante
migraciones y seeding; no hace falta un database update previo:

```powershell
dotnet run --project src/MantenimientoFlota.Api --no-launch-profile --no-build --no-restore --urls https://localhost:7090
```

Abrir https://localhost:7090/scalar después de completar el inicio. Comprobar con
GET los seis catálogos y órdenes (10 registros iniciales cada uno), las dos
asociaciones (15 cada una), /api/Repuesto?stockBajo=true (REP-001), activos no
operativos y un técnico con varias especialidades. No asumir IDs numéricos.
Opcionalmente usar SELECT en QA para confirmar DB_NAME(), las cinco entradas de
__EFMigrationsHistory y los conteos. Estas consultas y los GET son comprobaciones
de lectura. Detener manualmente y repetir el mismo arranque, que puede ejecutar
migraciones pendientes y el callback: comparar conteos, IDs y contenido para
comprobar que se conservan. No efectuar escrituras de prueba en esta verificación.

Para comprobar Production posteriormente, usar variables temporales y
--no-launch-profile, manteniendo la conexión explícita a QA: el bloque de
inicialización no debe ejecutarse y Scalar no se publica. No probar este caso
cambiando la conexión a la base actual. Cerrar la terminal dedicada al terminar
descarta las variables temporales. Este procedimiento está documentado, no
ejecutado; reproducibilidad y segundo arranque requieren comprobación real.

## Logging y excepciones inesperadas (A4/A5, vigente)

La integración sigue la presentación Clase 6-1: `AddExceptionHandler` registra
`GlobalExceptionHandler : IExceptionHandler`; `AddProblemDetails` habilita la
infraestructura y `UseExceptionHandler` protege los endpoints. Los fallos
esperados se expresan con BusinessException y sus tres derivadas de Domain.
GlobalExceptionHandler conserva los mensajes, errores por campo y códigos
400/404/409, con Warning. Los fallos inesperados mantienen 500 genérico y
Error con tipo y stack seguros. El middleware anterior se retiró.

Serilog.AspNetCore 10.0.0 reemplaza al proveedor de logging anterior. La
configuración central está en `Api/Common/ApiLoggingConfiguration.cs`. Se
registran consola y archivo diario `logs/serilog-AAAAMMDD.txt` bajo ContentRoot;
`ApiLogs:Directorio` permite cambiar el directorio. Hay rotación diaria y por
10 MiB, retención máxima de 14 archivos y escritura compartida. Los archivos
runtime quedan excluidos de Git. Los niveles se configuran en ese componente;
la sección anterior Logging de appsettings ya no se usa.

Orden de ejecución: UseSerilogRequestLogging (mediante UseApiRequestLogging),
RequestLoggingMiddleware, UseExceptionHandler y los endpoints. Serilog mide
la duración hasta que termina el manejador de errores y emite un solo cierre.
RequestLoggingMiddleware registra el inicio y conserva el TraceId en el scope.
El cierre contiene TraceId, método, plantilla de ruta, Status y DurationMs:

- 2xx: Information / Success.
- Validación y rechazos esperados 400/404/409: Warning / Validation o ExpectedFailure.
- Error técnico 500: Error / UnexpectedException.
- Otras respuestas: Information / Unclassified, sin atribuir una causa de negocio.

El texto de cierre es `Solicitud finalizada`; buscar el TraceId del JSON y
comprobar Status=200 con INF o un rechazo con WRN. Los logs de operación de los
controladores se conservan. Un error manejado añade un evento de diagnóstico
`Excepción procesada` con tipo y StackTrace; ese evento no es un segundo cierre.

El handler conserva ApiErrorResponse (derivado de Result), traceId, errorCode,
errors, timestampUtc y el mensaje público genérico. Concurrencia y conflictos
SQL Server identificados (547, 2601 y 2627 dentro de DbUpdateException) producen
409 con mensaje genérico. ImagenUpdateRejectedException se inspecciona sin
alterar su compensación. Un DbUpdateException sin causa identificable o que
envuelve una caída de infraestructura conserva 500: no se clasifica cualquier
fallo de guardado como conflicto. Esto precisa la clasificación simplificada
del ejemplo de clase.

No se pasan excepciones originales a nuestros loggers ni se imprimen Message,
ToString, Data o excepciones internas. Se conservan tipo completo y StackTrace;
las trazas pueden incluir nombres de código y rutas de fuente. Se suprimen los
diagnósticos duplicados del middleware estándar para errores manejados. El
filtro de Serilog también excluye eventos Microsoft con objeto Exception y
el evento de error EF Update, cuyo texto puede incluir la excepción completa.
Los comandos SQL siguen en Information, con datos sensibles de EF desactivados.
Estos filtros limitan detalle diagnóstico del framework deliberadamente.
No habilitar EnableSensitiveDataLogging con datos o credenciales reales.

Si la respuesta ya comenzó, no se reemplaza su cuerpo: la excepción se relanza
y se registra un único cierre `Solicitud interrumpida` con estado real,
ResponseStarted, tipo y traza segura. Ese caso suprime el cierre adicional de
Serilog para no volver a registrar la excepción original.

Verificación del lote: 14 comprobaciones con HTTP real contra un servidor
temporal sin SQL. Incluyen 200/400/404/409/500, concurrencia, restricciones SQL
simuladas, caída simulada, rechazo de imagen, respuesta iniciada, correlación,
niveles, cierre único y escritura a archivo sin el secreto sintético de prueba.
Además pasan las 88 comprobaciones directas de controladores. Queda pendiente
la regresión completa con SQL Server en QA y la inspección de Scalar/OpenAPI.

Referencias: [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore),
[manejo de errores en ASP.NET Core 10](https://learn.microsoft.com/aspnet/core/fundamentals/error-handling?view=aspnetcore-10.0).

## Formato de errores (A6)

Los errores construidos por la API conservan `isSuccess`, `status`, `title`,
`detail`, `data`, `traceId` y `errors`, y agregan:

- `errorCode`: código estable de texto, independiente del estado HTTP y del mensaje.
- `timestampUtc`: instante de construcción, capturado una sola vez mediante
  `DateTimeOffset.UtcNow`, en ISO 8601 con offset UTC cero (`+00:00`).

| Código | Uso |
|---|---|
| `VALIDATION_ERROR` | Validación automática de DTO, conversión o deserialización |
| `INVALID_REQUEST` | Rechazo explícito 400, incluso si incluye errores por campo |
| `RESOURCE_NOT_FOUND` | Recurso inexistente, 404 explícito |
| `CONFLICT` | Conflicto explícito, 409 |
| `UNEXPECTED_ERROR` | Error inesperado, 500 con mensaje genérico |

Ejemplo ilustrativo de validación automática:

```json
{
  "isSuccess": false,
  "status": 400,
  "title": "Error de validación",
  "detail": "Revise los campos indicados.",
  "data": null,
  "traceId": "identificador-de-la-solicitud",
  "errors": { "Estado": ["El estado es obligatorio."] },
  "errorCode": "VALIDATION_ERROR",
  "timestampUtc": "2026-10-20T18:30:00+00:00"
}
```

La fábrica selecciona explícitamente el código de validación automática; no lo
infiere de la presencia de errores por campo. `traceId` conserva exactamente
`HttpContext.TraceIdentifier`, también utilizado en el log. Los mensajes y claves
de validación no cambian. Los errores inesperados no exponen mensajes internos,
SQL, rutas físicas ni trazas de excepción.

El esquema compartido `ApiErrorResponse` documenta ambos campos para Scalar.
Se conserva la clasificación de logging: Validation y ExpectedFailure como
Warning, UnexpectedException como Error, y respuestas exitosas como Information /
Success. Las respuestas exitosas no incorporan estos campos.

Este alcance no incluye errores de enrutamiento, límites del servidor/proxy,
archivos estáticos ni respuestas ya iniciadas. No implica completar A4 o A5,
que siguen pendientes. La visualización del esquema, los errores HTTP, el offset
UTC y la coincidencia del traceId con el log requieren verificación posterior.

## Imágenes de activos (C13)

`PUT /api/Activo/{id}/imagen` recibe `multipart/form-data` con el campo obligatorio
`Archivo`. Admite JPEG (`.jpg`, `.jpeg`) y PNG (`.png`), no vacíos, con máximo
2 MiB de contenido. Se comprueban extensión, firma binaria, coherencia y bytes
realmente leídos. No se confía en Content-Type ni se decodifica completamente
la imagen: la firma no demuestra que todo su contenido sea una imagen válida.

La respuesta 200 contiene, por ejemplo:

```json
{"id":12,"imagenUrl":"/imagenes/activos/12/8a219d74eb084386a427ddc13e659d682.jpg"}
```

Abrir esa ruta usando el origen de la API permite consultar el archivo. Listado
y detalle conservan sus contratos y devuelven la misma propiedad ImagenUrl.
Los nombres son GUID generados por el servidor; el nombre del cliente solo se
utiliza para validar la extensión. SQL guarda exclusivamente la URL.

`ActivoImagenes:Directorio` configura una única raíz para escritura y publicación.
Por defecto es `App_Data/imagenes-activos`, relativa al directorio de contenido
de Api. Se publica bajo `/imagenes/activos`, sin listado de directorios. Las
cargas locales de esa ubicación están excluidas de Git. Una ubicación alternativa
debe persistirse y respaldarse por separado; no debe compartirse con archivos
ajenos ni contener enlaces simbólicos o puntos de unión.

Archivo ausente, vacío, superior a 2 MiB o incompatible produce 400 común; ID
inválido produce 400, y activo inexistente con entrada válida produce 404.
El límite de recepción es 3 MiB para dejar margen al multipart; el servidor o
proxy puede responder 413 antes del controlador. Scalar documenta el formulario
y estas respuestas. No se agregan endpoints de eliminación de imágenes.

El reemplazo guarda primero un archivo nuevo, actualiza únicamente ImagenUrl
y después intenta retirar el archivo administrado anterior del mismo activo.
URLs externas, rutas absolutas y rutas no canónicas nunca se eliminan.
Los fallos de escritura intentan limpiar el archivo parcial. Ante un rechazo SQL
conocido (restricciones o ausencia de fila) se intenta limpiar la imagen nueva;
otros errores, incluidos desconexiones y cancelaciones, pueden tener resultado
incierto y conservan ambos archivos. Los fallos de limpieza registran una
incidencia sin exponer rutas ni mensajes internos, conservando el resultado SQL.
Disco y SQL no tienen atomicidad conjunta: fallos o reemplazos concurrentes
pueden dejar archivos huérfanos que requieren revisión posterior.

POST/PUT rechazan asignaciones al espacio reservado, incluso mediante URLs
absolutas cuyo path apunte a él. PUT permite reenviar la ruta administrada propia
exacta; si el activo ya tiene imagen administrada, conserva esa URL aunque se
omita o se envíe una URL externa. Para reemplazarla se usa `/imagen`. Sin imagen
administrada, las URLs externas mantienen el comportamiento previo. No se
descargan URLs externas ni se modifican automáticamente datos históricos.

Pruebas manuales pendientes: carga JPEG/PNG y apertura en navegador, consulta,
reemplazo, conservación mediante PUT, rechazo de rutas reservadas, tamaños
límite, firmas incompatibles, ID inválido/inexistente y fallos de disco/SQL/limpieza.

## Código de activos (C17)

POST y PUT de activos validan el código con el mismo atributo `CodigoActivo`.
El patrón, aplicado después de `Trim()`, es:

```regex
\AVEH-(?:[A-Z]+-)*[0-9]{3}\z
```

El prefijo `VEH-` es obligatorio. Puede incluir segmentos descriptivos de letras
ASCII mayúsculas separados por guiones y debe terminar en exactamente tres
dígitos, incluido `000`. No se generan secuencias automáticamente.

| Entrada | Resultado |
|---|---|
| `VEH-001`, `VEH-010`, `VEH-000` | Válidos |
| `VEH-QA-BAJA-001` | Válido |
| ` VEH-001 ` | Válido; se guarda `VEH-001` |
| `veh-001`, `VEH-Qa-001` | Inválidos: minúsculas |
| `VEH-QA1-001`, `VEH--001` | Inválidos: segmento descriptivo incorrecto |
| `VEH-01`, `VEH-0001` | Inválidos: terminación distinta de tres dígitos |
| `VEH QA-001`, `VEH- 001` | Inválidos: espacios interiores |

`Required` rechaza valores ausentes, vacíos o compuestos solo por espacios.
La entrada completa admite como máximo 50 caracteres, incluidos los espacios
exteriores: superar ese límite se rechaza aunque el código recortado sea válido.
El servicio conserva su `Trim()` y comprueba unicidad sobre el mismo valor que
guarda. No convierte minúsculas ni modifica códigos históricos.

Un formato inválido produce 400 automático con errores por campo. Un código
válido duplicado conserva el 409 existente; PUT excluye el propio activo al
comprobar duplicados. Los datos existentes no se renombran ni se eliminan; un
código histórico incompatible sería rechazado si se reenvía mediante POST o PUT.

Ambos DTO de entrada describen la propiedad `Codigo` para su documentación en
Scalar. El atributo personalizado no genera automáticamente un `pattern` de
OpenAPI; la visualización de las descripciones debe verificarse en Scalar.
