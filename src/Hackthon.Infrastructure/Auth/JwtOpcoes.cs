namespace Hackthon.Infrastructure.Auth;

public sealed class JwtOpcoes
{
    public const string Secao = "Jwt";

    public string Issuer { get; set; } = "Hackthon";
    public string Audience { get; set; } = "Hackthon.Web";
    public string Secret { get; set; } = "";
    public int ExpiraHoras { get; set; } = 8;
}

public static class SegredoDesenvolvimento
{
    public const string Jwt = "dev-only-hackthon-sts-2026-chave-com-32-bytes-min";

    public const string Postgres =
        "Host=localhost;Port=54329;Database=hackthon;Username=hackthon;Password=hackthon_dev";
}
