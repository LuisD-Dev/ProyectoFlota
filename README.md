# Sistema de Gestión de Mantenimiento de Flota Vehicular

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
middleware sigue siendo el único propietario del ciclo. FromServices declara
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
y ejecuta MigrateAsync sobre MantenimientoFlotaDbContext. Esto crea la base si
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

## Ciclo de solicitudes (A4, primera etapa)

El middleware registra un inicio Information y un único cierre propio por
solicitud que lo atraviesa. Incluye TraceId, método HTTP, plantilla de ruta cuando
está disponible, estado HTTP y DurationMs medido mediante Stopwatch. En el inicio,
el estado es el valor actual de la respuesta, no una predicción del resultado;
la plantilla puede ser nula si todavía no se seleccionó el endpoint.

Los cierres 2xx mantienen Information / Success; la validación automática y los
rechazos esperados mantienen Warning / Validation y Warning / ExpectedFailure.
Una excepción inesperada tiene únicamente su cierre Error / UnexpectedException.
Las demás respuestas tienen cierre Information sin clasificación de negocio.
Los cierres se emiten exclusivamente desde finally, incluyendo el tiempo de
construcción del error. Esto no elimina los eventos propios del framework.

En excepciones se registran como campos estructurados únicamente el nombre
completo del tipo y StackTrace, además de los metadatos de solicitud. No se pasa
el objeto original al logger ni se registran Message, ToString, Data o
InnerException. Se omite por tanto la información adicional de excepciones
internas; el stack trace puede incluir nombres de código y rutas de fuente.
No se registran valores de ruta, URL completa, query string, cuerpos ni cabeceras.

Si la respuesta ya comenzó, se conserva su estado real, se registra
ResponseStarted=true y se relanza sin reemplazar el cuerpo. El contrato JSON,
traceId y mensajes de error permanecen intactos. La migración parcial de A5 se
describe en la sección siguiente.

Pendiente de verificar en ejecución: inicio y un cierre por TraceId, niveles y
clasificaciones, duración, coincidencia con el traceId del JSON, y comportamiento
ante excepciones antes/después de iniciar respuesta. A4 no está completamente
validado por revisión estática y compilación.

## Excepciones de negocio (A5, primera etapa)

Application define BusinessException abstracta y las excepciones concretas
InvalidRequestException, ResourceNotFoundException y ConflictException, sin
dependencias HTTP. Sus mensajes son públicos y controlados; no deben incorporar
mensajes de proveedores ni detalles técnicos. El único middleware traduce esas
clases a 400, 404 y 409 mediante ApiErrorFactory, con INVALID_REQUEST,
RESOURCE_NOT_FOUND y CONFLICT, respectivamente. Conserva todos los campos A6.

Especialidad se migró en la primera etapa: IDs inválidos, detalle inexistente,
duplicados de nombre y actualización/eliminación inexistente o eliminación con
técnicos asociados. Se conserva Trim(), la exclusión del propio ID y la prioridad
de existencia antes de duplicados/asociaciones. POST conserva 201, su DTO y
Location; PUT/DELETE conservan 204. El contrato interno de servicio devuelve el
DTO creado, detalle no nullable y Task para actualizar/eliminar. Se retira el
resultado de operaciones de Especialidad al no quedar consumidores.

Los rechazos de negocio tienen un único cierre Warning / ExpectedFailure con
TraceId y duración. Si la respuesta ya empezó, se registra el estado real y se
relanza sin sustituirla. Los errores técnicos conservan el tratamiento genérico
500; no se convierten excepciones de persistencia indiscriminadamente a 409.
La validación automática por campo permanece intacta como Validation.

Categoría de Activo utiliza ahora la misma jerarquía existente: ID inválido
produce InvalidRequestException; detalle, actualización y eliminación inexistentes
producen ResourceNotFoundException; nombre duplicado y eliminación con activos
asociados producen ConflictException. Conserva exactamente los mensajes, Trim(),
la exclusión del propio ID y la comprobación de existencia antes del duplicado o
de las asociaciones. El controlador ya no traduce resultados ni null a errores.
El contrato interno devuelve CategoriaActivoDto al crear, detalle no nullable y
Task al actualizar/eliminar; se retira CategoriaActivoOperationResult sin
consumidores. Se conservan listado, detalle con activos y cantidad, POST 201 con
Location, PUT/DELETE 204, DTO, repositorios y validación automática por campo.
Pruebas manuales pendientes para Categoría: ID cero/negativo, detalle/PUT/DELETE
inexistentes, duplicado en POST/PUT, conservación del nombre propio, eliminación
con/sin activos, detalle con/sin activos, respuestas exitosas y correlación del
traceId con un único cierre Warning / ExpectedFailure en rechazos.

Proveedor utiliza también las excepciones existentes: InvalidRequestException
para IDs inválidos, ResourceNotFoundException para detalle/actualización/eliminación
inexistentes y ConflictException para eliminación con repuestos asociados. No se
agrega validación de duplicados ni otra regla. Se mantiene la comprobación de
existencia antes de asociaciones y la normalización Trim() de sus campos.
IProveedorService devuelve ProveedorDto al crear, detalle no nullable y Task al
actualizar/eliminar. Se retira ProveedorOperationResult sin consumidores. Listado,
detalle con repuestos/precios/plazos, DTO, consultas, repositorios y
RepuestoProveedor permanecen intactos. POST conserva 201 y Location; PUT/DELETE,
204. Pruebas manuales pendientes: IDs inválidos, 404, eliminación con/sin
asociaciones, validación por campo, detalle vacío o con repuestos, normalización,
respuestas exitosas y correlación traceId / cierre Warning.

