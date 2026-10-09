namespace Hackthon.Infrastructure.Agente;

/// <summary>
/// Configuração do agente de IA (seção "AgenteIa" do appsettings ou variáveis AgenteIa__*).
///
/// Provedor:
///  - "Heuristico"       → sem chamada externa; usa o motor de palavras-chave (padrão).
///  - "OpenAICompativel" → chama POST {UrlBase}chat/completions. Funciona com OpenAI, Google Gemini
///                         (endpoint compatível), Groq, OpenRouter, Ollama, LM Studio etc.
///
/// NUNCA versione a ApiKey. Use: export AgenteIa__ApiKey="..." ou dotnet user-secrets.
/// </summary>
public sealed class AgenteIaOpcoes
{
    public const string Secao = "AgenteIa";

    public string Provedor { get; set; } = "OpenAICompativel";

    public string UrlBase
    {
        get => _urlBase;
        set => _urlBase = value.EndsWith('/') ? value : value + "/";
    }
    private string _urlBase = "https://api.openai.com/v1/";

    /// <summary>Alias aceito em appsettings.json para compatibilidade.</summary>
    public string Endpoint
    {
        get => UrlBase;
        set => UrlBase = value;
    }

    public string? ApiKey { get; set; }
    public string Modelo { get; set; } = "gemini-1.5-flash";
    public double Temperatura { get; set; } = 0.1;
    public int TimeoutSegundos { get; set; } = 90;
    public int MaxCaracteresDocumento { get; set; } = 60000;

    /// <summary>Tentativas para falhas transitórias do provedor (429/5xx) antes de recuar.</summary>
    public int Tentativas { get; set; } = 3;

    /// <summary>Envia response_format=json_object. Desligue se o provedor não suportar.</summary>
    public bool ExigirRespostaJson { get; set; } = true;

    public bool UsaLlm => Provedor.Equals("OpenAICompativel", StringComparison.OrdinalIgnoreCase);
}
