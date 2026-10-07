using Hackthon.Application.Acesso;
using Hackthon.Application.Comum;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Hackthon.Infrastructure.Persistencia;

public sealed class UnidadeDeTrabalho : IUnidadeDeTrabalho
{
    private readonly HackthonDbContext _db;

    public UnidadeDeTrabalho(HackthonDbContext db)
    {
        _db = db;
    }

    public async Task SalvarAsync(CancellationToken ct)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ConflitoUnico(ex, out var tipo))
        {
            if (tipo == TipoConflitoUnico.CpfCnpj)
                throw new ConflitoDeCpfCnpjException();

            throw new ConflitoDeEmailException();
        }
    }

    public async Task ExecutarEmTransacaoAsync(Func<CancellationToken, Task> trabalho, CancellationToken ct)
    {
        await using var transacao = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            await trabalho(ct);
            await SalvarAsync(ct);
            await transacao.CommitAsync(ct);
        }
        catch
        {
            await transacao.RollbackAsync(ct);
            throw;
        }
    }

    private enum TipoConflitoUnico { Email, CpfCnpj }

    private static bool ConflitoUnico(DbUpdateException ex, out TipoConflitoUnico tipo)
    {
        tipo = TipoConflitoUnico.Email;
        if (ex.InnerException is not PostgresException postgres
            || postgres.SqlState != PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }

        if (postgres.ConstraintName?.Contains("CpfCnpj", StringComparison.OrdinalIgnoreCase) == true
            || postgres.ConstraintName?.Contains("cpf_cnpj", StringComparison.OrdinalIgnoreCase) == true)
        {
            tipo = TipoConflitoUnico.CpfCnpj;
        }

        return true;
    }
}
