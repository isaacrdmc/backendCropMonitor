using CropMonitor.Models.AppMovil;
using CropMonitor.Models.WebApp;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Data
{
    public class CropMonitorDbContext : DbContext
    {
        public CropMonitorDbContext(DbContextOptions<CropMonitorDbContext> options)
            : base(options)
        {
        }

        // Definiciones de DbSet para todas las tablas de la App Móvil
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cultivo> Cultivos { get; set; }
        public DbSet<Temporada> Temporadas { get; set; }
        public DbSet<CultivosTemporada> Cultivos_Temporadas { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<RecetasCultivo> Recetas_Cultivos { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Sensor> Sensores { get; set; }
        public DbSet<LecturaSensor> LecturasSensores { get; set; }
        public DbSet<TipsCultivo> TipsCultivos { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<ConfiguracionNotificacion> ConfiguracionNotificaciones { get; set; }

        // Definiciones de DbSet para todas las tablas de la Aplicación Web
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<ComentarioBlog> ComentariosBlog { get; set; }
        public DbSet<GuiaCultivo> GuiaCultivo { get; set; }
        public DbSet<FAQ> FAQ { get; set; }
        public DbSet<Contacto> Contacto { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Kardex> Kardex { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =========================================================
            // CONFIGURACIÓN DE RELACIONES Y CLAVES PRIMARIAS COMPUESTAS
            // =========================================================

            // Clave primaria compuesta para Cultivos_Temporadas
            modelBuilder.Entity<CultivosTemporada>()
                .HasKey(ct => new { ct.CultivoID, ct.TemporadaID });

            // Clave primaria compuesta para Recetas_Cultivos
            modelBuilder.Entity<RecetasCultivo>()
                .HasKey(rc => new { rc.RecetaID, rc.CultivoID });


            // =========================================================
            // CONFIGURACIÓN DE RELACIONES Y ON DELETE CASCADE
            // =========================================================

            // Usuario y Modulo (Un Usuario tiene muchos Modulos)
            modelBuilder.Entity<Modulo>()
                .HasOne(m => m.Usuario) // Modulo tiene UN Usuario (navegación desde Modulo)
                .WithMany(u => u.Modulos) // Usuario tiene MUCHOS Modulos (navegación desde Usuario)
                .HasForeignKey(m => m.UsuarioID)
                .OnDelete(DeleteBehavior.Cascade); // Cuando se borra un Usuario, se borran sus Modulos

            // Modulo y Sensor (Un Modulo tiene muchos Sensores)
            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.Modulo) // Sensor tiene UN Modulo (navegación desde Sensor)
                .WithMany(m => m.Sensores) // Modulo tiene MUCHOS Sensores (navegación desde Modulo)
                .HasForeignKey(s => s.ModuloID)
                .OnDelete(DeleteBehavior.Cascade); // Cuando se borra un Modulo, se borran sus Sensores

            // Sensor y LecturaSensor (Un Sensor tiene muchas LecturasSensor)
            modelBuilder.Entity<LecturaSensor>()
                .HasOne(ls => ls.Sensor) // LecturaSensor tiene UN Sensor (navegación desde LecturaSensor)
                .WithMany(s => s.LecturasSensores) // Sensor tiene MUCHAS LecturasSensor (navegación desde Sensor)
                .HasForeignKey(ls => ls.SensorID)
                .OnDelete(DeleteBehavior.Cascade); // Cuando se borra un Sensor, se borran sus LecturasSensores

            // Usuario y Favorito (Un Usuario tiene muchos Favoritos)
            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Usuario) // Favorito tiene UN Usuario (navegación desde Favorito)
                .WithMany(u => u.Favoritos) // Usuario tiene MUCHOS Favoritos (navegación desde Usuario)
                .HasForeignKey(f => f.UsuarioID)
                .OnDelete(DeleteBehavior.Cascade); // Cuando se borra un Usuario, se borran sus Favoritos

            // Usuario y Notificacion (Un Usuario tiene muchas Notificaciones)
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario) // Notificacion tiene UN Usuario (navegación desde Notificacion)
                .WithMany(u => u.Notificaciones) // Usuario tiene MUCHAS Notificaciones (navegación desde Usuario)
                .HasForeignKey(n => n.UsuarioID)
                .OnDelete(DeleteBehavior.Cascade); // Cuando se borra un Usuario, se borran sus Notificaciones

            // Usuario y ConfiguracionNotificacion (Relación uno a uno/cero o uno)
            modelBuilder.Entity<ConfiguracionNotificacion>()
                .HasOne(cn => cn.Usuario) // ConfiguracionNotificacion tiene UNA instancia de Usuario (navegación desde ConfiguracionNotificacion)
                .WithOne(u => u.ConfiguracionNotificaciones) // Usuario tiene UNA instancia de ConfiguracionNotificacion (navegación desde Usuario)
                .HasForeignKey<ConfiguracionNotificacion>(cn => cn.UsuarioID) // La clave foránea está en ConfiguracionNotificacion
                .OnDelete(DeleteBehavior.Cascade); // Cuando se borra un Usuario, se borra su Configuración de Notificación

            // Blog y ComentarioBlog (Un Blog tiene muchos ComentariosBlog)
            modelBuilder.Entity<ComentarioBlog>()
                .HasOne(cb => cb.Blog) // ComentarioBlog tiene UN Blog (navegación desde ComentarioBlog)
                .WithMany(b => b.ComentariosBlog) // Blog tiene MUCHOS ComentariosBlog (navegación desde Blog)
                .HasForeignKey(cb => cb.BlogID)
                .OnDelete(DeleteBehavior.Cascade); // Cuando se borra un Blog, se borran sus Comentarios

            // Usuario y ComentarioBlog (Un Usuario tiene muchos ComentariosBlog)
            // Nota: ComentarioBlog.UsuarioID es anulable, lo que EF Core maneja bien con HasOne y HasForeignKey.
            modelBuilder.Entity<ComentarioBlog>()
                .HasOne(cb => cb.Usuario) // ComentarioBlog tiene UN Usuario (navegación desde ComentarioBlog)
                .WithMany(u => u.ComentariosBlog) // Usuario tiene MUCHOS ComentariosBlog (navegación desde Usuario)
                .HasForeignKey(cb => cb.UsuarioID)
                .OnDelete(DeleteBehavior.SetNull); // Aquí se usa SetNull porque UsuarioID es anulable.
                                                   // Si el Usuario se elimina, los ComentariosBlog se mantienen
                                                   // pero su UsuarioID se establece en NULL. Si quieres CASCADE,
                                                   // UsuarioID no debería ser anulable en ComentarioBlog.
                                                   // La sugerencia es mantener SetNull si el ID es anulable.
                                                   // Relación RecetasCultivo a Receta
            modelBuilder.Entity<RecetasCultivo>()
                .HasOne(rc => rc.Receta)
                .WithMany(r => r.RecetasCultivos)
                .HasForeignKey(rc => rc.RecetaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecetasCultivo>()
            .HasOne(rc => rc.Cultivo)
            .WithMany(c => c.RecetasCultivos) // Asegúrate que Cultivo tenga ICollection<RecetasCultivo>
            .HasForeignKey(rc => rc.CultivoID)
            .OnDelete(DeleteBehavior.Cascade);


            // =========================================================
            // CONFIGURACIÓN DE ÍNDICES PARA OPTIMIZACIÓN DE CONSULTAS
            // (Tu configuración actual, simplemente la mantenemos)
            // =========================================================

            // App Móvil
            // Índice ÚNICO para búsquedas rápidas de usuarios por correo (importante para login)
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            // Índice para búsquedas de módulos por usuario
            modelBuilder.Entity<Modulo>()
                .HasIndex(m => m.UsuarioID);

            // Índices para búsquedas de sensores por módulo o cultivo
            modelBuilder.Entity<Sensor>()
                .HasIndex(s => s.ModuloID);
            modelBuilder.Entity<Sensor>()
                .HasIndex(s => s.CultivoID);

            // Índice para búsquedas rápidas en LecturasSensores por SensorID y fecha
            modelBuilder.Entity<LecturaSensor>()
                .HasIndex(ls => new { ls.SensorID, ls.Timestamp });

            // Índice para búsquedas de TipsCultivos por cultivo
            modelBuilder.Entity<TipsCultivo>()
                .HasIndex(tc => tc.CultivoID);

            // Índice ÚNICO para que un usuario solo tenga una configuración de notificación
            modelBuilder.Entity<ConfiguracionNotificacion>()
                .HasIndex(cn => cn.UsuarioID)
                .IsUnique();

            // Índice ÚNICO para Favoritos para evitar duplicados (un usuario solo puede tener un cultivo como favorito una vez)
            modelBuilder.Entity<Favorito>()
                .HasIndex(f => new { f.UsuarioID, f.CultivoID })
                .IsUnique();
            // Índice adicional para búsquedas de favoritos por usuario
            modelBuilder.Entity<Favorito>()
                .HasIndex(f => f.UsuarioID);


            // Índice para búsquedas de Notificaciones por usuario y estado de lectura
            modelBuilder.Entity<Notificacion>()
                .HasIndex(n => new { n.UsuarioID, n.Leida });


            // Aplicación Web
            // Índice para búsquedas rápidas de clientes por correo
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Correo);

            // Índices para búsquedas de blog por fecha de publicación o autor
            modelBuilder.Entity<Blog>()
                .HasIndex(b => b.FechaPublicacion);
            modelBuilder.Entity<Blog>()
                .HasIndex(b => b.Autor);

            // Índices para búsquedas de comentarios por blog o usuario
            modelBuilder.Entity<ComentarioBlog>()
                .HasIndex(cb => cb.BlogID);
            modelBuilder.Entity<ComentarioBlog>()
                .HasIndex(cb => cb.UsuarioID);

            // Índice para búsquedas de guías por cultivo
            modelBuilder.Entity<GuiaCultivo>()
                .HasIndex(gc => gc.CultivoID);

            // Índice para productos por nombre
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Nombre);

            // Índices para compras por proveedor o fecha
            modelBuilder.Entity<Compra>()
                .HasIndex(c => c.ProveedorID);
            modelBuilder.Entity<Compra>()
                .HasIndex(c => c.FechaCompra);

            // Índices para ventas por cliente o fecha
            modelBuilder.Entity<Venta>()
                .HasIndex(v => v.ClienteID);
            modelBuilder.Entity<Venta>()
                .HasIndex(v => v.FechaVenta);

            // Índice para Kardex por producto y fecha
            modelBuilder.Entity<Kardex>()
                .HasIndex(k => new { k.ProductoID, k.Fecha });

            base.OnModelCreating(modelBuilder);
        }
    }
}