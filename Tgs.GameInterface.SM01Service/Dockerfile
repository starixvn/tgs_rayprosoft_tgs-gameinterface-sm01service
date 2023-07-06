#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameInterface.SM01Service/Tgs.GameInterface.SM01Service.csproj", "Tgs.GameInterface.SM01Service/"]
COPY ["Tgs.GameInterface.SM01Service.Core/Tgs.GameInterface.SM01Service.Core.csproj", "Tgs.GameInterface.SM01Service.Core/"]
RUN dotnet restore "Tgs.GameInterface.SM01Service/Tgs.GameInterface.SM01Service.csproj"
COPY . .
WORKDIR "/src/Tgs.GameInterface.SM01Service"
RUN dotnet build "Tgs.GameInterface.SM01Service.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameInterface.SM01Service.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameInterface.SM01Service.dll"]