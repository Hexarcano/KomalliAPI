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

namespace KomalliAPI.Ordenes.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenController : ControllerBase
    {
        private readonly KomalliContext _context;

        public OrdenController(KomalliContext context)
        {
            _context = context;
        }

        // GET: api/Orden
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orden>>> GetOrdenes()
        {
            return await _context.Ordenes.ToListAsync();
        }

        // GET: api/Orden/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Orden>> GetOrden(Guid id)
        {
            var orden = await _context.Ordenes.FindAsync(id);

            if (orden == null)
            {
                return NotFound();
            }

            return orden;
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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Orden orden = new Orden()
                    {
                        ClienteId = ordenRegistro.ClienteId,
                        PrecioTotal = ordenRegistro.PrecioTotal,
                        Pagado = ordenRegistro.Pagado
                    };

                    _context.Ordenes.Add(orden);

                    await _context.SaveChangesAsync();

                    List<ProductoOrden> productos = ordenRegistro.Productos;

                    double subtotal = 0;

                    foreach (var producto in productos)
                    {
                        subtotal = producto.PrecioUnitario * producto.Cantidad;

                        if (subtotal == producto.SubtotalProductos)
                        {
                            _context.ProductosOrden.Add(producto);

                            await _context.SaveChangesAsync();
                        }
                    }

                    return CreatedAtAction("GetOrden", new { id = orden.Id }, orden);
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
