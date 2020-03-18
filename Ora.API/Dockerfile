#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Ora.API/Ora.API.csproj", "Ora.API/"]
RUN dotnet restore "Ora.API/Ora.API.csproj"
COPY . .
WORKDIR "/src/Ora.API"
RUN dotnet build "Ora.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ora.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ora.API.dll"]