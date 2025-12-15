FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/FlightControl/AeroLux.FlightControl.Api/AeroLux.FlightControl.Api.csproj", "src/Services/FlightControl/AeroLux.FlightControl.Api/"]
COPY ["src/Services/FlightControl/AeroLux.FlightControl.Infrastructure/AeroLux.FlightControl.Infrastructure.csproj", "src/Services/FlightControl/AeroLux.FlightControl.Infrastructure/"]
COPY ["src/Services/FlightControl/AeroLux.FlightControl.Application/AeroLux.FlightControl.Application.csproj", "src/Services/FlightControl/AeroLux.FlightControl.Application/"]
COPY ["src/Services/FlightControl/AeroLux.FlightControl.Domain/AeroLux.FlightControl.Domain.csproj", "src/Services/FlightControl/AeroLux.FlightControl.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/FlightControl/AeroLux.FlightControl.Api/AeroLux.FlightControl.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/FlightControl/AeroLux.FlightControl.Api"
RUN dotnet build "AeroLux.FlightControl.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.FlightControl.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.FlightControl.Api.dll"]
