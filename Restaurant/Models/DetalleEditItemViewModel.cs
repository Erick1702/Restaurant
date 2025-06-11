namespace Restaurant.Models
{
    //ViewModel para la edicion de items
    public class DetalleEditItemViewModel
    {
        public int? PlatoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; } // mostrar el precio en el formulario
    }
}
