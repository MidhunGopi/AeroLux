# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY AeroLux.sln ./
COPY AeroLux.Domain/*.csproj ./AeroLux.Domain/
COPY AeroLux.Application/*.csproj ./AeroLux.Application/
COPY AeroLux.Infrastructure/*.csproj ./AeroLux.Infrastructure/
COPY AeroLux.API/*.csproj ./AeroLux.API/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY AeroLux.Domain/. ./AeroLux.Domain/
COPY AeroLux.Application/. ./AeroLux.Application/
COPY AeroLux.Infrastructure/. ./AeroLux.Infrastructure/
COPY AeroLux.API/. ./AeroLux.API/

# Build application
WORKDIR /src/AeroLux.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published application
COPY --from=publish /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "AeroLux.API.dll"]
