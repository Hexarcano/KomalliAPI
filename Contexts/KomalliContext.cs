using KomalliAPI.CategoriasProducto.Entities;
using KomalliAPI.Ordenes.Entities;
using KomalliAPI.Productos.Entities;
using Microsoft.EntityFrameworkCore;

namespace KomalliAPI.Contexts
{
    public class KomalliContext : DbContext
    {
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<ProductoOrden> ProductosOrden { get; set; }

        public KomalliContext(DbContextOptions<KomalliContext> options) : base(options) { }
    }
}
