using KomalliAPI.Productos.Entities;
using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.CategoriasProducto.Entities
{
    public class CategoriaProductoRegistro
    {
        [Required]
        [MaxLength(40)]
        public string Nombre { get; set; }

        [Required]
        public String ImagenBase64 { get; set; }
    }
}
