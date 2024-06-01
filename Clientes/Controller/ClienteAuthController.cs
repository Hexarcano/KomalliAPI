using KomalliAPI.Clientes.Entities;
using KomalliAPI.Clientes.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KomalliAPI.Clientes.Controller
{
    [Route("api/cliente")]
    [ApiController]
    public class ClienteAuthController : ControllerBase
    {
        private const string KEY = "LaContrasenaMasSeguraDelMundo12345"; //No debería estar a la vista, sino resguardada

        private readonly UserManager<Cliente> userManager;
        private readonly SignInManager<Cliente> signInManager;
        private readonly ITokenService tokenService;

        public ClienteAuthController(UserManager<Cliente> userManager, SignInManager<Cliente> signInManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(ClienteRegistroRequest registro)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            ClienteAuthResponse respuesta = null!;

            if (token != null)
            {
                respuesta = PrepararRespuesta(
                    null,
                    null,
                    "No tienes permiso para esta acción");

                return BadRequest(respuesta);
            }

            try
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

                if (!resultado.Succeeded)
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

                    respuesta = PrepararRespuesta(
                    null,
                    null,
                    $"Errores: {textoErrores}");

                    return BadRequest(textoErrores);
                }

                DatosSesion datosSesion = new DatosSesion(
                    nuevoCLiente.Id,
                    nuevoCLiente.Nombre,
                    nuevoCLiente.ApellidoPaterno,
                    nuevoCLiente.ApellidoMaterno,
                    nuevoCLiente.UserName,
                    nuevoCLiente.Email);

                token = GenerarToken(datosSesion);

                respuesta = PrepararRespuesta(
                "Bearer",
                token,
                $"Bienvenido {nuevoCLiente.Nombre}");

                return Ok(respuesta);
            }
            catch (InvalidOperationException ex)
            {
                respuesta = PrepararRespuesta(
                    null,
                    null,
                    "No se pudo acceder a la base de datos");

                return BadRequest(respuesta);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(ClienteLoginRequest login)
        {
            var respuesta = new ClienteAuthResponse();

            try
            {
                var resultado = await signInManager.PasswordSignInAsync(
                userName: login.Usuario,
                password: login.Contrasenia,
                isPersistent: false,
                lockoutOnFailure: false);

                if (resultado.Succeeded == false)
                {
                    respuesta = PrepararRespuesta(
                    null,
                    null,
                    "Usuario o contraseña erroneos");

                    return BadRequest(respuesta);
                }

                var usuario = await userManager.FindByNameAsync(login.Usuario);

                var datosSesion = new DatosSesion(
                    usuario.Id,
                    usuario.Nombre,
                    usuario.ApellidoPaterno,
                    usuario.ApellidoMaterno,
                    usuario.UserName,
                    usuario.Email);

                string token = GenerarToken(datosSesion);

                //var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                respuesta = PrepararRespuesta(
                    "Bearer",
                    token,
                    $"Bienvenido {usuario.Nombre}");

                return Ok(respuesta);

            }
            catch (InvalidOperationException ex)
            {
                respuesta = PrepararRespuesta(
                    null,
                    null,
                    "No se pudo acceder a la base de datos");

                return BadRequest(respuesta);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var respuesta = new ClienteAuthResponse();

            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

            if(!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(tokenService, token))
            {
                respuesta = PrepararRespuesta(
                    null,
                    null,
                    "No tienes permiso para esta acción");

                return Ok(respuesta);
            }

            Autorizador.RevocarToken(tokenService, token);

            respuesta = PrepararRespuesta(
                null,
                null,
                "Sesión Terminada");

            return Ok(respuesta);
        }

        private string GenerarToken(DatosSesion datosSesion)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var nombre = datosSesion.nombre + " " + datosSesion.apellidoP + " " + datosSesion.apellidoM;

            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, datosSesion.id.ToString()),
                new Claim(ClaimTypes.Name, datosSesion.nombre),
                new Claim(ClaimTypes.Email, datosSesion.email)
            };

            var token = new JwtSecurityToken(
                issuer: "https://0.0.0.0:7132",
                audience: "https://0.0.0.0:7132",
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClienteAuthResponse PrepararRespuesta(string tokenType, string accessToken, string mensaje)
        {
            ClienteAuthResponse respuesta = new ClienteAuthResponse();

            respuesta.TokenType = tokenType;
            respuesta.AccessToken = accessToken;
            respuesta.Mensaje = mensaje;

            return respuesta;
        }
    }
}
