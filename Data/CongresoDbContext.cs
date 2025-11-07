using Microsoft.EntityFrameworkCore;
using CongresoTIC.API.Models;

namespace CongresoTIC.API.Data
{
    public class CongresoDbContext : DbContext
    {
        public CongresoDbContext(DbContextOptions<CongresoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Participante> Participantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Participante>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UsuarioTwitter).IsUnique();
            });
        }
    }
}