Repuesto utiliza las mismas excepciones: InvalidRequestException para IDs
inválidos, ResourceNotFoundException para detalle/actualización/eliminación
inexistentes y ConflictException para código duplicado o eliminación con
proveedores asociados. POST/PUT conservan el código recortado con Trim() para
comprobar y guardar; PUT excluye el propio ID y comprueba existencia antes del
duplicado. Se conservan exactamente mensajes y códigos HTTP. No se traducen
fallos técnicos ni colisiones concurrentes de SQL a 409 en esta migración.
IRepuestoService devuelve RepuestoDto al crear, detalle no nullable y Task al
actualizar/eliminar; RepuestoOperationResult se retira sin consumidores.
Listado, filtro stockBajo, detalle con proveedores/conteos/indicadores, DTO,
repositorios, índice y RepuestoProveedor permanecen intactos. POST conserva 201
y Location; PUT/DELETE, 204. Pruebas manuales pendientes: IDs inválidos, 404,
duplicado en POST/PUT (incluidos espacios exteriores), PUT con código propio,
404 prioritario al actualizar un ID inexistente con código ajeno, eliminación
con/sin proveedores, validación por campo, éxitos, listado/filtro/detalle y
coincidencia del traceId con el cierre Warning.

Técnico utiliza también las excepciones existentes: InvalidRequestException para
IDs o especialidadId cero/negativos, ResourceNotFoundException para detalle,
actualización o eliminación inexistentes y ConflictException para eliminación
con especialidades asociadas. Conserva mensajes, normalización y comprobación
de existencia antes de asociaciones. No se agregan reglas de unicidad: el
servicio anterior no comprobaba duplicados. ITecnicoService devuelve TecnicoDto
al crear, detalle no nullable y Task al actualizar/eliminar; se retira
TecnicoOperationResult sin consumidores. Se conservan disponible/especialidadId,
su combinación AND y disponible=false, el detalle con certificaciones, fechas,
conteo y antigüedad con una referencia UTC, DTO, repositorios y TecnicoEspecialidad.
POST mantiene 201 y Location; PUT/DELETE, 204; la validación automática no cambia.
Pruebas manuales pendientes: IDs/filtros inválidos, 404, eliminación con/sin
especialidades, filtros individuales/combinados (incluido false), detalle con
cero/varias asociaciones y fechas límite, validación por campo, éxitos y traceId
correlacionado con el único cierre Warning.

Activo utiliza las excepciones existentes para detalle inexistente, códigos
duplicados, categoría inexistente, rutas reservadas, baja lógica y estados
administrativos. Conserva mensajes y prioridades: PUT comprueba existencia,
ruta, código, categoría y cambio administrativo antes de escribir; PATCH valida
destino antes de existencia y órdenes abiertas antes de resolver una repetición.
DELETE sigue rechazando cualquier orden y no escribe al repetir una baja sin
órdenes. Listado y validación automática de filtros (incluido estado=0), patrón
del código, detalle y cálculos permanecen intactos. IActivoService devuelve
ActivoDto al crear, detalle e imagen no nullable y Task para PUT/PATCH/DELETE;
se retira ActivoOperationResult al no quedar consumidores.

La validación del archivo es validación de entrada: conserva exactamente su
respuesta por campo mediante la fábrica (400, detail, errors.Archivo e
INVALID_REQUEST); no es una excepción de negocio pendiente. La ausencia del
activo se lanza antes de guardar la imagen y fuera del try de SQL. El catch
exclusivo de ImagenUpdateRejectedException conserva la limpieza de la imagen
nueva y relanza el fallo técnico; no se convierte a 409. Otros resultados SQL
inciertos siguen conservando ambos archivos. Solo tras éxito SQL se intenta
retirar la imagen anterior administrada. Almacenamiento y protección de rutas,
actualización exclusiva de ImagenUrl y validadores no se modifican.
Pruebas manuales pendientes: prioridades de 400/404/409, duplicados, categoría,
filtros, baja y repetición, órdenes abiertas/finales, PATCH/PUT sin cambios
parciales, éxitos/Location, errores por campo de archivo, carga/reemplazo y
fallos de persistencia/limpieza sobre un entorno de pruebas.

OrdenMantenimiento, TecnicoEspecialidad y RepuestoProveedor utilizan ahora las
excepciones existentes. Se retiran sus tres resultados de operaciones sin
consumidores. Los contratos devuelven el DTO creado y detalle no nullable;
actualización y eliminación de asociaciones devuelven Task. Órdenes conserva
únicamente listado, detalle y creación, con código generado, estado Solicitada,
normalización y cálculo de días intactos.

Se conserva el orden de POST: activo existente para órdenes; técnico, especialidad
y duplicado para TecnicoEspecialidad; repuesto, proveedor y duplicado para
RepuestoProveedor. Los mensajes y códigos HTTP no cambian. TecnicoEspecialidad
conservaba 404 para claves no positivas antes del lote de anotaciones. Actualmente
las claves cero/negativas de ambas asociaciones producen 400 automático por campo;
claves positivas sin asociación mantienen 404.
POST mantiene 201, DTO y Location con sus claves actuales; PUT/DELETE de
asociaciones mantienen 204. No se alteran consultas, DTO ni datos.

La búsqueda estática global no encuentra traducciones de rechazos de negocio
en los controladores ni resultados de fallo en servicios. La respuesta por campo
de validación de archivo en Activo permanece como validación de entrada, al igual
que la validación automática de DTO. La migración estática de los recorridos
actuales queda realizada; A5 aún requiere comprobaciones de ejecución antes de
declararse completamente validado. Pruebas manuales pendientes:
400 por ID, 404 de detalle/PUT/DELETE, 409 por duplicados o asociaciones,
actualización conservando el nombre propio, respuestas exitosas y Location,
validación automática y coincidencia de traceId con el único cierre Warning.

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
