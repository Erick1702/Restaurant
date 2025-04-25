using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class Usuario: IdentityUser
    {
        [Required]
        public string Nombre { get; set; }
    }
}
