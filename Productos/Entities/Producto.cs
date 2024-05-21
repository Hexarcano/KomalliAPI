using KomalliAPI.CategoriasProducto.Entities;
using KomalliAPI.Ordenes.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomalliAPI.Productos.Entities
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [Precision(2)]
        public double Precio { get; set; }

        [Required]
        public int Descuento { get; set; }

        [Required]
        public int CategoriaProductoId { get; set; }

        [Required]
        public CategoriaProducto CategoriaProducto { get; set; }

        [Required]
        public List<ProductoOrden> ProductosOrdenados { get; }
    }
}
