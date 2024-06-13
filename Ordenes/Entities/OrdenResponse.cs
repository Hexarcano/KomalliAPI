namespace KomalliAPI.Ordenes.Entities
{
    public class OrdenResponse
    {
        public string Mensaje { get; set; }
        public List<OrdenConsulta> Ordenes { get; set; }
        public List<ProductoOrdenConsulta> Productos { get; set; }
    }
}
