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
                name: "Guias",
                columns: table => new
                {
                    IdGuia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPlanta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CuidadoPlanta = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    NotasPlantas = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    EstacionPlanta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guias", x => x.IdGuia);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    IdModulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoModulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoModulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.IdModulo);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Estadisticas",
                columns: table => new
                {
                    IdEstadistica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdModulo = table.Column<int>(type: "int", nullable: false),
                    ValorTemperatura = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ValorHumedad = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ValorLuz = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    NivelAgua = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadisticas", x => x.IdEstadistica);
                    table.ForeignKey(
                        name: "FK_Estadisticas_Modulos_IdModulo",
                        column: x => x.IdModulo,
                        principalTable: "Modulos",
                        principalColumn: "IdModulo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plantas",
                columns: table => new
                {
                    IdPlanta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePlanta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoPlanta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UsoPlanta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FotoPlanta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Favorito = table.Column<bool>(type: "bit", nullable: false),
                    IdModulo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plantas", x => x.IdPlanta);
                    table.ForeignKey(
                        name: "FK_Plantas_Modulos_IdModulo",
                        column: x => x.IdModulo,
                        principalTable: "Modulos",
                        principalColumn: "IdModulo");
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    IdSensor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreSensor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoSensor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RangoMin = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    RangoMax = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    IdModulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.IdSensor);
                    table.ForeignKey(
                        name: "FK_Sensors_Modulos_IdModulo",
                        column: x => x.IdModulo,
                        principalTable: "Modulos",
                        principalColumn: "IdModulo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    IdComentario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    MeGusta = table.Column<int>(type: "int", nullable: false),
                    FechaComentario = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.IdComentario);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VentaModulos",
                columns: table => new
                {
                    IdVentas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadVenta = table.Column<int>(type: "int", nullable: false),
                    TotalVenta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdModulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaModulos", x => x.IdVentas);
                    table.ForeignKey(
                        name: "FK_VentaModulos_Modulos_IdModulo",
                        column: x => x.IdModulo,
                        principalTable: "Modulos",
                        principalColumn: "IdModulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VentaModulos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_IdUsuario",
                table: "Comentarios",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Estadisticas_IdModulo",
                table: "Estadisticas",
                column: "IdModulo");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_CodigoModulo",
                table: "Modulos",
                column: "CodigoModulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plantas_IdModulo",
                table: "Plantas",
                column: "IdModulo",
                unique: true,
                filter: "[IdModulo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_IdModulo",
                table: "Sensors",
                column: "IdModulo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VentaModulos_IdModulo",
                table: "VentaModulos",
                column: "IdModulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VentaModulos_IdUsuario",
                table: "VentaModulos",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Estadisticas");

            migrationBuilder.DropTable(
                name: "Guias");

            migrationBuilder.DropTable(
                name: "Plantas");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "VentaModulos");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
