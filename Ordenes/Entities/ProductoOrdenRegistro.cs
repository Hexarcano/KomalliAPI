using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomalliAPI.Ordenes.Entities
{
    public class ProductoOrdenRegistro
    {
        public int ProductoId { get; set; }
        public double PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public double SubtotalProductos { get; set; }
    }
}
