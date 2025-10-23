#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dorisoy.Pan.API/Dorisoy.Pan.API.csproj", "Dorisoy.Pan.API/"]
COPY ["Dorisoy.Pan.Common/Dorisoy.Pan.Common.csproj", "Dorisoy.Pan.Common/"]
COPY ["Dorisoy.Pan.Data/Dorisoy.Pan.Data.csproj", "Dorisoy.Pan.Data/"]
COPY ["Dorisoy.Pan.Domain/Dorisoy.Pan.Domain.csproj", "Dorisoy.Pan.Domain/"]
COPY ["Dorisoy.Pan.Helper/Dorisoy.Pan.Helper.csproj", "Dorisoy.Pan.Helper/"]
COPY ["Dorisoy.Pan.MediatR/Dorisoy.Pan.MediatR.csproj", "Dorisoy.Pan.MediatR/"]
COPY ["Dorisoy.Pan.Repository/Dorisoy.Pan.Repository.csproj", "Dorisoy.Pan.Repository/"]

RUN dotnet restore "./Dorisoy.Pan.API/Dorisoy.Pan.API.csproj"
COPY . .
WORKDIR "/src/Dorisoy.Pan.API"
RUN dotnet build "./Dorisoy.Pan.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dorisoy.Pan.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dorisoy.Pan.API.dll"]