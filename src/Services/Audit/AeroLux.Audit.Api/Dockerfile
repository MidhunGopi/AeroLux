FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Audit/AeroLux.Audit.Api/AeroLux.Audit.Api.csproj", "src/Services/Audit/AeroLux.Audit.Api/"]
COPY ["src/Services/Audit/AeroLux.Audit.Infrastructure/AeroLux.Audit.Infrastructure.csproj", "src/Services/Audit/AeroLux.Audit.Infrastructure/"]
COPY ["src/Services/Audit/AeroLux.Audit.Application/AeroLux.Audit.Application.csproj", "src/Services/Audit/AeroLux.Audit.Application/"]
COPY ["src/Services/Audit/AeroLux.Audit.Domain/AeroLux.Audit.Domain.csproj", "src/Services/Audit/AeroLux.Audit.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Audit/AeroLux.Audit.Api/AeroLux.Audit.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Audit/AeroLux.Audit.Api"
RUN dotnet build "AeroLux.Audit.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Audit.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Audit.Api.dll"]
