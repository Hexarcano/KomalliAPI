namespace KomalliAPI.Productos.Entities
{
    public class ProductoResponse
    {
        public string Mensaje { get; set; }
        public List<ProductoConsulta>? Productos { get; set; }
    }
}
