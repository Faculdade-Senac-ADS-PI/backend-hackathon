using Hackthon.Application.Acesso;
using Hackthon.Application.Agente;
using Hackthon.Application.Comum;
using Hackthon.Application.Normas;
using Hackthon.Application.Projetos;
using Hackthon.Infrastructure.Agente;
using Hackthon.Infrastructure.Arquivos;
using Hackthon.Infrastructure.Auth;
using Hackthon.Infrastructure.Normas;
using Hackthon.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pgvector.EntityFrameworkCore;

namespace Hackthon.Infrastructure;

public static class Dependencias
{
    public static IServiceCollection AddInfraestrutura(this IServiceCollection services, IConfiguration configuration)
    {
        var conexao = configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(conexao))
        {
            throw new InvalidOperationException(
                "Defina ConnectionStrings:Postgres. Em desenvolvimento o valor está em appsettings.Development.json.");
        }

        services.Configure<JwtOpcoes>(configuration.GetSection(JwtOpcoes.Secao));
        services.Configure<ArmazenamentoOpcoes>(configuration.GetSection(ArmazenamentoOpcoes.Secao));
        services.Configure<AgenteIaOpcoes>(configuration.GetSection(AgenteIaOpcoes.Secao));
        services.Configure<RagOpcoes>(configuration.GetSection(RagOpcoes.Secao));

        services.AddDbContext<HackthonDbContext>(opcoes =>
            opcoes.UseNpgsql(conexao, npgsql => npgsql.UseVector()));
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IRegistroAcessoRepositorio, RegistroAcessoRepositorio>();
        services.AddScoped<IProjetoRepositorio, ProjetoRepositorio>();
        services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        services.AddSingleton<ISenhaHasher, SenhaHasher>();
        services.AddSingleton<IRelogio, RelogioSistema>();
        services.AddSingleton<IEmissorToken, EmissorToken>();

        // Armazenamento de arquivos e extração de PDF e CSV
        services.AddSingleton<IArmazenamentoArquivos, ArmazenamentoLocalArquivos>();
        services.AddSingleton<IExtratorTextoPdf, ExtratorTextoPdfPig>();
        services.AddSingleton<IExtratorTextoArquivo, ExtratorTextoComposto>();

        // Agente de IA para avaliação dos 5 critérios Frascati / Lei do Bem
        services.AddHttpClient();
        services.AddSingleton<AnalisadorElegibilidadeHeuristico>();
        services.AddScoped<AnalisadorElegibilidadeLlm>();
        services.AddScoped<IAnalisadorElegibilidade, AnalisadorElegibilidadeLlm>();

        // RAG: base de normas (pgvector) para apoiar a avaliação e a consulta do analista
        services.AddSingleton<IFonteConhecimento, FonteConhecimentoLocal>();
        services.AddScoped<INormaRepositorio, NormaRepositorio>();
        services.AddHttpClient<IGeradorEmbedding, GeradorEmbeddingGemini>();
        services.AddScoped<IRecuperadorTrechos, RecuperadorTrechos>();
        services.AddScoped<IIndexarBase, IndexarBaseServico>();
        services.AddScoped<IConsultarBase, ConsultarBaseServico>();

        // Agente de apoio ao analista (consulta em linguagem natural com citações)
        services.AddScoped<IAgenteElegibilidade, AgenteElegibilidadeRag>();

        return services;
    }
}
