FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/FlightOperations/AeroLux.FlightOperations.Api/AeroLux.FlightOperations.Api.csproj", "src/Services/FlightOperations/AeroLux.FlightOperations.Api/"]
COPY ["src/Services/FlightOperations/AeroLux.FlightOperations.Infrastructure/AeroLux.FlightOperations.Infrastructure.csproj", "src/Services/FlightOperations/AeroLux.FlightOperations.Infrastructure/"]
COPY ["src/Services/FlightOperations/AeroLux.FlightOperations.Application/AeroLux.FlightOperations.Application.csproj", "src/Services/FlightOperations/AeroLux.FlightOperations.Application/"]
COPY ["src/Services/FlightOperations/AeroLux.FlightOperations.Domain/AeroLux.FlightOperations.Domain.csproj", "src/Services/FlightOperations/AeroLux.FlightOperations.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/FlightOperations/AeroLux.FlightOperations.Api/AeroLux.FlightOperations.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/FlightOperations/AeroLux.FlightOperations.Api"
RUN dotnet build "AeroLux.FlightOperations.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.FlightOperations.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.FlightOperations.Api.dll"]
