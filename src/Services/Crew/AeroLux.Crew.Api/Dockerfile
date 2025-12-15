FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Crew/AeroLux.Crew.Api/AeroLux.Crew.Api.csproj", "src/Services/Crew/AeroLux.Crew.Api/"]
COPY ["src/Services/Crew/AeroLux.Crew.Infrastructure/AeroLux.Crew.Infrastructure.csproj", "src/Services/Crew/AeroLux.Crew.Infrastructure/"]
COPY ["src/Services/Crew/AeroLux.Crew.Application/AeroLux.Crew.Application.csproj", "src/Services/Crew/AeroLux.Crew.Application/"]
COPY ["src/Services/Crew/AeroLux.Crew.Domain/AeroLux.Crew.Domain.csproj", "src/Services/Crew/AeroLux.Crew.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Crew/AeroLux.Crew.Api/AeroLux.Crew.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Crew/AeroLux.Crew.Api"
RUN dotnet build "AeroLux.Crew.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Crew.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Crew.Api.dll"]
