using Hackthon.Application.Comum;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

public interface IAlterarAtivacao
{
    Task<Resultado<UsuarioResumo>> ExecutarAsync(AlterarAtivacaoComando comando, CancellationToken ct);
}

public sealed class AlterarAtivacaoServico : IAlterarAtivacao
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRegistroAcessoRepositorio _registros;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly IRelogio _relogio;

    public AlterarAtivacaoServico(
        IUsuarioRepositorio usuarios,
        IRegistroAcessoRepositorio registros,
        IUnidadeDeTrabalho unidade,
        IRelogio relogio)
    {
        _usuarios = usuarios;
        _registros = registros;
        _unidade = unidade;
        _relogio = relogio;
    }

    public async Task<Resultado<UsuarioResumo>> ExecutarAsync(AlterarAtivacaoComando comando, CancellationToken ct)
    {
        var usuario = await _usuarios.ObterPorIdAsync(comando.UsuarioId, ct);
        if (usuario is null)
            return Resultado<UsuarioResumo>.Falha(404, "Conta não encontrada", "Não há uma pessoa com este identificador.");

        if (usuario.Ativo == comando.Ativo)
            return Resultado<UsuarioResumo>.Ok(UsuarioMapeador.Resumir(usuario));

        if (!comando.Ativo && usuario.Id == comando.AtorId)
        {
            return Resultado<UsuarioResumo>.Falha(
                409,
                "Operação recusada",
                "Você não pode desativar a própria conta.");
        }

        var agora = _relogio.Agora();

        try
        {
            await _unidade.ExecutarEmTransacaoAsync(async token =>
            {
                if (!comando.Ativo && usuario.Papel == Papel.Adm)
                {
                    var outros = await _usuarios.ContarAdmAtivosExcetoAsync(usuario.Id, token);
                    if (outros == 0)
                    {
                        throw new RegraDeNegocioException(
                            409,
                            "ADM obrigatório",
                            "O Hackthon precisa de pelo menos um ADM ativo.");
                    }
                }

                if (comando.Ativo)
                    usuario.Reativar(agora);
                else
                    usuario.Desativar(agora);

                await _registros.AdicionarAsync(
                    RegistroAcesso.Criar(
                        usuario.Id,
                        comando.AtorId,
                        comando.Ativo ? AcaoAcesso.Reativacao : AcaoAcesso.Desativacao,
                        comando.Ativo ? "Conta reativada" : "Conta desativada",
                        agora),
                    token);
            }, ct);
        }
        catch (Exception ex) when (ex is ConflitoDeEmailException or RegraDeNegocioException)
        {
            return Falhas.Traduzir<UsuarioResumo>(ex);
        }

        return Resultado<UsuarioResumo>.Ok(UsuarioMapeador.Resumir(usuario));
    }
}
