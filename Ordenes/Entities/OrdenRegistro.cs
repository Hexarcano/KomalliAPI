using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Ordenes.Entities
{
    public class OrdenRegistro
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string NombreCliente { get; set; }
        public double PrecioTotal { get; set; }
        public bool Pagado { get; set; }
        public string Comentario { get; set; }
        public List<ProductoOrdenRegistro> Productos { get; set; }
    }
}
