using System.ComponentModel.DataAnnotations;

namespace tarea3_aplicaciones_web.Models;

public class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    [Required(ErrorMessage = "Ingrese los nombres")]
    [StringLength(80, ErrorMessage = "Los nombres no pueden pasar de 80 caracteres")]
    public string? Nombres { get; set; }

    [Required(ErrorMessage = "Ingrese los apellidos")]
    [StringLength(80, ErrorMessage = "Los apellidos no pueden pasar de 80 caracteres")]
    public string? Apellidos { get; set; }

    [Display(Name = "Cédula")]
    [Required(ErrorMessage = "Ingrese la cédula")]
    [StringLength(10, ErrorMessage = "La cédula tiene 10 dígitos")]
    public string? Cedula { get; set; }

    [Display(Name = "Correo electrónico")]
    [Required(ErrorMessage = "Ingrese el correo")]
    [EmailAddress(ErrorMessage = "Escriba un correo válido, por ejemplo juan@gmail.com")]
    [StringLength(120, ErrorMessage = "El correo es demasiado largo")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Ingrese el número de celular")]
    [StringLength(10, ErrorMessage = "El celular tiene 10 dígitos")]
    public string? Celular { get; set; }

    [Display(Name = "Fecha de nacimiento")]
    [Required(ErrorMessage = "Seleccione la fecha de nacimiento")]
    [DataType(DataType.Date)]
    public DateTime? FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Seleccione el sexo")]
    [StringLength(15)]
    public string? Sexo { get; set; }

    [Display(Name = "Dirección")]
    [Required(ErrorMessage = "Ingrese la dirección")]
    [StringLength(150, ErrorMessage = "La dirección no puede pasar de 150 caracteres")]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "Seleccione la ciudad")]
    [StringLength(50)]
    public string? Ciudad { get; set; }

    [Display(Name = "Cupo de crédito")]
    [Required(ErrorMessage = "Ingrese el cupo de crédito")]
    [Range(0, 5000, ErrorMessage = "El cupo de crédito debe estar entre 0 y 5000")]
    public int? CupoCredito { get; set; }
}
