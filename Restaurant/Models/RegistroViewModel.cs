using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Restaurant.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:250, MinimumLength =1, ErrorMessage ="La longuitud del {0}  campo entre {2} y {1}")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoP { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Apellido Materno")]
        public string ApellidoM { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos.")]
        //[MaxLength(8, ErrorMessage = "El DNI no puede tener más de 8 dígitos.")] //Permite solo ingresar  8 digitos
        [StringLength(maximumLength:8,MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")] // Permite solo ingresar 8 digitos
        public string Dni { get; set; }
        public string? Sexo { get; set; }
        public string? Direccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo {0} no es un correo valido")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
