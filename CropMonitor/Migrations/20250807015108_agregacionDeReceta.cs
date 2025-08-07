using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CropMonitor.Migrations
{
    /// <inheritdoc />
    public partial class agregacionDeReceta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Recetas",
                columns: new[] { "RecetaID", "Descripcion", "Instrucciones", "NombreReceta" },
                values: new object[] { 1, "Una ensalada sencilla, refrescante y llena de sabor, perfecta para un almuerzo ligero o como guarnición. Destaca la frescura del tomate y la albahaca, complementada con el toque picante de la rúcula.", "1. Lava y seca bien la rúcula, los tomates cherry y las hojas de albahaca.\n2. Corta los tomates cherry por la mitad.\n3. En un bol grande, combina la rúcula y los tomates.\n4. Para el aderezo, mezcla aceite de oliva, vinagre balsámico, sal y pimienta al gusto. Emulsiona bien.\n5. Vierte el aderezo sobre la ensalada y mezcla suavemente.\n6. Espolvorea las hojas de albahaca fresca picada por encima y sirve de inmediato.", "Ensalada Fresca de Tomate Cherry, Albahaca y Rúcula" });

            migrationBuilder.InsertData(
                table: "Recetas_Cultivos",
                columns: new[] { "CultivoID", "RecetaID" },
                values: new object[,]
                {
                    { 5, 1 },
                    { 6, 1 },
                    { 11, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Recetas_Cultivos",
                keyColumns: new[] { "CultivoID", "RecetaID" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "Recetas_Cultivos",
                keyColumns: new[] { "CultivoID", "RecetaID" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "Recetas_Cultivos",
                keyColumns: new[] { "CultivoID", "RecetaID" },
                keyValues: new object[] { 11, 1 });

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "RecetaID",
                keyValue: 1);
        }
    }
}
