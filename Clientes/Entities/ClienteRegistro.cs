using System.ComponentModel.DataAnnotations;

namespace KomalliAPI.Clientes.Entities
{
    public class ClienteRegistro
    {
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(30)]
        public string ApellidoPaterno { get; set; }

        [Required]
        [MaxLength(30)]
        public string ApellidoMaterno { get; set; }

        [Required]
        [MaxLength(10)]
        public string Usuario { get; set; }

        [Required]
        public string Contrasenia { get; set; }

        [Required]
        [MaxLength(28)]
        public string Email { get; set; }
    }
}
