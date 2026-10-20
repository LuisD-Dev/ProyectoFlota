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
