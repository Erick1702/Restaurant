using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace Restaurant.Datos
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) {}
        protected ApplicationDbContext() {}

        //Asignacion de LLaveCompuesta
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DetalleComanda>().HasKey(g => new { g.ComandaId, g.PlatoId }); // Definir la clave compuesta
        }
        //Asignacion de DbSet para las entidades
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<TipoPlato> TipoPlatos { get; set; }
        public DbSet<Plato> Platos { get; set; }
        public DbSet<TipoConsumo> TipoConsumos { get; set; }
        public DbSet<Comanda> Comandas { get; set; }
        public DbSet<DetalleComanda> DetalleComandas { get; set; }
        public DbSet<Factura> Facturas { get; set; }

    }
}
