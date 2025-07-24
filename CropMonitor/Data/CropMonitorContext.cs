using CropMonitor.Models;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Data
{
    public class CropMonitorContext : DbContext
    {
        public CropMonitorContext(DbContextOptions<CropMonitorContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<VentaModulo> VentaModulos { get; set; }
        public DbSet<Planta> Plantas { get; set; }
        public DbSet<Estadistica> Estadisticas { get; set; }
        public DbSet<Guia> Guias { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Sensor> Sensors { get; set; } // Add the new Sensor DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-one relationship between Modulo and Planta
            modelBuilder.Entity<Modulo>()
                .HasOne(m => m.Planta)
                .WithOne(p => p.Modulo)
                .HasForeignKey<Planta>(p => p.IdModulo)
                .IsRequired(false); // IdModulo can be NULL

            // Configure one-to-one relationship between Modulo and VentaModulo
            modelBuilder.Entity<Modulo>()
                .HasOne(m => m.VentaModulo)
                .WithOne(vm => vm.Modulo)
                .HasForeignKey<VentaModulo>(vm => vm.IdModulo)
                .IsRequired(); // A VentaModulo must have a Modulo

            // Ensure unique constraint for Modulo.CodigoModulo
            modelBuilder.Entity<Modulo>()
                .HasIndex(m => m.CodigoModulo)
                .IsUnique();

            // Ensure unique constraint for Usuario.Correo
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            // Configure ComentarioText to map to the 'Comentario' column name if needed
            modelBuilder.Entity<Comentario>()
                .Property(c => c.ComentarioTexto)
                .HasColumnName("Comentario");

            // You might need to adjust column types or constraints here if they differ from default conventions
            // For example, if you want specific NVARCHAR lengths not handled by StringLength attribute.
        }
    }
}