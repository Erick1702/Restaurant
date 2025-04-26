using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class TipoPlato
    {
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }
        [StringLength(500)]
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public List<Plato> Platos { get; set; }
    }
}
