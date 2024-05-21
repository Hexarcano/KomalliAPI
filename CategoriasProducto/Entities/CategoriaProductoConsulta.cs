using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.CategoriasProducto.Entities
{
    public class CategoriaProductoConsulta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ImagenBase64 { get; set; }
    }
}
