using Microsoft.AspNetCore.Identity;
using Restaurant.Models;
using System.Security.Claims;

namespace Restaurant.Servicios
{
    public interface IServicoUsuarios
    {
        string? ObtenerUsuarioId();
    }

    public class ServicoUsuarios : IServicoUsuarios
    {
        private readonly UserManager<Usuario> userManager;
        private readonly HttpContext httpContext;

        public ServicoUsuarios(IHttpContextAccessor httpContextAccessor,
            UserManager<Usuario> userManager)
        {
            this.userManager = userManager;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public string? ObtenerUsuarioId()
        {

            var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            if (idClaim is null)
            {
                return null;
            }
            else
            {
                return idClaim.Value;
            }

        }
    }
}
