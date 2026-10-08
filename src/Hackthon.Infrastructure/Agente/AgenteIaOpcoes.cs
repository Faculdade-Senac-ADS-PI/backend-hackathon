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

    public string Provedor { get; set; } = "Heuristico";
    public string UrlBase { get; set; } = "https://api.openai.com/v1/";
    public string? ApiKey { get; set; }
    public string Modelo { get; set; } = "gpt-4o-mini";
    public double Temperatura { get; set; } = 0.1;
    public int TimeoutSegundos { get; set; } = 90;
    public int MaxCaracteresDocumento { get; set; } = 60000;

    /// <summary>Envia response_format=json_object. Desligue se o provedor não suportar.</summary>
    public bool ExigirRespostaJson { get; set; } = true;

    public bool UsaLlm => Provedor.Equals("OpenAICompativel", StringComparison.OrdinalIgnoreCase);
}
