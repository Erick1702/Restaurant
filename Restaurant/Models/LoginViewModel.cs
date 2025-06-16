using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Correo Electrónico")]
        public required string Email { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        public  required string Password { get; set; }
        public bool Recuerdame { get; set; }
    }
}
