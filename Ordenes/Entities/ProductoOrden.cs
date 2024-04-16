using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Ordenes.Entities
{
    public class ProductoOrden
    {
        [Key]
        [Required]
        public Guid OrdenId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Precision(2)]
        public double PrecioUnitario { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public double SubtotalProductos { get; set; }
    }
}
