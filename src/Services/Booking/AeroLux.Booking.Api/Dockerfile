FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Services/Booking/AeroLux.Booking.Api/AeroLux.Booking.Api.csproj", "src/Services/Booking/AeroLux.Booking.Api/"]
COPY ["src/Services/Booking/AeroLux.Booking.Infrastructure/AeroLux.Booking.Infrastructure.csproj", "src/Services/Booking/AeroLux.Booking.Infrastructure/"]
COPY ["src/Services/Booking/AeroLux.Booking.Application/AeroLux.Booking.Application.csproj", "src/Services/Booking/AeroLux.Booking.Application/"]
COPY ["src/Services/Booking/AeroLux.Booking.Domain/AeroLux.Booking.Domain.csproj", "src/Services/Booking/AeroLux.Booking.Domain/"]
COPY ["src/Shared/AeroLux.Shared.Kernel/AeroLux.Shared.Kernel.csproj", "src/Shared/AeroLux.Shared.Kernel/"]
COPY ["src/Shared/AeroLux.Shared.Infrastructure/AeroLux.Shared.Infrastructure.csproj", "src/Shared/AeroLux.Shared.Infrastructure/"]
RUN dotnet restore "src/Services/Booking/AeroLux.Booking.Api/AeroLux.Booking.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Booking/AeroLux.Booking.Api"
RUN dotnet build "AeroLux.Booking.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeroLux.Booking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeroLux.Booking.Api.dll"]
