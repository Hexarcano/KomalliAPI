using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Ordenes.Entities
{
    public class Orden
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ClienteId { get; set; }

        [Required]
        [Precision(2)]
        public double PrecioTotal { get; set; }

        [Required]
        public DateTime FechaExpedicion { get; set; }

        public DateTime? FechaPago { get; set; }

        [Required]
        public bool Pagado { get; set; }

        public string Comentario { get; set; }

        [Required]
        public List<ProductoOrden> ProductosOrdenados { get; }
    }
}
