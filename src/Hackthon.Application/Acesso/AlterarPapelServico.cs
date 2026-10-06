using Hackthon.Application.Comum;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

public interface IAlterarPapel
{
    Task<Resultado<UsuarioResumo>> ExecutarAsync(AlterarPapelComando comando, CancellationToken ct);
}

public sealed class AlterarPapelServico : IAlterarPapel
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRegistroAcessoRepositorio _registros;
    private readonly IUnidadeDeTrabalho _unidade;
    private readonly IRelogio _relogio;

    public AlterarPapelServico(
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

    public async Task<Resultado<UsuarioResumo>> ExecutarAsync(AlterarPapelComando comando, CancellationToken ct)
    {
        if (!Cadastro.TentarPapel(comando.Papel, out var papel))
            return Resultado<UsuarioResumo>.Falha(400, "Papel inválido", "Escolha Proponente, Analista ou ADM.");

        var usuario = await _usuarios.ObterPorIdAsync(comando.UsuarioId, ct);
        if (usuario is null)
            return Resultado<UsuarioResumo>.Falha(404, "Conta não encontrada", "Não há uma pessoa com este identificador.");

        if (usuario.Papel == papel)
            return Resultado<UsuarioResumo>.Ok(UsuarioMapeador.Resumir(usuario));

        var agora = _relogio.Agora();
        var papelAnterior = usuario.Papel;

        try
        {
            await _unidade.ExecutarEmTransacaoAsync(async token =>
            {
                if (usuario.Ativo && papelAnterior == Papel.Adm && papel != Papel.Adm)
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

                usuario.AlterarPapel(papel, agora);
                await _registros.AdicionarAsync(
                    RegistroAcesso.Criar(
                        usuario.Id,
                        comando.AtorId,
                        AcaoAcesso.AlteracaoPapel,
                        $"Papel alterado de {papelAnterior} para {papel}",
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
