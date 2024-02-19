#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY ["examples/TraefikExample/TraefikExample.csproj", "examples/TraefikExample/"]
COPY ["src/Gaaaabor.Akka.Discovery.Traefik/Gaaaabor.Akka.Discovery.Traefik.csproj", "src/Gaaaabor.Akka.Discovery.Traefik/"]

WORKDIR /src

RUN dotnet restore "examples/TraefikExample/TraefikExample.csproj"

COPY . .
WORKDIR "/src/examples/TraefikExample"
RUN dotnet build "TraefikExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraefikExample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraefikExample.dll"]