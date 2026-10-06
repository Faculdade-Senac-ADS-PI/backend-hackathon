using Hackthon.Application.Comum;

namespace Hackthon.Infrastructure.Auth;

public sealed class RelogioSistema : IRelogio
{
    public DateTimeOffset Agora() => DateTimeOffset.UtcNow;
}
