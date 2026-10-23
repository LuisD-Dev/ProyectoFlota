# Revisión final del Avance 1

Fecha: 23 de octubre de 2026. Equipo 7: Luis Diego Campos Murillo y Oscar Rojas Garcia.

## Alcance verificado

Se contrastaron los 98 rubros A1–G11 de la tabla con el contrato mínimo del Avance 1 y las instrucciones de entrega. Se conserva el alcance de solicitudes de mantenimiento; no se agrega el flujo del Avance 2. Las asociaciones con atributos permanecen anidadas en Técnico y Repuesto. Las excepciones propias se conservan para A5.

La documentación OpenAPI expone 47 operaciones, con esquemas Result, XML, ejemplos y ausencia del logger entre los parámetros públicos. PUT/PATCH/DELETE exitosos responden 200 con Result, según la decisión de seguir el patrón de clase. POST conserva 201 y Location resoluble. Se agregaron búsquedas paginadas de Activo y Repuesto sin reemplazar los listados del contrato mínimo.

## Verificaciones ejecutadas

- Compilación de la solución sin errores ni advertencias; revisión de espacios del diff correcta.
- 550 comprobaciones HTTP sobre una base SQL Server nueva y exclusiva de QA: 207 generales, 125 de validación y regresión, 127 de relaciones anidadas y 91 de búsqueda, ordenación y paginación. Todas correctas.
- La API real creó la base, aplicó las seis migraciones y ejecutó la inicialización automática. El reinicio conservó los conteos y checksums de las tablas consultadas, sin duplicar el seeder ni cambiar contraseñas existentes.
- 23 comprobaciones aisladas de seeders y administrador: carga representativa, hash, secreto requerido, callbacks e idempotencia.
- 49 comprobaciones de estados, JSON y modelo EF: sin enums en los contratos ni cambios pendientes de modelo frente al snapshot.
- 20 comprobaciones aisladas de excepciones y logging: niveles, respuesta consistente, un cierre por solicitud, duración, protección de mensajes internos y archivo de log.
- Cuatro consultas de detalle traducidas con el proveedor SQL Server conservan COUNT en SQL y entregan entidades al servicio para el mapeo.
- Correlación comprobada en la API real: un 404 contiene el mismo traceId del cierre Warning y un GET 200 registra cierre Information con duración.

Las pruebas HTTP modificaron exclusivamente datos de la base QA. Un primer arranque alcanzó la base habitual antes de corregir la selección de conexión; se detuvo antes de enviar pruebas HTTP. Las pruebas aisladas de errores técnicos usan excepciones simuladas y no provocan caídas del servidor SQL.

## Correspondencia de la entrega

El ZIP debe exportarse desde el commit final y excluir bin, obj, .git, logs y secretos de usuario. La tabla debe indicar ese mismo hash. El tag avance-1 debe señalar la misma versión en GitLab y GitHub.

El video anterior ya no demuestra el contrato vigente. Grabar la nueva versión, mostrar todos los casos del contrato mínimo en el orden exigido y completar el enlace, la duración y los minutos reales de la tabla. No sustituirlos por tiempos estimados del guion. Comprobar el acceso y audio en incógnito antes de enviar.

La revisión del historial encontró 59 commits previos atribuidos a Luis Diego y ninguno a Oscar. La sección 14 exige contribución individual verificable; el estado funcional del código no corrige este requisito. No se modificaron autores ni fechas históricas.
