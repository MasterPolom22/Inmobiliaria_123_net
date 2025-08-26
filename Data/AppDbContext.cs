using Microsoft.EntityFrameworkCore;
using WebInmo.Models;


namespace WebInmo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Propietario> Propietario => Set<Propietario>();
        public DbSet<Inquilino> Inquilino => Set<Inquilino>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // Propietario
            mb.Entity<Propietario>(e =>
            {
                e.Property(x => x.Apellido).HasMaxLength(60).IsRequired();
                e.Property(x => x.Nombre).HasMaxLength(60).IsRequired();
                e.Property(x => x.Dni).HasMaxLength(20).IsRequired();
                e.Property(x => x.Email).HasMaxLength(120);
                e.HasIndex(x => x.Dni).IsUnique();
                e.HasIndex(x => x.Email).IsUnique(false);
            });

            // Inquilino
            mb.Entity<Inquilino>(e =>
            {
                e.Property(x => x.Apellido).HasMaxLength(60).IsRequired();
                e.Property(x => x.Nombre).HasMaxLength(60).IsRequired();
                e.Property(x => x.Dni).HasMaxLength(20).IsRequired();
                e.Property(x => x.Email).HasMaxLength(120);
                e.HasIndex(x => x.Dni).IsUnique();
                e.HasIndex(x => x.Email).IsUnique(false);
            });
        }
    }
}
