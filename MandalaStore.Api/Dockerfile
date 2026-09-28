FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       libfreetype6 \
       libharfbuzz0b \
       libgtk-3-0 \
       libdrm2 \
       libatomic1 \
       libx11-6 \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MandalaStore.Api/MandalaStore.Api.csproj", "MandalaStore.Api/"]
COPY ["MandalaStore.Application/MandalaStore.Application.csproj", "MandalaStore.Application/"]
COPY ["MandalaStore.Infrastructure/MandalaStore.Infrastructure.csproj", "MandalaStore.Infrastructure/"]
COPY ["MandalaStore.Domain/MandalaStore.Domain.csproj", "MandalaStore.Domain/"]

RUN dotnet restore "./MandalaStore.Api/MandalaStore.Api.csproj"

COPY . .

WORKDIR "/src/MandalaStore.Api"
RUN dotnet build "./MandalaStore.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./MandalaStore.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN mkdir -p /app/wwwroot/uploads \
    && chown -R $APP_UID:$APP_UID /app/wwwroot

USER $APP_UID

ENTRYPOINT ["dotnet", "MandalaStore.Api.dll"]