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

Los catálogos restantes, órdenes y asociaciones (incluido TecnicoEspecialidad)
mantienen sus mecanismos actuales: A5 sigue parcial. Pruebas manuales pendientes:
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
