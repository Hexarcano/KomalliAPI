using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Productos.Entities
{
    public class ProductoConsulta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int Descuento { get; set; }
        public int CategoriaProductoId { get; set; }
    }
}
