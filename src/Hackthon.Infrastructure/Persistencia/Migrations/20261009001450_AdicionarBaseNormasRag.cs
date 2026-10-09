using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Hackthon.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarBaseNormasRag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AddColumn<string>(
                name: "ClasseRecomendadaIa",
                table: "projetos",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContradicoesIa",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciasDossieIa",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LacunasIa",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoClasseIa",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormasCitadasIaJson",
                table: "projetos",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrechosConsultadosIa",
                table: "projetos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "UsouRag",
                table: "projetos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "documentos_normativos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Arquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Trechos = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_normativos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "trechos_normativos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentoNome = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Referencia = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Pagina = table.Column<int>(type: "integer", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(768)", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trechos_normativos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trechos_normativos_documentos_normativos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "documentos_normativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_documentos_normativos_Nome",
                table: "documentos_normativos",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trechos_normativos_DocumentoId_Ordem",
                table: "trechos_normativos",
                columns: new[] { "DocumentoId", "Ordem" });

            migrationBuilder.CreateIndex(
                name: "ix_trechos_normativos_embedding",
                table: "trechos_normativos",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trechos_normativos");

            migrationBuilder.DropTable(
                name: "documentos_normativos");

            migrationBuilder.DropColumn(
                name: "ClasseRecomendadaIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "ContradicoesIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "EvidenciasDossieIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "LacunasIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "MotivoClasseIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NormasCitadasIaJson",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "TrechosConsultadosIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "UsouRag",
                table: "projetos");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
        }
    }
}
