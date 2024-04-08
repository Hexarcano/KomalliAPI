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

        public ClienteAuthController(UserManager<Cliente> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(ClienteRegistro registro)
        {
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
    }
}
