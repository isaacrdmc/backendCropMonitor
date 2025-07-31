using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CropMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AllCorrectedRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosBlog_Usuarios_UsuarioID",
                table: "ComentariosBlog");

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "ComentariosBlog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosBlog_Usuarios_UsuarioID",
                table: "ComentariosBlog",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosBlog_Usuarios_UsuarioID",
                table: "ComentariosBlog");

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "ComentariosBlog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosBlog_Usuarios_UsuarioID",
                table: "ComentariosBlog",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID");
        }
    }
}
