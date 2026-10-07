using Hackthon.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackthon.Infrastructure.Persistencia.Configuracoes;

public sealed class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(usuario => usuario.Id);
        builder.Property(usuario => usuario.Nome).HasMaxLength(160).IsRequired();
        builder.Property(usuario => usuario.Email).HasMaxLength(180).IsRequired();
        builder.HasIndex(usuario => usuario.Email).IsUnique();
        builder.Property(usuario => usuario.CpfCnpj).HasMaxLength(14).IsRequired();
        builder.HasIndex(usuario => usuario.CpfCnpj).IsUnique();
        builder.Property(usuario => usuario.SenhaHash).HasMaxLength(500).IsRequired();
        builder.Property(usuario => usuario.Papel).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(usuario => usuario.Ativo).IsRequired();
        builder.Property(usuario => usuario.CriadoEm).IsRequired();
        builder.Property(usuario => usuario.AtualizadoEm).IsRequired();
    }
}
