using KomalliAPI.Clientes.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KomalliAPI.Clientes.Controller
{
    [Route("api/cliente")]
    [ApiController]
    public class ClienteAuthController : ControllerBase
    {
        private readonly UserManager<Cliente> userManager;
        private readonly SignInManager<Cliente> signInManager;

        public ClienteAuthController(UserManager<Cliente> userManager, SignInManager<Cliente> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(ClienteRegistro registro)
        {
            var usuario = User.Identity;
            if (usuario != null)
            {
                return BadRequest("No tienes permiso para realizar esta acción");
            }

            Cliente nuevoCLiente = new Cliente()
            {
                Nombre = registro.Nombre,
                ApellidoPaterno = registro.ApellidoPaterno,
                ApellidoMaterno = registro.ApellidoMaterno,
                UserName = registro.Usuario,
                PasswordHash = registro.Contrasenia,
                Email = registro.Email
            };

            var resultado = await userManager.CreateAsync(nuevoCLiente, nuevoCLiente.PasswordHash);

            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(nuevoCLiente, "Cliente");

                return Ok("Cliente registrado con éxito");
            }
            else
            {
                var listaErrores = resultado.Errors.ToList();

                string textoErrores = string.Empty;

                for (int i = 0; i < listaErrores.Count; i++)
                {
                    if (i == listaErrores.Count - 1)
                    {
                        textoErrores += listaErrores[i].Description.ToString();
                    }
                    else
                    {
                        textoErrores += listaErrores[i].Description.ToString() + '\n';
                    }
                }

                return BadRequest(textoErrores);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(ClienteLogin login)
        {
            var resultado = await signInManager.PasswordSignInAsync(
                userName: login.Usuario,
                password: login.Contrasenia,
                isPersistent: false,
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return Ok("Inicio de sesión completado");
            }
            else
            {
                return BadRequest("Datos incorrectos");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var usuario = User.Identity;

            if (usuario == null || !usuario.IsAuthenticated)
            {
                return BadRequest("No tienes permiso para realizar esta acción");
            }

            await signInManager.SignOutAsync();

            return Ok("Sesión Terminada con éxito");
        }
    }
}
