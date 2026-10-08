using Hackthon.Domain.Projetos;
using Hackthon.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackthon.Infrastructure.Persistencia.Configuracoes;

public sealed class ProjetoConfiguracao : IEntityTypeConfiguration<Projeto>
{
    public void Configure(EntityTypeBuilder<Projeto> builder)
    {
        builder.ToTable("projetos");
        builder.HasKey(projeto => projeto.Id);

        builder.Property(projeto => projeto.Protocolo).HasMaxLength(30).IsRequired();
        builder.HasIndex(projeto => projeto.Protocolo).IsUnique();
        builder.Property(projeto => projeto.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(projeto => projeto.Setor).HasMaxLength(100).IsRequired();
        builder.Property(projeto => projeto.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasIndex(projeto => projeto.Status);
        builder.Property(projeto => projeto.VersaoEnvio).IsRequired().HasDefaultValue(1);

        // Documento
        builder.Property(projeto => projeto.NomeArquivoOriginal).HasMaxLength(255).IsRequired();
        builder.Property(projeto => projeto.CaminhoArquivo).HasMaxLength(500).IsRequired();
        builder.Property(projeto => projeto.TamanhoBytes).IsRequired();
        builder.Property(projeto => projeto.TextoExtraido).HasColumnType("text").IsRequired();

        // Avaliação da IA
        builder.Property(projeto => projeto.ResumoIa).HasColumnType("text");
        builder.Property(projeto => projeto.ParecerIa).HasColumnType("text");
        builder.Property(projeto => projeto.RecomendacaoIa).HasMaxLength(60);
        builder.Property(projeto => projeto.JustificativasIaJson).HasColumnType("jsonb");
        builder.Property(projeto => projeto.OrigemAnaliseIa).HasMaxLength(120);

        // Deliberação do analista
        builder.Property(projeto => projeto.JustificativaAnalista).HasMaxLength(4000);

        // Rastreabilidade da classificação (4 classes do desafio)
        builder.Property(projeto => projeto.RecorteSustentado).HasColumnType("text");
        builder.Property(projeto => projeto.LimitacaoTecnica).HasColumnType("text");
        builder.Property(projeto => projeto.EvidenciaNecessaria).HasColumnType("text");
        builder.Property(projeto => projeto.EloAusente).HasColumnType("text");
        builder.Property(projeto => projeto.EvidenciasAdicionais).HasColumnType("text");
        builder.Property(projeto => projeto.EvidenciasUtilizadas).HasColumnType("text");
        builder.Property(projeto => projeto.EvidenciasContrarias).HasColumnType("text");
        builder.Property(projeto => projeto.Lacunas).HasColumnType("text");
        builder.Property(projeto => projeto.RegraAplicada).HasColumnType("text");

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(projeto => projeto.AnalistaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(projeto => projeto.ProponenteId).IsRequired();
        builder.HasIndex(projeto => projeto.ProponenteId);
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(projeto => projeto.ProponenteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(projeto => projeto.CriadoEm).IsRequired();
        builder.Property(projeto => projeto.AtualizadoEm).IsRequired();

        builder.HasMany(projeto => projeto.Arquivos)
            .WithOne()
            .HasForeignKey(a => a.ProjetoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(projeto => projeto.Arquivos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
