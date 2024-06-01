using KomalliAPI.CategoriasProducto.Entities;
using KomalliAPI.Clientes.Entities;
using KomalliAPI.Clientes.Utils;
using KomalliAPI.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KomalliAPI.CategoriasProducto.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaProductoController : ControllerBase
    {
        private readonly KomalliContext _context;
        private readonly ITokenService _tokenService;

        public CategoriaProductoController(KomalliContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> PostCategoria(CategoriaProductoRegistro categoriaProducto)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new CategoriaProductoResponse()
                {
                    Mensaje = mensaje,
                    Categorias = null
                });
            }

            var existe = await _context.CategoriasProducto
                .FirstOrDefaultAsync(c => c.Nombre == categoriaProducto.Nombre);

            if (existe != null)
            {
                return BadRequest("Ya existe esa categoría");
            }

            CategoriaProducto nuevaCategoria = new CategoriaProducto()
            {
                Nombre = categoriaProducto.Nombre,
                ImagenBase64 = categoriaProducto.ImagenBase64
            };

            _context.Add(nuevaCategoria);

            var resultado = await _context.SaveChangesAsync();

            if (resultado < 1)
            {
                mensaje = "Verificar datos de categoría";

                return BadRequest(new CategoriaProductoResponse()
                {
                    Mensaje = mensaje,
                    Categorias = null
                });
            }

            List<CategoriaProductoConsulta> categorias = new List<CategoriaProductoConsulta> {
                new CategoriaProductoConsulta() {
                    Id = nuevaCategoria.Id,
                    Nombre = nuevaCategoria.Nombre,
                    ImagenBase64 = nuevaCategoria.ImagenBase64
                }
            };

            CategoriaProductoResponse respuesta = new CategoriaProductoResponse()
            {
                Mensaje = mensaje,
                Categorias = categorias
            };

            return Ok(respuesta);
        }

        [HttpGet]
        public async Task<ActionResult<CategoriaProductoResponse>> GetCategorias()
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Categorias encontradas";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new CategoriaProductoResponse()
                {
                    Mensaje = mensaje,
                    Categorias = null
                });
            }

            List<CategoriaProductoConsulta> categorias = await _context.CategoriasProducto.Select(c =>
                new CategoriaProductoConsulta
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    ImagenBase64 = c.ImagenBase64
                }).ToListAsync();

            if (categorias == null)
            {
                mensaje = "No se encontraron categorias";
            }

            CategoriaProductoResponse respuesta = new CategoriaProductoResponse()
            {
                Mensaje = mensaje,
                Categorias = categorias
            };

            return Ok(respuesta);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaProductoResponse>> GetCategoria(int id)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Categorias encontradas";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new CategoriaProductoResponse()
                {
                    Mensaje = mensaje,
                    Categorias = null
                });
            }

            var categoria = await _context.CategoriasProducto.FindAsync(id);

            if (categoria == null)
            {
                mensaje = "No se encontraron categorias";


                return NotFound(new CategoriaProductoResponse()
                {
                    Mensaje = mensaje,
                    Categorias = null
                });
            }

            CategoriaProductoConsulta categoriaConsulta = new CategoriaProductoConsulta()
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                ImagenBase64 = categoria.ImagenBase64
            };

            List<CategoriaProductoConsulta> categorias = new List<CategoriaProductoConsulta>() { categoriaConsulta };

            CategoriaProductoResponse respuesta = new CategoriaProductoResponse()
            {
                Mensaje = mensaje,
                Categorias = categorias
            };

            return Ok(respuesta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaProductoConsulta categoriaProducto)
        {
            if (id != categoriaProducto.Id)
            {
                return BadRequest("El id no coincide con la categoria");
            }

            var categoria = _context.CategoriasProducto.FirstOrDefaultAsync(c => c.Id == categoriaProducto.Id).Result;

            if (categoria == null)
            {
                return BadRequest("Categoria inexistente");
            }

            if (categoria.ImagenBase64 != categoriaProducto.ImagenBase64)
            {
                categoria.ImagenBase64 = categoriaProducto.ImagenBase64;
            }

            _context.Entry(categoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CategoriaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.CategoriasProducto.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            _context.CategoriasProducto.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriaExists(int id)
        {
            return _context.CategoriasProducto.Any(c => c.Id == id);
        }
    }
}
