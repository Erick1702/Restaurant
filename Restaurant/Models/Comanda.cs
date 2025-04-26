namespace Restaurant.Models
{
    public class Comanda
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int? MesaId { get; set; }
        public Mesa? Mesa { get; set; }
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int? TipoConsumoId { get; set; }
        public TipoConsumo? TipoConsumo { get; set; }
        public List<DetalleComanda> DetalleComandas { get; set; }
        public Factura? Factura { get; set; }
    }
}
