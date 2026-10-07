using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct);
    Task<Usuario?> ObterPorCpfCnpjAsync(string cpfCnpj, CancellationToken ct);
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Usuario>> ListarAsync(CancellationToken ct);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct);
    Task<int> ContarAdmAtivosExcetoAsync(Guid usuarioId, CancellationToken ct);
}

public interface IRegistroAcessoRepositorio
{
    Task AdicionarAsync(RegistroAcesso registro, CancellationToken ct);
}

public interface IUnidadeDeTrabalho
{
    Task SalvarAsync(CancellationToken ct);
    Task ExecutarEmTransacaoAsync(Func<CancellationToken, Task> trabalho, CancellationToken ct);
}

public interface ISenhaHasher
{
    string GerarHash(string senha);
    bool Conferir(string senhaHash, string senha);
}

public interface IEmissorToken
{
    TokenEmitido Emitir(Usuario usuario, DateTimeOffset agora);
}
