using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Ordenes.Entities
{
    public class OrdenRegistro
    {
        public Guid ClienteId { get; set; }
        public double PrecioTotal { get; set; }
        public bool Pagado { get; set; }
        public string Comentario { get; set; }
        public List<ProductoOrdenRegistro> Productos { get; set; }
    }
}
