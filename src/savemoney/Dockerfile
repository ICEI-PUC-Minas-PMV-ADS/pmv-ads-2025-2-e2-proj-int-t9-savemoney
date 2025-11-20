# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Stage 1: Base (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar apenas o .csproj primeiro (melhor cache)
COPY ["savemoney.csproj", "."]
RUN dotnet restore "./savemoney.csproj"

# Copiar o resto do código
COPY . .

# Build
WORKDIR "/src/."
RUN dotnet build "./savemoney.csproj" -c $BUILD_CONFIGURATION -o /app/build


# Stage 3: Publish

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./savemoney.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Stage 4: Final (Produção)

FROM base AS final
WORKDIR /app

# Copiar arquivos publicados
COPY --from=publish /app/publish .

# Entrypoint
ENTRYPOINT ["dotnet", "savemoney.dll"]