# Revisión del Avance 1 - Equipo 7

Fecha: 22/10/2026. Integrantes: Luis Diego Campos Murillo y Oscar Rojas Garcia.
Contexto: flota vehicular. Base de revisión: `1c5d466e61df2727ce7faccfd463c5459f75db31` más las correcciones descritas aquí, todavía sin commit.
La entrega permanece abierta para ajustes. Este informe no congela una versión ni reemplaza el video o la defensa.

## Documentos contrastados

- Enunciado Proyecto ISW-621.pdf: alcance del Avance 1 (página impresa 5), stack (6-7), módulos aplicables (8-10), historial de commits (21-23).
- Instrucciones_entrega_defensa_Avance1_ISW-621_G02.pdf: cuatro entregables y coincidencia de versiones (página 1), orden del video (2-3), tabla y evidencias (3), defensa (4-6).
- TablaLlenaLuisDiego.xlsx: 98 rubros originales A1-G11. Se conservaron IDs, textos, orden, listas de validación, formato condicional y fórmulas del resumen, salvo la fórmula de preparación final, que ahora muestra los pendientes de entrega.

## Resultado técnico por sección

| Sección | Rubros | Revisión |
| --- | ---: | --- |
| A. Arquitectura y transversales | 9 | Capas, documentación, ILogger/Serilog, jerarquía de excepciones, contrato de errores, validaciones y calculados. |
| B. Persistencia | 6 | Modelo EF, FK, tablas puente, seis migraciones, seeders e idempotencia. |
| C. Activos | 20 | CRUD, filtros, baja lógica, estado administrativo, imágenes, categoría, órdenes y calculados. |
| D. Categorías | 11 | CRUD, unicidad, restricción de borrado, intervalo y detalle con activos. |
| E. Técnicos y especialidades | 20 | CRUD, filtros, relaciones con atributos, asociación/desasociación y antigüedades. |
| F. Repuestos y proveedores | 21 | CRUD, stock bajo, condiciones N:N, actualización y calculados. |
| G. Órdenes | 11 | Crear solicitudes, listar/filtrar y consultar detalle. Estado inicial asignado por el servidor. |

Los 98 rubros se declaran completos respecto de la implementación revisada y las pruebas aisladas. La demostración de la versión vigente en Scalar/SQL Server, los minutos del video y los datos finales de entrega siguen pendientes. El porcentaje del Excel cuenta rubros declarados completos; no es una calificación ni una garantía de aceptación.

## Diferencias detectadas y corregidas

### A5: excepciones propias compatibles con Result

El refactor anterior había eliminado la jerarquía y usaba Result.Failure en todos los rechazos de negocio. Esto no satisfacía el texto literal de A5.

Se añadieron `Domain/Exceptions/BusinessException`, `InvalidRequestException`, `ResourceNotFoundException` y `ConflictException`. Los nueve servicios lanzan las derivadas correspondientes en sus 87 puntos de rechazo, conservando reglas, prioridades y mensajes. Las firmas siguen retornando Result y las respuestas exitosas 200/201 mantienen `data`; 204 permanece sin cuerpo.

`GlobalExceptionHandler` es el único traductor de excepciones y produce 400/404/409 para la jerarquía. El resultado de error sigue siendo `ApiErrorResponse : Result<object?>`. Se conservaron `errorCode`, `traceId`, `timestampUtc` y errores por campo. Los 400 por categoría inexistente, ruta reservada de imagen o destino administrativo inválido identifican el campo. La validación automática y la validación binaria de imágenes conservan su contrato.

Los fallos inesperados mantienen 500 genérico y Error con tipo/stack seguros. Se preserva la plantilla de ruta incluso tras una excepción de negocio. No se registran el objeto original de excepción, datos de solicitudes ni credenciales.

### C16, D9, E18 y F20: conteos en SQL

El refactor anterior obtenía entidades con Include y contaba sus colecciones en Mapster. Se reemplazaron esos cuatro detalles por proyecciones LINQ de DTO: Activo, CategoriaActivo, Tecnico y Repuesto.

Los `Count()` se traducen a `COUNT(*)` en SQL Server antes de materializar el resultado. Las colecciones se proyectan únicamente a los resúmenes requeridos. Las antigüedades siguen calculándose en Application. Mapster permanece en todos los módulos para las demás conversiones de entidades, creación, actualización y consultas.

No se añadieron columnas calculadas, enums, rutas ni migraciones. No cambian los IDs ni los datos.

## Comprobaciones realizadas sobre la versión corregida

