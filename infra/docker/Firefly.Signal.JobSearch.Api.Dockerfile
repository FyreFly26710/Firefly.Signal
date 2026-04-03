FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .
RUN dotnet restore "services/api/src/Firefly.Signal.JobSearch.Api/Firefly.Signal.JobSearch.Api.csproj"
RUN dotnet publish "services/api/src/Firefly.Signal.JobSearch.Api/Firefly.Signal.JobSearch.Api.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Firefly.Signal.JobSearch.Api.dll"]
