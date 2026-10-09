using Hackthon.Domain.Acesso;
using Hackthon.Domain.Projetos;
using Hackthon.Domain.Usuarios;
using Hackthon.Infrastructure.Normas;
using Microsoft.EntityFrameworkCore;

namespace Hackthon.Infrastructure.Persistencia;

public sealed class HackthonDbContext : DbContext
{
    public HackthonDbContext(DbContextOptions<HackthonDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RegistroAcesso> RegistrosAcesso => Set<RegistroAcesso>();
    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<ArquivoProjetoItem> ArquivosProjeto => Set<ArquivoProjetoItem>();
    public DbSet<DocumentoNormativo> DocumentosNormativos => Set<DocumentoNormativo>();
    public DbSet<TrechoNormativo> TrechosNormativos => Set<TrechoNormativo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HackthonDbContext).Assembly);
    }
}
