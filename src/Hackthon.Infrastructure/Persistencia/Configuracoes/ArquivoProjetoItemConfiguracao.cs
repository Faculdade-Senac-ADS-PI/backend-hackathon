using Hackthon.Domain.Projetos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackthon.Infrastructure.Persistencia.Configuracoes;

public sealed class ArquivoProjetoItemConfiguracao : IEntityTypeConfiguration<ArquivoProjetoItem>
{
    public void Configure(EntityTypeBuilder<ArquivoProjetoItem> builder)
    {
        builder.ToTable("projeto_arquivos");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ProjetoId).IsRequired();
        builder.HasIndex(a => a.ProjetoId);

        builder.Property(a => a.NomeOriginal).HasMaxLength(255).IsRequired();
        builder.Property(a => a.Caminho).HasMaxLength(500).IsRequired();
        builder.Property(a => a.TamanhoBytes).IsRequired();
        builder.Property(a => a.Extensao).HasMaxLength(10).IsRequired();
        builder.Property(a => a.CriadoEm).IsRequired();
    }
}
