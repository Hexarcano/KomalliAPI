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
                Nombre = categoriaProducto.Nombre
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
        public async Task<ActionResult<IEnumerable<CategoriaProductoConsulta>>> GetCategorias()
        {
            var categorias = await _context.CategoriasProducto.Select(c => new { c.Id, c.Nombre } ).ToListAsync();

            return Ok(categorias);
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
                Nombre = categoria.Nombre
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

            CategoriaProducto categoria = new CategoriaProducto()
            {
                Id = categoriaProducto.Id,
                Nombre = categoriaProducto.Nombre
            };

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
