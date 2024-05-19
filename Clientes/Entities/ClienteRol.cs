using Microsoft.AspNetCore.Identity;

namespace KomalliAPI.Clientes.Entities
{
    public class ClienteRol : IdentityRole<Guid>
    {
        public ClienteRol() : base() { }

        public ClienteRol(string rolName) : base(rolName) { }
    }
}
