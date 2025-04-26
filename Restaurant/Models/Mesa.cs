using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models
{
    public class Mesa
    {
        public int Id { get; set; }
        [Required]
        [StringLength(5)]
        public string Numero { get; set; }
        public int? Capacidad { get; set; }
        [StringLength(100)]
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public int? EstadoId { get; set; }
        public Estado? Estado { get; set; }

        public List<Comanda> Comandas { get; set; }
    }
}
