namespace Restaurant.Models
{
    public class ComandaEditViewModel
    {
        public int Id { get; set; }
        public int? MesaId { get; set; }
        public int? TipoConsumoId { get; set; }
        public List<DetalleEditItemViewModel> Detalles { get; set; } = new List<DetalleEditItemViewModel>();
    }
}

