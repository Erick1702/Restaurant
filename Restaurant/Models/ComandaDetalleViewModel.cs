namespace Restaurant.Models
{
    public class ComandaDetalleViewModel
    {

        public int ComandaId { get; set; }
        public DateTime Fecha { get; set; }
        public string? Mesa { get; set; }
        public string? Usuario { get; set; }
        public string? TipoConsumo { get; set; }
        //public int? Estado { get; set; }
        public List<DetalleItemViewModel> Detalles { get; set; } = new();
        public decimal Total => Detalles.Sum(d => d.Subtotal);
    }
}
