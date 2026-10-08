using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackthon.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarEstruturaFrascatiEPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnoBase",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "BeneficioEstimado",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "CndAtiva",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "DispendioEquipe",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "DispendioMateriais",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "DispendioTerceiros",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "DispendioTotal",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "IncertezaTecnica",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "LucroReal",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "Metodologia",
                table: "projetos");

            migrationBuilder.RenameColumn(
                name: "ParecerAnalista",
                table: "projetos",
                newName: "JustificativaAnalista");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "projetos",
                newName: "Setor");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AnalisadoEm",
                table: "projetos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AnalistaId",
                table: "projetos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaminhoArquivo",
                table: "projetos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DecididoEm",
                table: "projetos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JustificativasIaJson",
                table: "projetos",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeArquivoOriginal",
                table: "projetos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NotaCriatividade",
                table: "projetos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaIa",
                table: "projetos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaIncerteza",
                table: "projetos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaNovidade",
                table: "projetos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaSistematico",
                table: "projetos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaTransferibilidade",
                table: "projetos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrigemAnaliseIa",
                table: "projetos",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParecerIa",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecomendacaoIa",
                table: "projetos",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumoIa",
                table: "projetos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TamanhoBytes",
                table: "projetos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "TextoExtraido",
                table: "projetos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VersaoEnvio",
                table: "projetos",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_projetos_AnalistaId",
                table: "projetos",
                column: "AnalistaId");

            migrationBuilder.AddForeignKey(
                name: "FK_projetos_usuarios_AnalistaId",
                table: "projetos",
                column: "AnalistaId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projetos_usuarios_AnalistaId",
                table: "projetos");

            migrationBuilder.DropIndex(
                name: "IX_projetos_AnalistaId",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "AnalisadoEm",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "AnalistaId",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "CaminhoArquivo",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "DecididoEm",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "JustificativasIaJson",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NomeArquivoOriginal",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NotaCriatividade",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NotaIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NotaIncerteza",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NotaNovidade",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NotaSistematico",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "NotaTransferibilidade",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "OrigemAnaliseIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "ParecerIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "RecomendacaoIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "ResumoIa",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "TamanhoBytes",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "TextoExtraido",
                table: "projetos");

            migrationBuilder.DropColumn(
                name: "VersaoEnvio",
                table: "projetos");

            migrationBuilder.RenameColumn(
                name: "Setor",
                table: "projetos",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "JustificativaAnalista",
                table: "projetos",
                newName: "ParecerAnalista");

            migrationBuilder.AddColumn<string>(
                name: "AnoBase",
                table: "projetos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BeneficioEstimado",
                table: "projetos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CndAtiva",
                table: "projetos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DispendioEquipe",
                table: "projetos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DispendioMateriais",
                table: "projetos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DispendioTerceiros",
                table: "projetos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DispendioTotal",
                table: "projetos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "IncertezaTecnica",
                table: "projetos",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "LucroReal",
                table: "projetos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Metodologia",
                table: "projetos",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }
    }
}
