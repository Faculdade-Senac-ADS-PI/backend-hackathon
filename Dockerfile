# ============================================================
# Backend Hackathon — .NET 8 API (modo produção)
# ============================================================

# ---- Stage 1: build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Manifestos primeiro para aproveitar o cache do restore
COPY global.json Directory.Build.props ./
COPY src/Hackthon.Api/Hackthon.Api.csproj src/Hackthon.Api/
COPY src/Hackthon.Application/Hackthon.Application.csproj src/Hackthon.Application/
COPY src/Hackthon.Domain/Hackthon.Domain.csproj src/Hackthon.Domain/
COPY src/Hackthon.Infrastructure/Hackthon.Infrastructure.csproj src/Hackthon.Infrastructure/
RUN dotnet restore src/Hackthon.Api/Hackthon.Api.csproj

# Código-fonte e publicação
COPY src/ src/
RUN dotnet publish src/Hackthon.Api/Hackthon.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Stage 2: runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Diretório dos uploads (recebe volume nomeado no compose)
RUN mkdir -p /app/scratch/uploads

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5088 \
    ASPNETCORE_ENVIRONMENT=Development \
    DOTNET_EnableDiagnostics=0

EXPOSE 5088

ENTRYPOINT ["dotnet", "Hackthon.Api.dll"]
