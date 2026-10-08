using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackthon.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDeliberacaoRastreavel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renomeia as classes binárias antigas para as 4 classes oficiais do desafio.
            // A coluna é "Status" (case-sensitive no PostgreSQL).
            migrationBuilder.Sql("UPDATE projetos SET \"Status\" = 'Elegivel' WHERE \"Status\" = 'Aprovado';");
            migrationBuilder.Sql("UPDATE projetos SET \"Status\" = 'NaoElegivel' WHERE \"Status\" = 'Reprovado';");

            migrationBuilder.AddColumn<string>(
                name: "EloAusente",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciaNecessaria",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciasAdicionais",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciasContrarias",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciasUtilizadas",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lacunas",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LimitacaoTecnica",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecorteSustentado",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegraAplicada",
                table: "projetos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverte as renomeações de classes para os valores binários anteriores.
            migrationBuilder.Sql("UPDATE projetos SET \"Status\" = 'Aprovado' WHERE \"Status\" = 'Elegivel';");
            migrationBuilder.Sql("UPDATE projetos SET \"Status\" = 'Reprovado' WHERE \"Status\" IN ('NaoElegivel', 'ComRessalvas', 'EvidenciaInsuficiente');");

            migrationBuilder.DropColumn(
                name: "EloAusente",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "EvidenciaNecessaria",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "EvidenciasAdicionais",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "EvidenciasContrarias",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "EvidenciasUtilizadas",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "Lacunas",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "LimitacaoTecnica",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "RecorteSustentado",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "RegraAplicada",
                table: "projetos");
        }
    }
}
