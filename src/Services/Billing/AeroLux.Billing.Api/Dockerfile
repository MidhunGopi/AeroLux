FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Billing/AeroLux.Billing.Api/AeroLux.Billing.Api.csproj", "src/Services/Billing/AeroLux.Billing.Api/"]
COPY ["src/Services/Billing/AeroLux.Billing.Infrastructure/AeroLux.Billing.Infrastructure.csproj", "src/Services/Billing/AeroLux.Billing.Infrastructure/"]
COPY ["src/Services/Billing/AeroLux.Billing.Application/AeroLux.Billing.Application.csproj", "src/Services/Billing/AeroLux.Billing.Application/"]
COPY ["src/Services/Billing/AeroLux.Billing.Domain/AeroLux.Billing.Domain.csproj", "src/Services/Billing/AeroLux.Billing.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Billing/AeroLux.Billing.Api/AeroLux.Billing.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Billing/AeroLux.Billing.Api"
RUN dotnet build "AeroLux.Billing.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Billing.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Billing.Api.dll"]
