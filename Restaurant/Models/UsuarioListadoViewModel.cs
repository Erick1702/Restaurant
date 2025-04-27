namespace Restaurant.Models
{
    public class UsuarioListadoViewModel
    {
        public IEnumerable<UsuarioViewModel> Usuarios { get; set; } = [];
        public string? Mensaje { get; set; }
    }
}
