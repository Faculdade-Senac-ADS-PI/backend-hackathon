using Hackthon.Application.Agente;
using Hackthon.Application.Comum;

namespace Hackthon.Infrastructure.Agente;

/// <summary>
/// Implementação de espera. Não chama modelo nenhum e não lê vetor.
/// Troque este registro no DI por uma classe que consulte pgvector e um provedor de LLM.
/// </summary>
public sealed class AgenteElegibilidadeNaoConfigurado : IAgenteElegibilidade
{
    public ContratoAgente DescreverContrato() => new(
        Estado: "nao-configurado",
        Responsabilidade: "Apoiar o analista com trechos das normas. Não decidir se o projeto é pesquisa.",
        ComoPlugar: "Crie uma classe em Hackthon.Infrastructure/Agente que implemente IAgenteElegibilidade, grave embeddings com a extensão pgvector (já habilitada no Postgres, sem índice) e troque o registro em Dependencias. Nenhuma chave de IA é exigida para o acesso dos papéis.",
        EntradasPrevistas:
        [
            "projetoId",
            "pergunta do analista",
            "trechos do material do projeto"
        ],
        Saida: "resposta de apoio, citações (documento, trecho, referência) e decisaoAutomatica sempre false.");

    public Task<Resultado<ConsultaAgenteResposta>> ConsultarAsync(ConsultaAgenteComando comando, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(comando.Pergunta))
        {
            return Task.FromResult(Resultado<ConsultaAgenteResposta>.Falha(
                400,
                "Consulta incompleta",
                "Informe a pergunta que o analista quer fazer às normas."));
        }

        return Task.FromResult(Resultado<ConsultaAgenteResposta>.Falha(
            501,
            "Agente ainda não ligado",
            "A consulta às normas (RAG com pgvector) é o ponto de extensão. Troque AgenteElegibilidadeNaoConfigurado pela implementação real. O acesso dos papéis não depende de chave de IA."));
    }
}
