namespace Restaurant.Models
{
    //ViewModel para asignar Roles
    public class EditarRolesViewModel
    {
        public required string UsuarioId { get; set; }
        public List<string> RolesSeleccionados { get; set; } = [];
    }
}
