# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PortfolioApi.csproj", "./"]
RUN dotnet restore "PortfolioApi.csproj"

COPY . .
RUN dotnet publish "PortfolioApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 10000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PortfolioApi.dll"]