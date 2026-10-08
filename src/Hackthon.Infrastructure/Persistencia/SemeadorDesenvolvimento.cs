using Hackthon.Application.Acesso;
using Hackthon.Domain.Acesso;
using Hackthon.Domain.Projetos;
using Hackthon.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Hackthon.Infrastructure.Persistencia;

public static class SemeadorDesenvolvimento
{
    public static readonly IReadOnlyList<ContaSemente> Contas =
    [
        new("Marina Costa", "marina.costa@hackthon.dev", "11144477735", "Proponente@2026", Papel.Proponente),
        new("Rafael Duarte", "rafael.duarte@hackthon.dev", "22255588846", "Analista@2026", Papel.Analista),
        new("Helena Borges", "helena.borges@hackthon.dev", "33366699957", "Adm@2026", Papel.Adm),
        new("Carlos Souza", "carlos.souza@hackthon.dev", "44477700083", "Assistente@2026", Papel.Assistente)
    ];

    public static async Task SemearAsync(HackthonDbContext db, ISenhaHasher senhas, CancellationToken ct)
    {
        var agora = DateTimeOffset.UtcNow;
        if (!await db.Usuarios.AnyAsync(ct))
        {
            var usuarios = new List<Usuario>();
            foreach (var conta in Contas)
            {
                usuarios.Add(Usuario.Criar(conta.Nome, conta.Email, conta.CpfCnpj, senhas.GerarHash(conta.Senha), conta.Papel, agora));
            }

            await using var transacao = await db.Database.BeginTransactionAsync(ct);
            db.Usuarios.AddRange(usuarios);
            await db.SaveChangesAsync(ct);
            db.RegistrosAcesso.AddRange(usuarios.Select(usuario =>
                RegistroAcesso.Criar(usuario.Id, null, AcaoAcesso.Criacao, "Semente de desenvolvimento", agora)));
            await db.SaveChangesAsync(ct);
            await transacao.CommitAsync(ct);
        }

        if (!await db.Projetos.AnyAsync(ct))
        {
            var proponente = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == "marina.costa@hackthon.dev", ct);
            if (proponente is not null)
            {
                var documento = new DocumentoProposta(
                    nomeOriginal: "proposta_termodinamica_2026.pdf",
                    caminho: "propostas/semente/proposta_termodinamica_2026.pdf",
                    tamanhoBytes: 245760,
                    textoExtraido: "Projeto focado em modelagem e otimização termodinâmica experimental para cadeia fria perecível com controle adaptativo.");

                var projetoExemplo = Projeto.Criar(
                    protocolo: "PROP-2026-0101",
                    titulo: "Algoritmo Heurístico de Otimização Termodinâmica para Cargas Perecíveis",
                    setor: "Engenharia de Operações",
                    proponenteId: proponente.Id,
                    documento: documento,
                    agora: agora);

                var avaliacao = new AvaliacaoFrascati(
                    notaNovidade: 18,
                    notaCriatividade: 17,
                    notaSistematico: 19,
                    notaTransferibilidade: 16,
                    notaIncerteza: 18,
                    resumo: "Proposta técnica com forte componente de incerteza e delineamento experimental estruturado, apresentando aderência sólida aos preceitos do Manual de Frascati para incentivos da Lei do Bem.",
                    parecer: "Incerteza técnica comprovada. Metodologia com delineamento experimental e hipóteses testáveis claramente evidenciadas.",
                    recomendacao: AvaliacaoFrascati.ClassificarPorNota(88),
                    justificativasJson: """
                    {
                      "novidade": "Desenvolvimento de nova formulação matemática não disponível em literatura aberta.",
                      "criatividade": "Aplicação de abordagem heurística adaptativa para controle térmico em tempo real.",
                      "sistematico": "Protocolos experimentais planejados com registros de calibração e rastreabilidade.",
                      "transferibilidade": "Resultados modelados passíveis de codificação e replicação metodológica.",
                      "incerteza": "Elevada incerteza sobre o comportamento convectivo em geometrias irregulares."
                    }
                    """,
                    origem: "semente");

                projetoExemplo.RegistrarAnaliseIa(avaliacao, agora);

                await db.Projetos.AddAsync(projetoExemplo, ct);
                await db.SaveChangesAsync(ct);
            }
        }
    }

    public sealed record ContaSemente(string Nome, string Email, string CpfCnpj, string Senha, Papel Papel);
}
