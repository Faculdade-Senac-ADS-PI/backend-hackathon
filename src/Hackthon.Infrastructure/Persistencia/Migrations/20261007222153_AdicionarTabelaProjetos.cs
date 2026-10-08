using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackthon.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaProjetos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "projetos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Protocolo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AnoBase = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IncertezaTecnica = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Metodologia = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DispendioEquipe = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DispendioMateriais = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DispendioTerceiros = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DispendioTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BeneficioEstimado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LucroReal = table.Column<bool>(type: "boolean", nullable: false),
                    CndAtiva = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ParecerAnalista = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ProponenteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projetos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_projetos_usuarios_ProponenteId",
                        column: x => x.ProponenteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_projetos_ProponenteId",
                table: "projetos",
                column: "ProponenteId");

            migrationBuilder.CreateIndex(
                name: "IX_projetos_Protocolo",
                table: "projetos",
                column: "Protocolo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_projetos_Status",
                table: "projetos",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "projetos");
        }
    }
}
