#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Ora.Services.Configurable/Ora.Services.Configurable.csproj", "Ora.Services.Configurable/"]
COPY ["Ora.Common/Ora.Common.csproj", "Ora.Common/"]
COPY ["Ora.Common.Commands/Ora.Common.Commands.csproj", "Ora.Common.Commands/"]
RUN dotnet restore "Ora.Services.Configurable/Ora.Services.Configurable.csproj"
COPY . .
WORKDIR "/src/Ora.Services.Configurable"
RUN dotnet build "Ora.Services.Configurable.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ora.Services.Configurable.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ora.Services.Configurable.dll"]