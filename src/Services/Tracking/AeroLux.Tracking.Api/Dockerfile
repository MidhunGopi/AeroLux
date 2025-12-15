FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Tracking/AeroLux.Tracking.Api/AeroLux.Tracking.Api.csproj", "src/Services/Tracking/AeroLux.Tracking.Api/"]
COPY ["src/Services/Tracking/AeroLux.Tracking.Infrastructure/AeroLux.Tracking.Infrastructure.csproj", "src/Services/Tracking/AeroLux.Tracking.Infrastructure/"]
COPY ["src/Services/Tracking/AeroLux.Tracking.Application/AeroLux.Tracking.Application.csproj", "src/Services/Tracking/AeroLux.Tracking.Application/"]
COPY ["src/Services/Tracking/AeroLux.Tracking.Domain/AeroLux.Tracking.Domain.csproj", "src/Services/Tracking/AeroLux.Tracking.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Tracking/AeroLux.Tracking.Api/AeroLux.Tracking.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Tracking/AeroLux.Tracking.Api"
RUN dotnet build "AeroLux.Tracking.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Tracking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Tracking.Api.dll"]
