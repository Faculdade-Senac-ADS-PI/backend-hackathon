using Hackthon.Application.Acesso;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Hackthon.Infrastructure.Persistencia;

public static class SemeadorDesenvolvimento
{
    public static readonly IReadOnlyList<ContaSemente> Contas =
    [
        new("Marina Costa", "marina.costa@hackthon.dev", "Proponente@2026", Papel.Proponente),
        new("Rafael Duarte", "rafael.duarte@hackthon.dev", "Analista@2026", Papel.Analista),
        new("Helena Borges", "helena.borges@hackthon.dev", "Adm@2026", Papel.Adm)
    ];

    public static async Task SemearAsync(HackthonDbContext db, ISenhaHasher senhas, CancellationToken ct)
    {
        if (await db.Usuarios.AnyAsync(ct))
            return;

        var agora = DateTimeOffset.UtcNow;
        var usuarios = new List<Usuario>();
        foreach (var conta in Contas)
        {
            usuarios.Add(Usuario.Criar(conta.Nome, conta.Email, senhas.GerarHash(conta.Senha), conta.Papel, agora));
        }

        await using var transacao = await db.Database.BeginTransactionAsync(ct);
        db.Usuarios.AddRange(usuarios);
        await db.SaveChangesAsync(ct);
        db.RegistrosAcesso.AddRange(usuarios.Select(usuario =>
            RegistroAcesso.Criar(usuario.Id, null, AcaoAcesso.Criacao, "Semente de desenvolvimento", agora)));
        await db.SaveChangesAsync(ct);
        await transacao.CommitAsync(ct);
    }

    public sealed record ContaSemente(string Nome, string Email, string Senha, Papel Papel);
}
