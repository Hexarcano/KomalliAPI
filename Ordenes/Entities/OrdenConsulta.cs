namespace KomalliAPI.Ordenes.Entities
{
    public class OrdenConsulta
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string NombreCliente { get; set; }
        public double PrecioTotal { get; set; }
        public DateTime FechaExpedicion { get; set; }
        public DateTime? FechaPago { get; set; }
        public bool Pagado { get; set; }
        public string? Comentario { get; set; }
    }
}
