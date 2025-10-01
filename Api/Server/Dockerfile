FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files in correct order for layer caching
COPY ["Api/Users.Api.csproj", "Api/"]
COPY ["Api/Server/Users.Api.Server.csproj", "Api/Server/"]
COPY ["Domain/Users.Domain.csproj", "Domain/"]
COPY ["Application/Users.Application.csproj", "Application/"]
COPY ["Infrastructure/Users.Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Api/Server/Users.Api.Server.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/Api/Server"
RUN dotnet build "Users.Api.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Users.Api.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Api.Server.dll"]