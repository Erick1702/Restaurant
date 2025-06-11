namespace Restaurant.Models
{
    //ViewMOodel para detalle de item
    public class DetalleItemViewModel
    {
        public string Plato { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal {  get; set; }
    }
}
