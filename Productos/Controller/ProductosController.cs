using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KomalliAPI.Contexts;
using KomalliAPI.Productos.Entities;

namespace KomalliAPI.Productos
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly KomalliContext _context;

        public ProductosController(KomalliContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoConsulta>>> GetProductos()
        {
            var consulta = await _context.Productos.Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Precio,
                c.Descuento,
                c.CategoriaProductoId
            }).ToListAsync();

            return Ok(consulta);
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // PUT: api/Productos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoConsulta productoConsulta)
        {
            if (id != productoConsulta.Id)
            {
                return BadRequest();
            }

            Producto producto = new Producto()
            {
                Id = productoConsulta.Id,
                Nombre = productoConsulta.Nombre,
                Precio = productoConsulta.Precio,
                Descuento = productoConsulta.Descuento,
                CategoriaProductoId = productoConsulta.CategoriaProductoId
            };

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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

        // POST: api/Productos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(ProductoRegistro producto)
        {
            Producto nuevoProducto = new Producto()
            {
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Descuento = producto.Descuento,
                CategoriaProductoId = producto.CategoriaProductoId
            };

            _context.Productos.Add(nuevoProducto);

            var resultado = await _context.SaveChangesAsync();

            if (resultado < 1)
            {
                return BadRequest("Verificar datos de producto");
            }

            return Ok("Producto creado con éxito");
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
