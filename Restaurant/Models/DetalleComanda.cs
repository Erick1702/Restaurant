namespace Restaurant.Models
{
    public class DetalleComanda
    {
        public int ComandaId { get; set; }
        public Comanda? Comanda { get; set; }
        public int? PlatoId { get; set; }
        public Plato? Plato { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public string? Sugerencia { get; set; }
        public int? EstadoId { get; set; }
        public Estado? Estado { get; set; }
    }
}
