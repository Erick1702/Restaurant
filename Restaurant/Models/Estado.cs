using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class Estado
    {
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }
        [StringLength(250)]
        public string? Tipo { get; set; }
        public bool Activo { get; set; } = true;

        public List<Mesa> Mesas { get; set; }
        public List<Comanda> Comandas { get; set; }
        public List<DetalleComanda> DetalleComandas { get; set; }
    }
}
