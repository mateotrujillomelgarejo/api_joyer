FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo los .csproj primero (mejora cacheo de capas)
COPY api_joyeria.Api/api_joyeria.Api.csproj api_joyeria.Api/
COPY api_joyeria.Domain/api_joyeria.Domain.csproj api_joyeria.Domain/
COPY api_joyeria.Application/api_joyeria.Application.csproj api_joyeria.Application/
COPY api_joyeria.Infrastructure/api_joyeria.Infrastructure.csproj api_joyeria.Infrastructure/

RUN dotnet restore api_joyeria.Api/api_joyeria.Api.csproj

# Copiar el resto del código
COPY . .

RUN dotnet publish api_joyeria.Api/api_joyeria.Api.csproj -c Release -o /app/out

FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "api_joyeria.Api.dll"]
