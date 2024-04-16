using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Ordenes.Entities
{
    public class OrdenRegistro
    {
        [Required]
        public Guid ClienteId { get; set; }

        [Required]
        [Precision(2)]
        public double PrecioTotal { get; set; }

        [Required]
        public bool Pagado { get; set; }

        [Required]
        public List<ProductoOrden> Productos { get; set; }
    }
}
