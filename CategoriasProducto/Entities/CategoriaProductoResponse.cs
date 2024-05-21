namespace KomalliAPI.CategoriasProducto.Entities
{
    public class CategoriaProductoResponse
    {
        public string Mensaje { get; set; }
        public List<CategoriaProductoConsulta> Categorias { get; set; }
    }
}
