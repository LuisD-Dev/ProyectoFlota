using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MantenimientoFlota.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class CodigoActivoAttribute : ValidationAttribute
{
    public CodigoActivoAttribute()
        : base("El código debe comenzar con VEH-, incluir opcionalmente segmentos de letras mayúsculas separados por guiones y terminar en tres dígitos. Ejemplos: VEH-001 y VEH-QA-BAJA-001.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null || value is string texto && string.IsNullOrWhiteSpace(texto))
        {
            return true;
        }

        return value is string codigo
            && Regex.IsMatch(codigo.Trim(), @"\AVEH-(?:[A-Z]+-)*[0-9]{3}\z", RegexOptions.CultureInvariant);
    }
}
