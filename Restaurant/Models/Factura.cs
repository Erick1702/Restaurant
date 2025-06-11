namespace Restaurant.Models
{
    public class Factura
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ComandaId { get; set; }
        public Comanda? Comanda { get; set; }
        public decimal Total { get; set; }
    }
}
