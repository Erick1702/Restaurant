namespace Restaurant.Models
{
    //Para la vista de asigancion de roles a los usuarios
    public class UsuariosRolesUsuarioViewModel
    {
        public string UsuarioId { get; set; }
        public string Email { get; set; }
        public IEnumerable<UsuarioRolViewModel> Roles { get; set; } = [];
    }
}
