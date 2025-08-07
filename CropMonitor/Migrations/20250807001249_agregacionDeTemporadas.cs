using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CropMonitor.Migrations
{
    /// <inheritdoc />
    public partial class agregacionDeTemporadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Temporadas",
                columns: new[] { "TemporadaID", "NombreTemporada" },
                values: new object[,]
                {
                    { 1, "Primavera" },
                    { 2, "Verano" },
                    { 3, "Otoño" },
                    { 4, "Invierno" }
                });

            migrationBuilder.InsertData(
                table: "Cultivos_Temporadas",
                columns: new[] { "CultivoID", "TemporadaID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 1, 4 },
                    { 2, 1 },
                    { 2, 3 },
                    { 2, 4 },
                    { 3, 1 },
                    { 3, 3 },
                    { 4, 1 },
                    { 4, 3 },
                    { 4, 4 },
                    { 5, 1 },
                    { 5, 3 },
                    { 6, 1 },
                    { 6, 2 },
                    { 7, 1 },
                    { 7, 2 },
                    { 8, 1 },
                    { 8, 2 },
                    { 9, 1 },
                    { 9, 2 },
                    { 10, 1 },
                    { 10, 2 },
                    { 11, 1 },
                    { 11, 2 },
                    { 12, 2 },
                    { 13, 1 },
                    { 13, 3 },
                    { 14, 3 },
                    { 15, 2 },
                    { 15, 3 },
                    { 15, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 9, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 11, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 11, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 12, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 13, 1 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 13, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 14, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 15, 2 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 15, 3 });

            migrationBuilder.DeleteData(
                table: "Cultivos_Temporadas",
                keyColumns: new[] { "CultivoID", "TemporadaID" },
                keyValues: new object[] { 15, 4 });

            migrationBuilder.DeleteData(
                table: "Temporadas",
                keyColumn: "TemporadaID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Temporadas",
                keyColumn: "TemporadaID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Temporadas",
                keyColumn: "TemporadaID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Temporadas",
                keyColumn: "TemporadaID",
                keyValue: 4);
        }
    }
}
