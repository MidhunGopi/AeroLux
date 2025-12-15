FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Identity/AeroLux.Identity.Api/AeroLux.Identity.Api.csproj", "src/Services/Identity/AeroLux.Identity.Api/"]
COPY ["src/Services/Identity/AeroLux.Identity.Infrastructure/AeroLux.Identity.Infrastructure.csproj", "src/Services/Identity/AeroLux.Identity.Infrastructure/"]
COPY ["src/Services/Identity/AeroLux.Identity.Application/AeroLux.Identity.Application.csproj", "src/Services/Identity/AeroLux.Identity.Application/"]
COPY ["src/Services/Identity/AeroLux.Identity.Domain/AeroLux.Identity.Domain.csproj", "src/Services/Identity/AeroLux.Identity.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Identity/AeroLux.Identity.Api/AeroLux.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/AeroLux.Identity.Api"
RUN dotnet build "AeroLux.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Identity.Api.dll"]
