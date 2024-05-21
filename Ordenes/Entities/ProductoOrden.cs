using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomalliAPI.Ordenes.Entities
{
    [PrimaryKey(nameof(ProductoId), nameof(OrdenId))]
    public class ProductoOrden
    {
        [Required]
        [ForeignKey("Orden")]
        public Guid OrdenId { get; set; }

        [Required]
        [ForeignKey("Producto")]
        public int ProductoId { get; set; }

        [Required]
        [Precision(2)]
        public double PrecioUnitario { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public double SubtotalProductos { get; set; }

        public Producto Producto { get; set; } = null;
        public Orden Orden { get; set; } = null;
    }
}
