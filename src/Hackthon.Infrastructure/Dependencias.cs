using Hackthon.Application.Acesso;
using Hackthon.Application.Agente;
using Hackthon.Application.Comum;
using Hackthon.Infrastructure.Agente;
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
        services.AddDbContext<HackthonDbContext>(opcoes => opcoes.UseNpgsql(conexao));
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IRegistroAcessoRepositorio, RegistroAcessoRepositorio>();
        services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        services.AddSingleton<ISenhaHasher, SenhaHasher>();
        services.AddSingleton<IRelogio, RelogioSistema>();
        services.AddSingleton<IEmissorToken, EmissorToken>();
        services.AddSingleton<IAgenteElegibilidade, AgenteElegibilidadeNaoConfigurado>();
        return services;
    }
}
