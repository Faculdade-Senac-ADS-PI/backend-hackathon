using System.Text;
using System.Text.Json.Serialization;
using Hackthon.Api.Auth;
using Hackthon.Application.Acesso;
using Hackthon.Application.Projetos;
using Hackthon.Infrastructure;
using Hackthon.Infrastructure.Auth;
using Hackthon.Infrastructure.Persistencia;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    if (string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Secret"]))
        builder.Configuration["Jwt:Secret"] = SegredoDesenvolvimento.Jwt;
    if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("Postgres")))
        builder.Configuration["ConnectionStrings:Postgres"] = SegredoDesenvolvimento.Postgres;
}

builder.Services.AddControllers()
    .AddJsonOptions(opcoes =>
    {
        opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        opcoes.InvalidModelStateResponseFactory = _ =>
            new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Pedido inválido",
                Detail = "Confira os campos enviados."
            });
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hackthon",
        Version = "v1",
        Description = "Acesso dos papéis e o contrato do agente de elegibilidade da Lei do Bem."
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole o token emitido por POST /api/auth/entrar."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfraestrutura(builder.Configuration);
builder.Services.AddScoped<RegistrarUsuarioServico>();
builder.Services.AddScoped<IRegistrarUsuario>(sp => sp.GetRequiredService<RegistrarUsuarioServico>());
builder.Services.AddScoped<IRegistrarProponente>(sp => sp.GetRequiredService<RegistrarUsuarioServico>());
builder.Services.AddScoped<IEntrar, EntrarServico>();
builder.Services.AddScoped<IObterPerfil, ObterPerfilServico>();
builder.Services.AddScoped<IListarUsuarios, ListarUsuariosServico>();
builder.Services.AddScoped<ICriarUsuario, CriarUsuarioServico>();
builder.Services.AddScoped<IAlterarPapel, AlterarPapelServico>();
builder.Services.AddScoped<IAlterarAtivacao, AlterarAtivacaoServico>();
builder.Services.AddScoped<IEnviarProposta, EnviarPropostaServico>();
builder.Services.AddScoped<IConsultarProjetos, ConsultarProjetosServico>();
builder.Services.AddScoped<IDeliberarProjeto, DeliberarProjetoServico>();

var jwt = builder.Configuration.GetSection(JwtOpcoes.Secao).Get<JwtOpcoes>() ?? new JwtOpcoes();
if (string.IsNullOrWhiteSpace(jwt.Secret) || jwt.Secret.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Secret precisa ter pelo menos 32 caracteres. Em desenvolvimento o fallback está em appsettings.Development.json.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.MapInboundClaims = false;
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = "papel",
            NameClaimType = "nome"
        };
        opcoes.Events = new JwtBearerEvents
        {
            OnMessageReceived = contexto =>
            {
                if (string.IsNullOrEmpty(contexto.Token)
                    && contexto.Request.Cookies.TryGetValue(SessaoCookie.Nome, out var cookie)
                    && !string.IsNullOrWhiteSpace(cookie))
                {
                    contexto.Token = Uri.UnescapeDataString(cookie);
                }

                return Task.CompletedTask;
            },
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();
                contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
                contexto.Response.ContentType = "application/problem+json";
                await contexto.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Não autenticado",
                    Detail = "Entre de novo para continuar."
                });
            },
            OnForbidden = async contexto =>
            {
                contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
                contexto.Response.ContentType = "application/problem+json";
                await contexto.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Acesso negado",
                    Detail = "Seu papel não pode fazer esta operação."
                });
            }
        };
    });

builder.Services.AddAuthorization(opcoes =>
{
    opcoes.AddPolicy(Politicas.SomenteAdm, politica => politica.RequireRole("Adm"));
    opcoes.AddPolicy(Politicas.AnalistaOuAdm, politica => politica.RequireRole("Analista", "Assistente", "Adm"));
    opcoes.AddPolicy(Politicas.SomenteProponente, politica => politica.RequireRole("Proponente"));
});

var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? [];
builder.Services.AddCors(opcoes =>
{
    opcoes.AddPolicy("web", politica =>
    {
        if (origens.Length == 0)
            politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else
            politica.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandler = async contexto =>
    {
        var falha = contexto.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var logger = contexto.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Hackthon");
        logger.LogError(falha, "Falha não tratada em {Caminho}", contexto.Request.Path);
        contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
        contexto.Response.ContentType = "application/problem+json";
        await contexto.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro inesperado",
            Detail = "A operação não foi concluída. Tente de novo."
        });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("web");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment() && Environment.GetEnvironmentVariable("EF_SKIP_DB_INIT") != "1")
{
    await BancoInicial.AplicarAsync(app.Services);
    app.Logger.LogInformation("Postgres migrado. Se o banco estava vazio, as contas de desenvolvimento foram criadas.");
}

app.Run();
