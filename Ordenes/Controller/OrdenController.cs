using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KomalliAPI.Contexts;
using KomalliAPI.Ordenes.Entities;
using KomalliAPI.Productos.Entities;
using Microsoft.AspNetCore.Authorization;
using KomalliAPI.CategoriasProducto.Entities;
using KomalliAPI.Clientes.Utils;
using NuGet.Common;
using KomalliAPI.Clientes.Entities;
using KomalliAPI.Clientes.Controller;

namespace KomalliAPI.Ordenes.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenController : ControllerBase
    {
        private readonly KomalliContext _context;
        private readonly ITokenService _tokenService;

        public OrdenController(KomalliContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // GET: api/Orden
        [HttpGet]
        public async Task<ActionResult<OrdenResponse>> GetOrdenes()
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "Ordenes encontradas";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }

            List<Orden> lista = await _context.Ordenes.ToListAsync();

            if (lista.Count > 0)
            {
                List<OrdenConsulta> ordenes = new List<OrdenConsulta>();

                foreach (var item in lista)
                {
                    var orden = new OrdenConsulta()
                    {
                        Id = item.Id,
                        ClienteId = item.ClienteId,
                        PrecioTotal = item.PrecioTotal,
                        FechaExpedicion = item.FechaExpedicion,
                        FechaPago = item.FechaPago,
                        Pagado = item.Pagado,
                        Comentario = item.Comentario,
                        NombreCliente = item.NombreCliente
                    };

                    ordenes.Add(orden);
                }

                return Ok(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = ordenes,
                    Productos = null
                });
            }
            else
            {
                mensaje = "Ordenes no encontradas";

                var resultado = new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                };

                return Ok(resultado);
            }

        }

        // GET: api/Orden/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenResponse>> GetOrden(Guid id)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }

            Orden? orden = await _context.Ordenes.FindAsync(id);

            if (orden == null)
            {
                mensaje = "No encontrada";

                return NotFound(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }

            List<OrdenConsulta> ordenes = new List<OrdenConsulta>()
            {
                new OrdenConsulta()
                {
                    Id = orden.Id,
                    ClienteId = orden.ClienteId,
                    NombreCliente = orden.NombreCliente,
                    PrecioTotal = orden.PrecioTotal,
                    FechaExpedicion = orden.FechaExpedicion,
                    FechaPago = orden.FechaPago,
                    Pagado = orden.Pagado,
                    Comentario = orden.Comentario
                }
            };

            List<ProductoOrden> productosOrden = await _context.ProductosOrden
                .Where(po => po.OrdenId == orden.Id)
                .ToListAsync();

            List<ProductoOrdenConsulta> productos = new List<ProductoOrdenConsulta>();

            foreach (var item in productosOrden)
            {

                productos.Add(new ProductoOrdenConsulta()
                {
                    OrdenId = item.OrdenId,
                    ProductoId = item.ProductoId,
                    PrecioUnitario = item.PrecioUnitario,
                    Cantidad = item.Cantidad,
                    SubtotalProductos = item.SubtotalProductos,
                    Producto = item.Producto,
                });
            }

            mensaje = "Ordenes encontradas";

            return Ok(new OrdenResponse()
            {
                Mensaje = mensaje,
                Ordenes = ordenes,
                Productos = productos
            });
        }

        // PUT: api/Orden/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<OrdenResponse>> PutOrden(Guid id, OrdenRegistro ordenNueva)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }

            if (id != ordenNueva.Id)
            {
                mensaje = "El id no coincide con la orden";

                return BadRequest(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }

            var ordenExistente = await _context.Ordenes.Include(o => o.ProductosOrdenados).FirstOrDefaultAsync(o => o.Id == id);

            if (ordenExistente == null)
            {
                return NotFound(new OrdenResponse()
                {
                    Mensaje = "Orden no encontrada",
                    Ordenes = null,
                    Productos = null
                });
            }

            ordenExistente.NombreCliente = ordenNueva.NombreCliente;
            ordenExistente.PrecioTotal = ordenNueva.PrecioTotal;
            ordenExistente.Pagado = ordenNueva.Pagado;
            ordenExistente.Comentario = ordenNueva.Comentario;
            ordenExistente.FechaExpedicion = DateTime.Now;
            ordenExistente.FechaPago = ordenNueva.Pagado ? DateTime.Now : (DateTime?)null;

            var productosAnteriores = ordenExistente.ProductosOrdenados.ToList();
            var productosNuevos = ordenNueva.Productos;

            foreach (var item in productosAnteriores)
            {
                if (!productosNuevos.Any(p => p.ProductoId == item.ProductoId))
                {
                    _context.ProductosOrden.Remove(item);
                }
            }

            _context.Entry(ordenExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                mensaje = "Orden actualizada";

                return Ok(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenExists(id))
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

        // POST: api/Orden
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrdenResponse>> PostOrden(OrdenRegistro ordenRegistro)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Orden orden = new Orden();

                    if (ordenRegistro.Pagado)
                    {
                        orden.NombreCliente = ordenRegistro.NombreCliente;
                        orden.PrecioTotal = ordenRegistro.PrecioTotal;
                        orden.Pagado = ordenRegistro.Pagado;
                        orden.FechaExpedicion = DateTime.Now;
                        orden.FechaPago = DateTime.Now;
                        orden.Comentario = ordenRegistro.Comentario;
                    }
                    else
                    {
                        orden.NombreCliente = ordenRegistro.NombreCliente;
                        orden.PrecioTotal = ordenRegistro.PrecioTotal;
                        orden.Pagado = false;
                        orden.FechaExpedicion = DateTime.Now;
                        orden.FechaPago = null;
                        orden.Comentario = ordenRegistro.Comentario;
                    }

                    _context.Ordenes.Add(orden);

                    await _context.SaveChangesAsync();

                    List<ProductoOrdenRegistro> productos = ordenRegistro.Productos;

                    double subtotal = 0;

                    foreach (var producto in productos)
                    {
                        subtotal = producto.Cantidad * producto.PrecioUnitario;

                        _context.ProductosOrden.Add(new ProductoOrden()
                        {
                            OrdenId = orden.Id,
                            ProductoId = producto.ProductoId,
                            PrecioUnitario = producto.PrecioUnitario,
                            Cantidad = producto.Cantidad,
                            SubtotalProductos = subtotal
                        });

                        await _context.SaveChangesAsync();
                    }

                    transaction.Commit();

                    mensaje = "Orden creada con éxito";

                    return Ok(new OrdenResponse()
                    {
                        Mensaje = mensaje,
                        Ordenes = null,
                        Productos = null
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    transaction.Rollback();

                    mensaje = "Error de conexión con la base de datos";

                    return BadRequest(new OrdenResponse()
                    {
                        Mensaje = "",
                        Ordenes = null,
                        Productos = null
                    });
                }
            }
        }

        // DELETE: api/Orden/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrden(Guid id)
        {
            string? token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            string mensaje = "";

            if (!Autorizador.TieneToken(token) || !Autorizador.EsTokenValido(_tokenService, token))
            {
                mensaje = "No tienes permiso para hacer esta acción";

                return BadRequest(new OrdenResponse()
                {
                    Mensaje = mensaje,
                    Ordenes = null,
                    Productos = null
                });
            }

            var orden = await _context.Ordenes.FindAsync(id);
            if (orden == null)
            {
                return NotFound();
            }

            _context.Ordenes.Remove(orden);
            await _context.SaveChangesAsync();

            mensaje = "Orden eliminada con éxito";

            return Ok(new OrdenResponse()
            {
                Mensaje = mensaje,
                Ordenes = null,
                Productos = null
            });
        }

        private bool OrdenExists(Guid id)
        {
            return _context.Ordenes.Any(e => e.Id == id);
        }
    }
}
