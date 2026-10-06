using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HackthonDbContext).Assembly);
    }
}