| Comprobación | Resultado | Entorno / límite |
| --- | --- | --- |
| Compilación de la solución | Correcta, 0 errores y 0 advertencias | SDK preliminar emite NETSDK1057 informativo. |
| Regresión HTTP | 427 comprobaciones correctas | Controladores, servicios y repositorios reales; SQLite en memoria. |
| Excepciones, logging y archivo | 17 comprobaciones correctas | HTTP aislado; incluye las tres derivadas, 500, fallos de persistencia simulados y respuesta iniciada. |
| Correlación de errores | 101 respuestas comprobadas | Cada traceId tiene un cierre Warning con ruta y duración. |
| Seeders | 23 comprobaciones correctas | SQLite; sync/async, idempotencia, 10 registros por tabla principal, hash y ausencia de contraseña predeterminada. |
| Estados, JSON y modelo | 49 comprobaciones correctas | Sin enums; metadatos Result.Paged; sin operaciones de migración pendientes respecto del snapshot. |
| Proyecciones SQL Server | Las cuatro contienen COUNT(*) | Compiladas con proveedor SQL Server; interceptor impide abrir la conexión. |
| OpenAPI | 45 operaciones en siete grupos | Summaries y esquemas; ILogger ausente como entrada; asociaciones anidadas. |
| Excel exportado | 98 rubros, cero estados vacíos y cero errores de fórmula | Dos hojas originales; fórmulas, validaciones y formato condicional conservados. |
| Diff | Sin errores de whitespace | No se crearon commits ni se ejecutó push. |

Las pruebas HTTP y los seeders aislados no sustituyen una regresión completa sobre SQL Server. El usuario había mostrado el arranque y creación de `MantenimientoFlotaDb_Refactor_Equipo7_QA`; esta revisión no abrió ni modificó bases SQL Server, no aplicó migraciones y no ejecutó la API de producción. Los servidores aislados de esta revisión se detuvieron.

## Lo que debe cambiar en la tabla y el video

- Rutas N:N actuales: `/api/Tecnico/{tecnicoId}/especialidades` y `/api/Repuesto/{repuestoId}/proveedores`, con GET/PUT/DELETE del par al agregar el ID relacionado.
- En POST de asociaciones, el propietario proviene de la ruta. El cuerpo ya no lleva tecnicoId o repuestoId.
- Respuestas exitosas: mostrar `data`, por ejemplo `data.id`. No reutilizar como evidencia de esta versión las respuestas anteriores sin envoltorio.
- Mostrar desde una base SQL Server nueva el arranque/migraciones/seeding de la versión vigente y las consultas posteriores que demuestran persistencia.
- Mantener el orden exigido: presentación, base desde cero, Scalar, Activos, Categorías, Técnicos/Especialidades, Repuestos/Proveedores, Órdenes y logs.
- Máximo 15 minutos, audio claro y enlace reproducible en navegador sin pedir acceso. Se permiten cortes de tiempos muertos, no ocultar o sustituir respuestas.
- Registrar minutos reales mm:ss en las evidencias funcionales. No se inventaron tiempos ni enlace del video nuevo.

## Pendientes de entrega que no se resuelven cambiando código

1. **Historial de participación:** el historial local revisado contiene 58 commits atribuidos a Luis Diego y ninguno atribuido a Oscar. La sección 14 exige participación real de ambos; los umbrales exactos incluyen 25% por integrante para el nivel adecuado y señalan como crítico menos de 10% de un integrante. La evaluación considera también continuidad semanal. Debe aclararse el trabajo/historial real con el docente; no se deben cambiar autores, fechas ni fabricar commits para aparentar participación. No se verificaron ramas remotas no presentes en el checkout.
2. **Versión única:** el tag local avance-1 sigue en `84dbb50fea7cb706348caf645bcd86d6fc973078`, anterior a los seis commits y a estas correcciones. Al acordar la versión final antes del plazo, coordinar tag, GitLab, ZIP, tabla y video. No se movió ni publicó el tag en esta revisión.
3. **ZIP:** regenerarlo desde la versión definitiva, sin bin/obj y sin credenciales, logs o archivos locales de desarrollo. No se inspeccionó un ZIP actualizado en esta revisión.
4. **Video y Excel:** completar enlace, duración, minutos y hash final después del commit. La fecha efectiva de entrega permanece por confirmar; el límite indicado es 27/10/2026 a las 12:00 m. d.
5. **Defensa:** ambos integrantes deben poder explicar cualquier módulo y realizar cambios sencillos. Las instrucciones prohíben consultar apuntes, internet o IA durante la defensa.
6. **Dependencias:** no se realizó en esta revisión una consulta en línea de vulnerabilidades/deprecaciones de paquetes; la compilación correcta no acredita esa condición.

## Git y continuidad

No se ejecutaron git add, commit, push, reset ni cambios de tag. Las correcciones quedan en el árbol de trabajo para que el usuario cree un commit local. No se requiere cerrar el proyecto ni congelar la entrega hoy. Los cambios de mañana pueden incorporarse con otro commit real antes de acordar la versión final.
