using Hackthon.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hackthon.Infrastructure.Persistencia;

public sealed class HackthonDbContextFactory : IDesignTimeDbContextFactory<HackthonDbContext>
{
    public HackthonDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<HackthonDbContext>()
            .UseNpgsql(SegredoDesenvolvimento.Postgres)
            .Options;
        return new HackthonDbContext(options);
    }
}
