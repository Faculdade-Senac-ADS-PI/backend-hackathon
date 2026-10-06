namespace Hackthon.Domain.Usuarios;

public sealed class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string SenhaHash { get; private set; } = null!;
    public Papel Papel { get; private set; }
    public bool Ativo { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset AtualizadoEm { get; private set; }

    private Usuario()
    {
    }

    public static Usuario Criar(string nome, string email, string senhaHash, Papel papel, DateTimeOffset agora)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório.", nameof(nome));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.", nameof(email));
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("A senha precisa estar protegida antes de gravar.", nameof(senhaHash));

        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            SenhaHash = senhaHash,
            Papel = papel,
            Ativo = true,
            CriadoEm = agora,
            AtualizadoEm = agora
        };
    }

    public void AlterarPapel(Papel papel, DateTimeOffset agora)
    {
        Papel = papel;
        AtualizadoEm = agora;
    }

    public void Desativar(DateTimeOffset agora)
    {
        Ativo = false;
        AtualizadoEm = agora;
    }

    public void Reativar(DateTimeOffset agora)
    {
        Ativo = true;
        AtualizadoEm = agora;
    }
}
