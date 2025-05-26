namespace Restaurant.Models
{
    public class ComandaCreateViewModel
    {

        public int? MesaId { get; set; }
        public int? TipoConsumoId { get; set; }
        public int? EstadoId { get; set; } // Estado inicial de la comanda (Emitido, Cerrado, etc.)

        // Detalles de platos
        public List<DetalleComandaViewModel> Detalles { get; set; } = new List<DetalleComandaViewModel>();

        

    }

    public class DetalleComandaViewModel
    {
        public int? PlatoId { get; set; }
        public int Cantidad { get; set; }
    }
}
