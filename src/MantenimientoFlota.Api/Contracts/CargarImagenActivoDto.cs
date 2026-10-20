using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Api.Contracts;

public sealed class CargarImagenActivoDto
{
    [Required(ErrorMessage = "El archivo es obligatorio.")]
    [Description("Imagen JPEG o PNG, no vacía, máximo 2 MiB. Se comprueban extensión y firma; no se realiza decodificación completa.")]
    public IFormFile? Archivo { get; set; }
}
