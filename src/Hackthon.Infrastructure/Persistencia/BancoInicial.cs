using Hackthon.Application.Acesso;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hackthon.Infrastructure.Persistencia;

public static class BancoInicial
{
    public static async Task AplicarAsync(IServiceProvider services, CancellationToken ct = default)
    {
        await using var escopo = services.CreateAsyncScope();
        var db = escopo.ServiceProvider.GetRequiredService<HackthonDbContext>();
        var senhas = escopo.ServiceProvider.GetRequiredService<ISenhaHasher>();
        await db.Database.MigrateAsync(ct);
        await SemeadorDesenvolvimento.SemearAsync(db, senhas, ct);
    }
}
