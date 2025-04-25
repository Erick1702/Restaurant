using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class LoginViewModel
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public  required string Password { get; set; }
        public bool Recuerdame { get; set; }
    }
}
