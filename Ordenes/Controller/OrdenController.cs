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
            } else
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
                    PrecioTotal = orden.PrecioTotal,
                    FechaExpedicion = orden.FechaExpedicion,
                    FechaPago = orden.FechaPago,
                    Pagado = orden.Pagado,
                    Comentario = orden.Comentario
                }
            };

            List<ProductoOrdenConsulta> productos = new List<ProductoOrdenConsulta>();

            foreach (var item in orden.ProductosOrdenados)
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
        public async Task<IActionResult> PutOrden(Guid id, Orden orden)
        {
            if (id != orden.Id)
            {
                return BadRequest();
            }

            _context.Entry(orden).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
        public async Task<ActionResult<Orden>> PostOrden(OrdenRegistro ordenRegistro)
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
                    Orden orden = new Orden()
                    {
                        ClienteId = ordenRegistro.ClienteId,
                        PrecioTotal = ordenRegistro.PrecioTotal,
                        Pagado = false,
                        FechaExpedicion = DateTime.Now,
                        FechaPago = null,
                        Comentario = ordenRegistro.Comentario
                    };

                    _context.Ordenes.Add(orden);

                    var ordenCreada = await _context.SaveChangesAsync();

                    if (ordenCreada > 0)
                    {
                        List<ProductoOrdenRegistro> productos = ordenRegistro.Productos;

                        double subtotal = 0;

                        foreach (var producto in productos)
                        {
                            subtotal = producto.PrecioUnitario * producto.Cantidad;

                            if (subtotal == producto.SubtotalProductos)
                            {
                                _context.ProductosOrden.Add(new ProductoOrden()
                                {
                                    ProductoId = producto.ProductoId,
                                    PrecioUnitario = producto.PrecioUnitario,
                                    Cantidad = producto.Cantidad,
                                    SubtotalProductos = subtotal
                                });

                                await _context.SaveChangesAsync();

                                return Ok(new OrdenResponse()
                                {
                                    Mensaje = ""
                                });
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }

                        return CreatedAtAction("GetOrden", new { id = orden.Id }, orden);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    transaction.Rollback();

                    return BadRequest();
                }
            }
        }

        // DELETE: api/Orden/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrden(Guid id)
        {
            var orden = await _context.Ordenes.FindAsync(id);
            if (orden == null)
            {
                return NotFound();
            }

            _context.Ordenes.Remove(orden);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrdenExists(Guid id)
        {
            return _context.Ordenes.Any(e => e.Id == id);
        }
    }
}
