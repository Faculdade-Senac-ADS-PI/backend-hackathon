namespace Hackthon.Domain.Acesso;

public sealed class RegistroAcesso
{
    public Guid Id { get; private set; }
    public Guid? UsuarioId { get; private set; }
    public Guid? AtorId { get; private set; }
    public AcaoAcesso Acao { get; private set; }
    public string? Detalhe { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }

    private RegistroAcesso()
    {
    }

    public static RegistroAcesso Criar(
        Guid? usuarioId,
        Guid? atorId,
        AcaoAcesso acao,
        string? detalhe,
        DateTimeOffset agora)
    {
        return new RegistroAcesso
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            AtorId = atorId,
            Acao = acao,
            Detalhe = string.IsNullOrWhiteSpace(detalhe) ? null : detalhe.Trim(),
            CriadoEm = agora
        };
    }
}
