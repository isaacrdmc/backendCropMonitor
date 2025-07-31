using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CropMonitor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    BlogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.BlogID);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteID);
                });

            migrationBuilder.CreateTable(
                name: "Contacto",
                columns: table => new
                {
                    ContactoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacto", x => x.ContactoID);
                });

            migrationBuilder.CreateTable(
                name: "Cultivos",
                columns: table => new
                {
                    CultivoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequisitosClima = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RequisitosAgua = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RequisitosLuz = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultivos", x => x.CultivoID);
                });

            migrationBuilder.CreateTable(
                name: "FAQ",
                columns: table => new
                {
                    FAQID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pregunta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Respuesta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQ", x => x.FAQID);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: true),
                    Unidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoID);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    ProveedorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEmpresa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.ProveedorID);
                });

            migrationBuilder.CreateTable(
                name: "Recetas",
                columns: table => new
                {
                    RecetaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreReceta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instrucciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.RecetaID);
                });

            migrationBuilder.CreateTable(
                name: "Temporadas",
                columns: table => new
                {
                    TemporadaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTemporada = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temporadas", x => x.TemporadaID);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RolUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmailConfirmado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    VentaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteID = table.Column<int>(type: "int", nullable: true),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.VentaID);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ClienteID");
                });

            migrationBuilder.CreateTable(
                name: "GuiaCultivo",
                columns: table => new
                {
                    GuiaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CultivoID = table.Column<int>(type: "int", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiaCultivo", x => x.GuiaID);
                    table.ForeignKey(
                        name: "FK_GuiaCultivo_Cultivos_CultivoID",
                        column: x => x.CultivoID,
                        principalTable: "Cultivos",
                        principalColumn: "CultivoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kardex",
                columns: table => new
                {
                    KardexID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoID = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "date", nullable: true),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Promedio = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Debe = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Haber = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Saldo = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kardex", x => x.KardexID);
                    table.ForeignKey(
                        name: "FK_Kardex_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID");
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    CompraID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorID = table.Column<int>(type: "int", nullable: true),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.CompraID);
                    table.ForeignKey(
                        name: "FK_Compras_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorID");
                });

            migrationBuilder.CreateTable(
                name: "Recetas_Cultivos",
                columns: table => new
                {
                    RecetaID = table.Column<int>(type: "int", nullable: false),
                    CultivoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas_Cultivos", x => new { x.RecetaID, x.CultivoID });
                    table.ForeignKey(
                        name: "FK_Recetas_Cultivos_Cultivos_CultivoID",
                        column: x => x.CultivoID,
                        principalTable: "Cultivos",
                        principalColumn: "CultivoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recetas_Cultivos_Recetas_RecetaID",
                        column: x => x.RecetaID,
                        principalTable: "Recetas",
                        principalColumn: "RecetaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cultivos_Temporadas",
                columns: table => new
                {
                    CultivoID = table.Column<int>(type: "int", nullable: false),
                    TemporadaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultivos_Temporadas", x => new { x.CultivoID, x.TemporadaID });
                    table.ForeignKey(
                        name: "FK_Cultivos_Temporadas_Cultivos_CultivoID",
                        column: x => x.CultivoID,
                        principalTable: "Cultivos",
                        principalColumn: "CultivoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cultivos_Temporadas_Temporadas_TemporadaID",
                        column: x => x.TemporadaID,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComentariosBlog",
                columns: table => new
                {
                    ComentarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogID = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaComentario = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosBlog", x => x.ComentarioID);
                    table.ForeignKey(
                        name: "FK_ComentariosBlog_Blog_BlogID",
                        column: x => x.BlogID,
                        principalTable: "Blog",
                        principalColumn: "BlogID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentariosBlog_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID");
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionNotificaciones",
                columns: table => new
                {
                    ConfiguracionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    FrecuenciaRiego = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HorarioNotificacion = table.Column<TimeSpan>(type: "time", nullable: true),
                    ActivarRiegoAutomatico = table.Column<bool>(type: "bit", nullable: false),
                    TipoAlertaSensor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HabilitarRecomendacionesEstacionales = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionNotificaciones", x => x.ConfiguracionID);
                    table.ForeignKey(
                        name: "FK_ConfiguracionNotificaciones_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoritos",
                columns: table => new
                {
                    FavoritoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    CultivoID = table.Column<int>(type: "int", nullable: false),
                    FechaAgregado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoritos", x => x.FavoritoID);
                    table.ForeignKey(
                        name: "FK_Favoritos_Cultivos_CultivoID",
                        column: x => x.CultivoID,
                        principalTable: "Cultivos",
                        principalColumn: "CultivoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoritos_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    ModuloID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreModulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiasEnFuncionamiento = table.Column<int>(type: "int", nullable: true),
                    CantidadCultivosActual = table.Column<int>(type: "int", nullable: true),
                    CantidadCultivosMax = table.Column<int>(type: "int", nullable: true),
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.ModuloID);
                    table.ForeignKey(
                        name: "FK_Modulos_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipsCultivos",
                columns: table => new
                {
                    TipID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CultivoID = table.Column<int>(type: "int", nullable: false),
                    DescripcionTip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipsCultivos", x => x.TipID);
                    table.ForeignKey(
                        name: "FK_TipsCultivos_Cultivos_CultivoID",
                        column: x => x.CultivoID,
                        principalTable: "Cultivos",
                        principalColumn: "CultivoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TipsCultivos_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID");
                });

            migrationBuilder.CreateTable(
                name: "DetalleVentas",
                columns: table => new
                {
                    DetalleVentaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaID = table.Column<int>(type: "int", nullable: true),
                    ProductoID = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    PrecioVenta = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleVentas", x => x.DetalleVentaID);
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID");
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Ventas_VentaID",
                        column: x => x.VentaID,
                        principalTable: "Ventas",
                        principalColumn: "VentaID");
                });

            migrationBuilder.CreateTable(
                name: "DetalleCompras",
                columns: table => new
                {
                    DetalleCompraID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraID = table.Column<int>(type: "int", nullable: true),
                    ProductoID = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleCompras", x => x.DetalleCompraID);
                    table.ForeignKey(
                        name: "FK_DetalleCompras_Compras_CompraID",
                        column: x => x.CompraID,
                        principalTable: "Compras",
                        principalColumn: "CompraID");
                    table.ForeignKey(
                        name: "FK_DetalleCompras_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID");
                });

            migrationBuilder.CreateTable(
                name: "Sensores",
                columns: table => new
                {
                    SensorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuloID = table.Column<int>(type: "int", nullable: false),
                    CultivoID = table.Column<int>(type: "int", nullable: true),
                    TipoSensor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UltimaLectura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorLectura = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    EstadoRiego = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EsAcuaHidroponico = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensores", x => x.SensorID);
                    table.ForeignKey(
                        name: "FK_Sensores_Cultivos_CultivoID",
                        column: x => x.CultivoID,
                        principalTable: "Cultivos",
                        principalColumn: "CultivoID");
                    table.ForeignKey(
                        name: "FK_Sensores_Modulos_ModuloID",
                        column: x => x.ModuloID,
                        principalTable: "Modulos",
                        principalColumn: "ModuloID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LecturasSensores",
                columns: table => new
                {
                    LecturaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorID = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturasSensores", x => x.LecturaID);
                    table.ForeignKey(
                        name: "FK_LecturasSensores_Sensores_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensores",
                        principalColumn: "SensorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    NotificacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    TipoNotificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaHoraEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    CultivoID = table.Column<int>(type: "int", nullable: true),
                    SensorID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.NotificacionID);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Cultivos_CultivoID",
                        column: x => x.CultivoID,
                        principalTable: "Cultivos",
                        principalColumn: "CultivoID");
                    table.ForeignKey(
                        name: "FK_Notificaciones_Sensores_SensorID",
                        column: x => x.SensorID,
                        principalTable: "Sensores",
                        principalColumn: "SensorID");
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Autor",
                table: "Blog",
                column: "Autor");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_FechaPublicacion",
                table: "Blog",
                column: "FechaPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Correo",
                table: "Clientes",
                column: "Correo");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosBlog_BlogID",
                table: "ComentariosBlog",
                column: "BlogID");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosBlog_UsuarioID",
                table: "ComentariosBlog",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_FechaCompra",
                table: "Compras",
                column: "FechaCompra");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ProveedorID",
                table: "Compras",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionNotificaciones_UsuarioID",
                table: "ConfiguracionNotificaciones",
                column: "UsuarioID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cultivos_Temporadas_TemporadaID",
                table: "Cultivos_Temporadas",
                column: "TemporadaID");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCompras_CompraID",
                table: "DetalleCompras",
                column: "CompraID");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCompras_ProductoID",
                table: "DetalleCompras",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_ProductoID",
                table: "DetalleVentas",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_VentaID",
                table: "DetalleVentas",
                column: "VentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_CultivoID",
                table: "Favoritos",
                column: "CultivoID");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_UsuarioID",
                table: "Favoritos",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_UsuarioID_CultivoID",
                table: "Favoritos",
                columns: new[] { "UsuarioID", "CultivoID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuiaCultivo_CultivoID",
                table: "GuiaCultivo",
                column: "CultivoID");

            migrationBuilder.CreateIndex(
                name: "IX_Kardex_ProductoID_Fecha",
                table: "Kardex",
                columns: new[] { "ProductoID", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturasSensores_SensorID_Timestamp",
                table: "LecturasSensores",
                columns: new[] { "SensorID", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_UsuarioID",
                table: "Modulos",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_CultivoID",
                table: "Notificaciones",
                column: "CultivoID");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_SensorID",
                table: "Notificaciones",
                column: "SensorID");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioID_Leida",
                table: "Notificaciones",
                columns: new[] { "UsuarioID", "Leida" });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Nombre",
                table: "Productos",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_Cultivos_CultivoID",
                table: "Recetas_Cultivos",
                column: "CultivoID");

            migrationBuilder.CreateIndex(
                name: "IX_Sensores_CultivoID",
                table: "Sensores",
                column: "CultivoID");

            migrationBuilder.CreateIndex(
                name: "IX_Sensores_ModuloID",
                table: "Sensores",
                column: "ModuloID");

            migrationBuilder.CreateIndex(
                name: "IX_TipsCultivos_CultivoID",
                table: "TipsCultivos",
                column: "CultivoID");

            migrationBuilder.CreateIndex(
                name: "IX_TipsCultivos_UsuarioID",
                table: "TipsCultivos",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteID",
                table: "Ventas",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_FechaVenta",
                table: "Ventas",
                column: "FechaVenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentariosBlog");

            migrationBuilder.DropTable(
                name: "ConfiguracionNotificaciones");

            migrationBuilder.DropTable(
                name: "Contacto");

            migrationBuilder.DropTable(
                name: "Cultivos_Temporadas");

            migrationBuilder.DropTable(
                name: "DetalleCompras");

            migrationBuilder.DropTable(
                name: "DetalleVentas");

            migrationBuilder.DropTable(
                name: "FAQ");

            migrationBuilder.DropTable(
                name: "Favoritos");

            migrationBuilder.DropTable(
                name: "GuiaCultivo");

            migrationBuilder.DropTable(
                name: "Kardex");

            migrationBuilder.DropTable(
                name: "LecturasSensores");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Recetas_Cultivos");

            migrationBuilder.DropTable(
                name: "TipsCultivos");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "Temporadas");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Sensores");

            migrationBuilder.DropTable(
                name: "Recetas");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Cultivos");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
