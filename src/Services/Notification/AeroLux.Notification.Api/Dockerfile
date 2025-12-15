FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Notification/AeroLux.Notification.Api/AeroLux.Notification.Api.csproj", "src/Services/Notification/AeroLux.Notification.Api/"]
COPY ["src/Services/Notification/AeroLux.Notification.Infrastructure/AeroLux.Notification.Infrastructure.csproj", "src/Services/Notification/AeroLux.Notification.Infrastructure/"]
COPY ["src/Services/Notification/AeroLux.Notification.Application/AeroLux.Notification.Application.csproj", "src/Services/Notification/AeroLux.Notification.Application/"]
COPY ["src/Services/Notification/AeroLux.Notification.Domain/AeroLux.Notification.Domain.csproj", "src/Services/Notification/AeroLux.Notification.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Notification/AeroLux.Notification.Api/AeroLux.Notification.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Notification/AeroLux.Notification.Api"
RUN dotnet build "AeroLux.Notification.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Notification.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Notification.Api.dll"]
