using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;
using System.Text.Json;

namespace Restaurant.Controllers
{
    public class UsuariosController: Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly ApplicationDbContext context;
        private readonly IConfiguration config;

        public UsuariosController(UserManager<Usuario> userManager,
                                  SignInManager<Usuario> signInManager,
                                  ApplicationDbContext context,
                                  IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.config = config;
        }

        public IActionResult Registro()
        {
            return View();
        }

        //Para creacion de nuevo usuarios
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new Usuario()
            {
                Email = modelo.Email,
                UserName = modelo.Email,
                Nombre = modelo.Nombre,
                ApellidoP = modelo.ApellidoP,
                ApellidoM = modelo.ApellidoM,
                Dni = modelo.Dni,
                Direccion = modelo.Direccion

            };

            var resultado=await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded) {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            } else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }

        //Incio de sesion
        [AllowAnonymous]
        public IActionResult Login(string? mensaje = null)
        {
            if (mensaje is not null)
            {
                ViewData["mensaje"]=mensaje;
            }

            return View();
        }

        //Para la vista de acceso denegado
        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        //Inicio de sesion
        //Valida datos de user y password y recpcta
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var token = Request.Form["g-recaptcha-response"];
            if (!await EsReCaptchaValido(token))
            {
                ModelState.AddModelError("", "Verificación reCAPTCHA fallida.");
                return View(modelo);
            }

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
                modelo.Password,modelo.Recuerdame, lockoutOnFailure: true);

            if (resultado.IsLockedOut)
            {

                ModelState.AddModelError(string.Empty, "Tu cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos. Intenta nuevamente más tarde.");
                return View(modelo);
            }

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrectos.");
                return View(modelo);
            }

            
            
        }

        //Cierre de sesion
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        //Listado de usuarios
        [HttpGet]
        [Authorize(Roles=Constantes.ROL_ADMINISTRADOR)]
        public async Task<IActionResult>Listado(string? mensaje=null)
        {
            var usuarios = await context.Users.Select(x => new UsuarioViewModel
            {
                Id = x.Id,
                Email = x.Email!, //Significa que no es null
                Nombre = x.Nombre,
                ApellidoP = x.ApellidoP,
                ApellidoM = x.ApellidoM,
                Dni = x.Dni,
                Direccion = x.Direccion,
                Sexo = x.Sexo
            }).ToListAsync();

            var modelo = new UsuarioListadoViewModel();
            
            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;
            return View(modelo);
        }

        //Muestra roles de usuarios
        [HttpGet]
        [Authorize(Roles=Constantes.ROL_ADMINISTRADOR)]
        public async Task<IActionResult> RolesUsuario(string usuarioId)
        {
            var usuario = await userManager.FindByIdAsync(usuarioId); // Buscar por Id

            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            //Roles del usuario
            var rolesQueElUsuarioTiene = await userManager.GetRolesAsync(usuario);
            //Roles existentes
            var rolesExistentes = await context.Roles.ToListAsync();
           
            var rolesDelUsuario = rolesExistentes.Select(x => new UsuarioRolViewModel
            {
                Nombre = x.Name!,
                TieneRol = rolesQueElUsuarioTiene.Contains(x.Name!)
            });

            var modelo = new UsuariosRolesUsuarioViewModel
            {
                UsuarioId = usuario.Id,
                Email = usuario.Email!,
                Roles = rolesDelUsuario.OrderBy(x => x.Nombre)
            };

            return View(modelo);
        }

        //Asignar nuevos roles
        [HttpPost]
        [Authorize(Roles=Constantes.ROL_ADMINISTRADOR)]
        public async Task<IActionResult>EditarRoles (EditarRolesViewModel modelo)
        {
            var usuario = await userManager.FindByIdAsync(modelo.UsuarioId);

            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await context.UserRoles.Where(x => x.UserId == usuario.Id).ExecuteDeleteAsync();

            await userManager.AddToRolesAsync(usuario, modelo.RolesSeleccionados);

            var mensaje = $"Los roles del {usuario.Email} han sido actualizados";
            return RedirectToAction("Listado", new { mensaje });
        }

        //valida recapcta
        private async Task<bool> EsReCaptchaValido(string token)
        {
            var secret = config.GetValue<string>("GoogleReCaptcha:SecretKey");
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}",
                null);

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(json).GetProperty("success").GetBoolean();
        }
    }
}
