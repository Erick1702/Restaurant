using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Restaurant.Models
{
    public class RegistroViewModel
    {
        [Required]
        [StringLength(maximumLength:250, MinimumLength =10, ErrorMessage ="La longuitud del {0]  campo entre {2} y {1}")]
        public string Nombre { get; set; }
        //[Required]
        //[Display(Name ="Apellido Paterno")]
        //public string ApellidoP { get; set; }
        //[Required]
        //[Display(Name = "Apellido Materno")]
        //public  string ApellidoM { get; set; }
        //[Required]
        //public string Dni {  get; set; }
        //public string Sexo { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
