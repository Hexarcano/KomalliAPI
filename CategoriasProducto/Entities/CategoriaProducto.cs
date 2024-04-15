using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.CategoriasProducto.Entities
{
    [Index(nameof(Nombre), IsUnique = true)]
    public class CategoriaProducto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Nombre { get; set; }

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
