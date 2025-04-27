using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class Plato
    {
        public int Id { get; set; }
        public int TipoPlatoId { get; set; }
        public TipoPlato? TipoPlato { get; set; } // Relación con TipoPlato
        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }
        [StringLength(500)]
        public string? Descripcion { get; set; }
        [Required]
        public decimal Precio { get; set; }
        public bool Activo { get; set; } = true;

        public List<DetalleComanda> DetalleComandas { get; set; } = new List<DetalleComanda>();
    }
}
