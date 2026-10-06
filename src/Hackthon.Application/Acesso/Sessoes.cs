using Hackthon.Application.Comum;
using Hackthon.Domain.Usuarios;

namespace Hackthon.Application.Acesso;

internal static class Sessoes
{
    public static SessaoResposta Emitir(IEmissorToken emissor, Usuario usuario, DateTimeOffset agora)
    {
        var emitido = emissor.Emitir(usuario, agora);
        return new SessaoResposta(emitido.Token, emitido.ExpiraEm, UsuarioMapeador.Resumir(usuario));
    }
}
