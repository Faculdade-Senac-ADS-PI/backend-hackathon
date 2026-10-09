using Hackthon.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pgvector.EntityFrameworkCore;

namespace Hackthon.Infrastructure.Persistencia;

public sealed class HackthonDbContextFactory : IDesignTimeDbContextFactory<HackthonDbContext>
{
    public HackthonDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<HackthonDbContext>()
            .UseNpgsql(SegredoDesenvolvimento.Postgres, npgsql => npgsql.UseVector())
            .Options;
        return new HackthonDbContext(options);
    }
}
