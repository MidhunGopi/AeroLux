FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Fleet/AeroLux.Fleet.Api/AeroLux.Fleet.Api.csproj", "src/Services/Fleet/AeroLux.Fleet.Api/"]
COPY ["src/Services/Fleet/AeroLux.Fleet.Infrastructure/AeroLux.Fleet.Infrastructure.csproj", "src/Services/Fleet/AeroLux.Fleet.Infrastructure/"]
COPY ["src/Services/Fleet/AeroLux.Fleet.Application/AeroLux.Fleet.Application.csproj", "src/Services/Fleet/AeroLux.Fleet.Application/"]
COPY ["src/Services/Fleet/AeroLux.Fleet.Domain/AeroLux.Fleet.Domain.csproj", "src/Services/Fleet/AeroLux.Fleet.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Fleet/AeroLux.Fleet.Api/AeroLux.Fleet.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Fleet/AeroLux.Fleet.Api"
RUN dotnet build "AeroLux.Fleet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Fleet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Fleet.Api.dll"]
