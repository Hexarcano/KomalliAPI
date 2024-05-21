using KomalliAPI.CategoriasProducto.Entities;
using KomalliAPI.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KomalliAPI.CategoriasProducto.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaProductoController : ControllerBase
    {
        private readonly KomalliContext _context;

        public CategoriaProductoController(KomalliContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostCategoria(CategoriaProductoRegistro categoriaProducto)
        {
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
                return BadRequest("Verificar datos de categoría");
            }

            return Ok("Categoria creada con éxito");
        }

        [HttpGet]
        public async Task<ActionResult<CategoriaProductoResponse>> GetCategorias()
        {
            List<CategoriaProductoConsulta> categorias = await _context.CategoriasProducto.Select(c => 
                new CategoriaProductoConsulta
                {
                    Id = c.Id, 
                    Nombre = c.Nombre,
                    ImagenBase64 = c.ImagenBase64
                }).ToListAsync();

            string mensaje = "Categorias encontradas";

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
        public async Task<ActionResult<CategoriaProductoConsulta>> GetCategoria(int id)
        {
            var categoria = await _context.CategoriasProducto.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            var categoriaConsulta = new CategoriaProductoConsulta()
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                ImagenBase64 = categoria.ImagenBase64
            };

            return categoriaConsulta;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaProductoConsulta categoriaProducto)
        {
            if (id != categoriaProducto.Id)
            {
                return BadRequest("El id no coincide con la categoria");
            }

            var categoria = _context.CategoriasProducto.FirstOrDefaultAsync(c => c.Id == categoriaProducto.Id).Result;

            if(categoria == null) {
                return BadRequest("Categoria inexistente");
            }

            if(categoria.ImagenBase64 != categoriaProducto.ImagenBase64)
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
