using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{8}$",
        ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    public string Dni { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Inscripcion> Inscripciones { get; set; }
        = new List<Inscripcion>();

    [StringLength(9)]
    public string? Telefono { get; set; }
}