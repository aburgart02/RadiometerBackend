FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RadiometerWebApp/RadiometerWebApp.csproj", "RadiometerWebApp/"]
RUN dotnet restore "RadiometerWebApp/RadiometerWebApp.csproj"
COPY . .
WORKDIR "/src/RadiometerWebApp"
RUN dotnet build "RadiometerWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RadiometerWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RadiometerWebApp.dll"]
