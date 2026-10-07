using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackthon.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCpfCnpjEPapelAssistente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CpfCnpj",
                table: "usuarios",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_CpfCnpj",
                table: "usuarios",
                column: "CpfCnpj",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_usuarios_CpfCnpj",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "CpfCnpj",
                table: "usuarios");
        }
    }
}
