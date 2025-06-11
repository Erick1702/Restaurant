namespace Restaurant.Models
{
    //ViewModel para listar usuarios
    public class UsuarioListadoViewModel
    {
        public IEnumerable<UsuarioViewModel> Usuarios { get; set; } = [];
        public string? Mensaje { get; set; }
    }
}
