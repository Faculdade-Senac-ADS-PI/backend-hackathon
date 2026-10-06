using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackthon.Infrastructure.Persistencia.Configuracoes;

public sealed class RegistroAcessoConfiguracao : IEntityTypeConfiguration<RegistroAcesso>
{
    public void Configure(EntityTypeBuilder<RegistroAcesso> builder)
    {
        builder.ToTable("registros_acesso");
        builder.HasKey(registro => registro.Id);
        builder.Property(registro => registro.Acao).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(registro => registro.Detalhe).HasMaxLength(500);
        builder.Property(registro => registro.CriadoEm).IsRequired();
        builder.HasIndex(registro => registro.UsuarioId);
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(registro => registro.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
