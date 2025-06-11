using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class Usuario: IdentityUser
    {
        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; }
        //Agregando campos a la tabla usuario
        [Required]
        [MaxLength(150)]
        public string ApellidoP { get; set; }
        [Required]
        [MaxLength(150)]
        public string ApellidoM { get; set; }
        [Required]
        [MaxLength(8)]
        public string Dni { get; set; }
        [StringLength(1)]
        public string? Sexo { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Direccion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
