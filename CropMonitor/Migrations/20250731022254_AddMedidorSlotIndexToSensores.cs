using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CropMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddMedidorSlotIndexToSensores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedidorSlotIndex",
                table: "Sensores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedidorSlotIndex",
                table: "Sensores");
        }
    }
}
