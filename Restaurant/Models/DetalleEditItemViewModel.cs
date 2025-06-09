namespace Restaurant.Models
{
    public class DetalleEditItemViewModel
    {

        public int? PlatoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; } // mostrar el precio en el formulario
    }
}
