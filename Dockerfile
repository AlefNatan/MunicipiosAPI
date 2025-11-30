# BASE (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# BUILD IMAGE
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar csprojs
COPY MunicipiosAPI/MunicipiosAPI.csproj MunicipiosAPI/
COPY MunicipiosAPI.Domain/MunicipiosAPI.Domain.csproj MunicipiosAPI.Domain/
COPY MunicipiosAPI.Providers/MunicipiosAPI.Providers.csproj MunicipiosAPI.Providers/
COPY MunicipiosAPI.Service/MunicipiosAPI.Service.csproj MunicipiosAPI.Service/

# Restore
RUN dotnet restore MunicipiosAPI/MunicipiosAPI.csproj

# Copiar tudo
COPY . .

# Build + Publish
RUN dotnet publish MunicipiosAPI/MunicipiosAPI.csproj -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MunicipiosAPI.dll"]
