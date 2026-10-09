using Hackthon.Infrastructure.Normas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackthon.Infrastructure.Persistencia.Configuracoes;

public sealed class DocumentoNormativoConfiguracao : IEntityTypeConfiguration<DocumentoNormativo>
{
    public void Configure(EntityTypeBuilder<DocumentoNormativo> builder)
    {
        builder.ToTable("documentos_normativos");
        builder.HasKey(documento => documento.Id);
        builder.Property(documento => documento.Nome).HasMaxLength(300).IsRequired();
        builder.HasIndex(documento => documento.Nome).IsUnique();
        builder.Property(documento => documento.Arquivo).HasMaxLength(500);
        builder.Property(documento => documento.Hash).HasMaxLength(64);

        builder.HasMany(documento => documento.Itens)
            .WithOne(trecho => trecho.Documento)
            .HasForeignKey(trecho => trecho.DocumentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class TrechoNormativoConfiguracao : IEntityTypeConfiguration<TrechoNormativo>
{
    public void Configure(EntityTypeBuilder<TrechoNormativo> builder)
    {
        builder.ToTable("trechos_normativos");
        builder.HasKey(trecho => trecho.Id);
        builder.Property(trecho => trecho.DocumentoNome).HasMaxLength(300).IsRequired();
        builder.Property(trecho => trecho.Referencia).HasMaxLength(120);
        builder.Property(trecho => trecho.Texto).IsRequired();
        builder.Property(trecho => trecho.Hash).HasMaxLength(64).IsRequired();

        builder.Property(trecho => trecho.Embedding)
            .HasColumnType($"vector({RagOpcoes.DimensaoPadrao})")
            .IsRequired();

        // Índice HNSW para busca por similaridade de cosseno.
        builder.HasIndex(trecho => trecho.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasDatabaseName("ix_trechos_normativos_embedding");

        builder.HasIndex(trecho => new { trecho.DocumentoId, trecho.Ordem });
    }
}
