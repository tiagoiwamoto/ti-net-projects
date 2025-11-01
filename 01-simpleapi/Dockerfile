FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["01-simpleapi/01-simpleapi.csproj", "01-simpleapi/"]
RUN dotnet restore "01-simpleapi/01-simpleapi.csproj"
COPY . .
WORKDIR "/src/01-simpleapi"
RUN dotnet build "./01-simpleapi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./01-simpleapi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "01-simpleapi.dll"]
