#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NetConnMon/NetConnMon.csproj", "NetConnMon/"]
COPY ["NetConnMon.Domain/NetConnMon.Domain.csproj", "NetConnMon.Domain/"]
COPY ["NetConnMon.Server/NetConnMon.Server.csproj", "NetConnMon.Server/"]
COPY ["NetConnMon.Persistence/NetConnMon.Persistence.csproj", "NetConnMon.Persistence/"]
RUN dotnet restore "NetConnMon/NetConnMon.csproj"
COPY . .
WORKDIR "/src/NetConnMon"
RUN dotnet build "NetConnMon.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetConnMon.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetConnMon.dll"]