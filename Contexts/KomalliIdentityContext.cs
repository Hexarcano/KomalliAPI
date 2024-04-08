using KomalliAPI.Clientes.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KomalliAPI.Contexts
{
    public class KomalliIdentityContext : IdentityDbContext<Cliente>
    {
        public KomalliIdentityContext(DbContextOptions<KomalliIdentityContext> options) : base(options) { }
    }
}
