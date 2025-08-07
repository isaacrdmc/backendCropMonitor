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

modelBuilder.Entity<Cultivo>().HasData(
    new Cultivo
    {
        CultivoID = 1,
        Nombre = "Lechuga",
        Descripcion = "Planta herbácea cultivada para la alimentación. Sus hojas se consumen frescas en ensaladas.",
        ImagenURL = "https://www.guiadejardineria.com/wp-content/uploads/2016/07/cultiva-tus-lechugas-en-maceta-03.jpg",
        RequisitosClima = "Clima fresco, 15-18°C",
        RequisitosAgua = "Riego frecuente, mantener el suelo húmedo",
        RequisitosLuz = "Sol parcial"
    },
    new Cultivo
    {
        CultivoID = 2,
        Nombre = "Espinaca",
        Descripcion = "Hortaliza de hoja que crece rápido, ideal para consumo fresco o cocido.",
        ImagenURL = "https://www.minutoar.com.ar/u/fotografias/m/2021/3/18/f768x1-50265_50392_110.jpg",
        RequisitosClima = "Clima fresco, 10-20°C",
        RequisitosAgua = "Riego regular, evitar el encharcamiento",
        RequisitosLuz = "Sol parcial o sombra"
    },
    new Cultivo
    {
        CultivoID = 3,
        Nombre = "Acelga",
        Descripcion = "Planta de hojas grandes, muy nutritiva y resistente. Se usa en guisos y salteados.",
        ImagenURL = "https://www.unhuertoenmibalcon.com/blog/wp-content/uploads/IMG_20130613_210642_web.jpg",
        RequisitosClima = "Amplia tolerancia, 10-25°C",
        RequisitosAgua = "Riego abundante y constante",
        RequisitosLuz = "Sol directo o semisombra"
    },
    new Cultivo
    {
        CultivoID = 4,
        Nombre = "Kale",
        Descripcion = "Una de las hortalizas más nutritivas. Hojas rizadas, ideal para ensaladas o batidos.",
        ImagenURL = "https://www.menudiet.es/images/blog/kale-la-col-rizada-americana.jpg",
        RequisitosClima = "Clima fresco, resiste heladas",
        RequisitosAgua = "Riego regular",
        RequisitosLuz = "Sol directo"
    },
    new Cultivo
    {
        CultivoID = 5,
        Nombre = "Rúcula",
        Descripcion = "Hierba de sabor picante y distintivo. Crece rápidamente, perfecta para ensaladas.",
        ImagenURL = "https://graciasnaturaleza.com/wp-content/uploads/2022/05/plantar-rucula-d.jpg",
        RequisitosClima = "Clima fresco a templado",
        RequisitosAgua = "Riego frecuente, evitar que el suelo se seque",
        RequisitosLuz = "Sol completo"
    },
    new Cultivo
    {
        CultivoID = 6,
        Nombre = "Tomate cherry",
        Descripcion = "Variedad de tomate pequeña y dulce. Perfecta para cultivar en macetas en balcones y terrazas.",
        ImagenURL = "https://huerto-en-casa.com/wp-content/uploads/2022/01/tomate-cherry-en-maceta.jpg",
        RequisitosClima = "Clima cálido, 20-30°C",
        RequisitosAgua = "Riego abundante y constante, especialmente en verano",
        RequisitosLuz = "Mucho sol"
    },
    new Cultivo
    {
        CultivoID = 7,
        Nombre = "Pimiento",
        Descripcion = "Planta que produce frutos dulces o picantes. Requiere mucho sol para fructificar.",
        ImagenURL = "https://cdn.manomano.com/media/edison/4/7/1/6/47161050dc9c.jpg",
        RequisitosClima = "Clima cálido, 20-25°C",
        RequisitosAgua = "Riego regular, sin encharcar",
        RequisitosLuz = "Sol directo"
    },
    new Cultivo
    {
        CultivoID = 8,
        Nombre = "Calabacín",
        Descripcion = "Hortaliza que produce frutos alargados. Necesita espacio y un recipiente grande.",
        ImagenURL = "https://cdn0.ecologiaverde.com/es/posts/4/5/7/cultivar_calabacin_en_maceta_754_orig.jpg",
        RequisitosClima = "Clima cálido, 18-24°C",
        RequisitosAgua = "Riego muy abundante",
        RequisitosLuz = "Sol directo"
    },
    new Cultivo
    {
        CultivoID = 9,
        Nombre = "Fresa",
        Descripcion = "Pequeño fruto rojo y dulce. Se adapta bien a macetas colgantes o camas elevadas.",
        ImagenURL = "https://thumbs.dreamstime.com/b/cierre-de-una-planta-fresa-con-jugosas-bayas-rojas-en-olla-terracota-ideal-para-la-jardiner%C3%ADa-el-hogar-comida-saludable-y-temas-385577850.jpg",
        RequisitosClima = "Templado, 15-25°C",
        RequisitosAgua = "Riego regular, mantener el suelo húmedo",
        RequisitosLuz = "Sol completo"
    },
    new Cultivo
    {
        CultivoID = 10,
        Nombre = "Pepino",
        Descripcion = "Fruto alargado, crujiente y refrescante. Requiere un tutor para trepar.",
        ImagenURL = "https://www.imporalaska.com/uploads/products/2022/01/pic_1643299016_1643299058.jpg",
        RequisitosClima = "Clima cálido, 20-30°C",
        RequisitosAgua = "Riego muy abundante y constante",
        RequisitosLuz = "Sol directo"
    },
    new Cultivo
    {
        CultivoID = 11,
        Nombre = "Albahaca",
        Descripcion = "Hierba aromática con hojas verdes y un sabor dulce y picante. Ideal para la cocina italiana.",
        ImagenURL = "https://s1.elespanol.com/2015/06/11/cocinillas/cocinillas_40255977_116187896_425x640.jpg",
        RequisitosClima = "Clima cálido, sensible al frío",
        RequisitosAgua = "Riego moderado, sin mojar las hojas",
        RequisitosLuz = "Sol directo"
    },
    new Cultivo
    {
        CultivoID = 12,
        Nombre = "Menta",
        Descripcion = "Hierba muy fácil de cultivar y con un aroma refrescante. Se propaga rápidamente.",
        ImagenURL = "https://www.launion.com.mx/images/2025/1600602ab00db37fdab66f3be0de1c98.jpg",
        RequisitosClima = "Clima templado, resiste el frío",
        RequisitosAgua = "Riego abundante",
        RequisitosLuz = "Sol parcial"
    },
    new Cultivo
    {
        CultivoID = 13,
        Nombre = "Perejil",
        Descripcion = "Hierba aromática con hojas finas y un sabor fresco. Muy utilizada en la cocina.",
        ImagenURL = "https://s1.elespanol.com/2021/07/07/actualidad/594701747_194475799_1706x960.jpg",
        RequisitosClima = "Clima templado",
        RequisitosAgua = "Riego regular, mantener la humedad",
        RequisitosLuz = "Semisombra"
    },
    new Cultivo
    {
        CultivoID = 14,
        Nombre = "Cilantro",
        Descripcion = "Hierba de sabor intenso y fresco, común en la cocina latinoamericana y asiática.",
        ImagenURL = "https://cdn0.uncomo.com/es/posts/0/3/7/como_sembrar_cilantro_27730_orig.jpg",
        RequisitosClima = "Clima templado a fresco",
        RequisitosAgua = "Riego moderado",
        RequisitosLuz = "Sol parcial"
    },
    new Cultivo
    {
        CultivoID = 15,
        Nombre = "Romero",
        Descripcion = "Arbusto aromático de hojas perennes. Muy resistente, ideal para principiantes.",
        ImagenURL = "https://s1.elespanol.com/2015/07/22/cocinillas/cocinillas_50504957_116199069_1273x1280.jpg",
        RequisitosClima = "Clima seco y cálido",
        RequisitosAgua = "Riego escaso, resiste sequía",
        RequisitosLuz = "Sol directo"
    }
);

            modelBuilder.Entity<Temporada>().HasData(
    new Temporada { TemporadaID = 1, NombreTemporada = "Primavera" },
    new Temporada { TemporadaID = 2, NombreTemporada = "Verano" },
    new Temporada { TemporadaID = 3, NombreTemporada = "Otoño" },
    new Temporada { TemporadaID = 4, NombreTemporada = "Invierno" }
);

            modelBuilder.Entity<CultivosTemporada>().HasData(
    // Primavera
    new CultivosTemporada { CultivoID = 1, TemporadaID = 1 },  // Lechuga
    new CultivosTemporada { CultivoID = 2, TemporadaID = 1 },  // Espinaca
    new CultivosTemporada { CultivoID = 3, TemporadaID = 1 },  // Acelga
    new CultivosTemporada { CultivoID = 4, TemporadaID = 1 },  // Kale
    new CultivosTemporada { CultivoID = 5, TemporadaID = 1 },  // Rúcula
    new CultivosTemporada { CultivoID = 6, TemporadaID = 1 },  // Tomate cherry
    new CultivosTemporada { CultivoID = 7, TemporadaID = 1 },  // Pimiento
    new CultivosTemporada { CultivoID = 8, TemporadaID = 1 },  // Calabacín
    new CultivosTemporada { CultivoID = 9, TemporadaID = 1 },  // Fresa
    new CultivosTemporada { CultivoID = 10, TemporadaID = 1 }, // Pepino
    new CultivosTemporada { CultivoID = 11, TemporadaID = 1 }, // Albahaca
    new CultivosTemporada { CultivoID = 13, TemporadaID = 1 }, // Perejil

    // Verano
    new CultivosTemporada { CultivoID = 6, TemporadaID = 2 },  // Tomate cherry
    new CultivosTemporada { CultivoID = 7, TemporadaID = 2 },  // Pimiento
    new CultivosTemporada { CultivoID = 8, TemporadaID = 2 },  // Calabacín
    new CultivosTemporada { CultivoID = 9, TemporadaID = 2 },  // Fresa
    new CultivosTemporada { CultivoID = 10, TemporadaID = 2 }, // Pepino
    new CultivosTemporada { CultivoID = 11, TemporadaID = 2 }, // Albahaca
    new CultivosTemporada { CultivoID = 12, TemporadaID = 2 }, // Menta
    new CultivosTemporada { CultivoID = 15, TemporadaID = 2 }, // Romero

    // Otoño
    new CultivosTemporada { CultivoID = 1, TemporadaID = 3 },  // Lechuga
    new CultivosTemporada { CultivoID = 2, TemporadaID = 3 },  // Espinaca
    new CultivosTemporada { CultivoID = 3, TemporadaID = 3 },  // Acelga
    new CultivosTemporada { CultivoID = 4, TemporadaID = 3 },  // Kale
    new CultivosTemporada { CultivoID = 5, TemporadaID = 3 },  // Rúcula
    new CultivosTemporada { CultivoID = 13, TemporadaID = 3 }, // Perejil
    new CultivosTemporada { CultivoID = 14, TemporadaID = 3 }, // Cilantro
    new CultivosTemporada { CultivoID = 15, TemporadaID = 3 }, // Romero

    // Invierno
    new CultivosTemporada { CultivoID = 1, TemporadaID = 4 },  // Lechuga
    new CultivosTemporada { CultivoID = 2, TemporadaID = 4 },  // Espinaca
    new CultivosTemporada { CultivoID = 4, TemporadaID = 4 },  // Kale
    new CultivosTemporada { CultivoID = 15, TemporadaID = 4 } // Romero
);

    modelBuilder.Entity<Receta>().HasData(
    new Receta
    {
        RecetaID = 1,
        NombreReceta = "Ensalada Fresca de Tomate Cherry, Albahaca y Rúcula",
        Descripcion = "Una ensalada sencilla, refrescante y llena de sabor, perfecta para un almuerzo ligero o como guarnición. Destaca la frescura del tomate y la albahaca, complementada con el toque picante de la rúcula.",
        Instrucciones = "1. Lava y seca bien la rúcula, los tomates cherry y las hojas de albahaca.\n2. Corta los tomates cherry por la mitad.\n3. En un bol grande, combina la rúcula y los tomates.\n4. Para el aderezo, mezcla aceite de oliva, vinagre balsámico, sal y pimienta al gusto. Emulsiona bien.\n5. Vierte el aderezo sobre la ensalada y mezcla suavemente.\n6. Espolvorea las hojas de albahaca fresca picada por encima y sirve de inmediato."
    }
);

            modelBuilder.Entity<RecetasCultivo>().HasData(
    new RecetasCultivo { RecetaID = 1, CultivoID = 6 },  // Tomate cherry
    new RecetasCultivo { RecetaID = 1, CultivoID = 11 }, // Albahaca
    new RecetasCultivo { RecetaID = 1, CultivoID = 5 }   // Rúcula
);

            base.OnModelCreating(modelBuilder);
        }


    }
}