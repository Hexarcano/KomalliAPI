using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Productos.Entities
{
    public class ProductoRegistro
    {
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

    }
}
