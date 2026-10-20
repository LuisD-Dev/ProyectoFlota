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
