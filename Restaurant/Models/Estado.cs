using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class Estado
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(250)]
        public string Nombre { get; set; }
        [StringLength(250)]
        public string? Tipo { get; set; }
        public bool Activo { get; set; } = true;

        public List<Mesa> Mesas { get; set; } = new List<Mesa>();
        public List<Comanda> Comandas { get; set; } = new List<Comanda>();
        public List<DetalleComanda> DetalleComandas { get; set; } = new List<DetalleComanda>();
    }
}
