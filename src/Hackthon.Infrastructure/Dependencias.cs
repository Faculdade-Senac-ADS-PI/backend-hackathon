using Hackthon.Application.Acesso;
using Hackthon.Application.Agente;
using Hackthon.Application.Comum;
using Hackthon.Application.Projetos;
using Hackthon.Infrastructure.Agente;
using Hackthon.Infrastructure.Arquivos;
using Hackthon.Infrastructure.Auth;
using Hackthon.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddDbContext<HackthonDbContext>(opcoes => opcoes.UseNpgsql(conexao));
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

        // Agente legado para compatibilidade de endpoints anteriores
        services.AddSingleton<IAgenteElegibilidade, AgenteElegibilidadeNaoConfigurado>();

        return services;
    }
}
