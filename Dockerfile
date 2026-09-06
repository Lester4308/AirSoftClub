FROM mcr.microsoft.com/dotnet/sdk:10.0.302 AS build
WORKDIR /work
COPY global.json Directory.Build.props ./
COPY src ./src
RUN dotnet restore src/Airsoft.Server/Airsoft.Server.csproj --locked-mode
RUN dotnet publish src/Airsoft.Server/Airsoft.Server.csproj -c Release --no-restore -o /app
FROM mcr.microsoft.com/dotnet/aspnet:10.0.11
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
USER app
ENTRYPOINT ["dotnet", "Airsoft.Server.dll"]
