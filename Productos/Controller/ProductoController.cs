using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KomalliAPI.Contexts;
using KomalliAPI.Productos.Entities;
using KomalliAPI.Clientes.Entities;
using KomalliAPI.Clientes.Utils;
using NuGet.Common;
using Microsoft.AspNetCore.Authorization;

namespace KomalliAPI.Productos
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly KomalliContext _context;
        private readonly ITokenService _tokenService;

        public ProductoController(KomalliContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoResponse>>> GetProductos()
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Productos encontrados";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new ProductoResponse()
                {
                    Mensaje = mensaje,
                    Productos = null
                });
            }

            List<ProductoConsulta> productos = await _context.Productos.Select(c => new ProductoConsulta()
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Precio = c.Precio,
                PorcentajeDescuento = c.PorcentajeDescuento,
                CategoriaProductoId = c.CategoriaProductoId
            }).ToListAsync();

            return Ok(new ProductoResponse()
            {
                Mensaje = mensaje,
                Productos = productos
            });
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoResponse>> GetProducto(int id)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Producto encontrado";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new ProductoResponse()
                {
                    Mensaje = mensaje,
                    Productos = null
                });
            }

            var consulta = await _context.Productos.FindAsync(id);

            if (consulta == null)
            {
                mensaje = "Producto no encontrado";

                return NotFound(new ProductoResponse()
                {
                    Mensaje = mensaje,
                    Productos = null
                });
            }

            List<ProductoConsulta> producto = new List<ProductoConsulta>()
            {
                new ProductoConsulta()
                {
                    Id = consulta.Id,
                    Nombre = consulta.Nombre,
                    Precio = consulta.Precio,
                    PorcentajeDescuento = consulta.PorcentajeDescuento,
                    CategoriaProductoId = consulta.CategoriaProductoId
                }
            };

            return Ok(new ProductoResponse()
            {
                Mensaje = mensaje,
                Productos = producto
            });
        }

        // GET: api/Productos/Categoria/5
        [HttpGet("Categoria/{categoriaId}")]
        public async Task<ActionResult<ProductoResponse>> GetProductosPorCategoria(int categoriaId)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Producto encontrado";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new ProductoResponse()
                {
                    Mensaje = mensaje,
                    Productos = null
                });
            }

            List<ProductoConsulta> productos = await _context.Productos
                .Where(c => c.CategoriaProductoId == categoriaId)
                .Select(c => new ProductoConsulta()
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Precio = c.Precio,
                    PorcentajeDescuento = c.PorcentajeDescuento,
                    CategoriaProductoId = c.CategoriaProductoId
                })
                .ToListAsync();

            if (productos == null)
            {
                mensaje = "Producto no encontrado";
            }

            return Ok(new ProductoResponse()
            {
                Mensaje = mensaje,
                Productos = productos
            });
        }

        // PUT: api/Productos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoConsulta productoConsulta)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Productos encontrados";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new ProductoResponse()
                {
                    Mensaje = mensaje,
                    Productos = null
                });
            }

            if (id != productoConsulta.Id)
            {
                return BadRequest();
            }

            Producto producto = new Producto()
            {
                Id = productoConsulta.Id,
                Nombre = productoConsulta.Nombre,
                Precio = productoConsulta.Precio,
                PorcentajeDescuento = productoConsulta.PorcentajeDescuento,
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
        public async Task<ActionResult<ProductoResponse>> PostProducto(ProductoRegistro producto)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Producto encontrado";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new ProductoResponse()
                {
                    Mensaje = mensaje,
                    Productos = null
                });
            }

            Producto nuevoProducto = new Producto()
            {
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                PorcentajeDescuento = producto.PorcentajeDescuento,
                CategoriaProductoId = producto.CategoriaProductoId
            };

            _context.Productos.Add(nuevoProducto);

            var resultado = await _context.SaveChangesAsync();

            if (resultado < 1)
            {
                mensaje = "No existe producto de esa categoría";

                return BadRequest(new ProductoResponse()
                {
                    Mensaje = mensaje,
                    Productos = null
                });
            }

            mensaje = "Producto agregado";

            List<ProductoConsulta> productos = new List<ProductoConsulta>();

            productos.Add(new ProductoConsulta()
            {
                Id = nuevoProducto.Id,
                Nombre = nuevoProducto.Nombre,
                Precio = nuevoProducto.Precio,
                PorcentajeDescuento = nuevoProducto.PorcentajeDescuento,
                CategoriaProductoId = nuevoProducto.CategoriaProductoId
            });

            return Ok(new ProductoResponse()
            {
                Mensaje = mensaje,
                Productos = productos
            });
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
