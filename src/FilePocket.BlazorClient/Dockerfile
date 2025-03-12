#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["FilePocket.BlazorClient/FilePocket.BlazorClient.csproj", "FilePocket.BlazorClient/"]
RUN dotnet restore "./FilePocket.BlazorClient/FilePocket.BlazorClient.csproj"
COPY . .
WORKDIR "/FilePocket.BlazorClient"
RUN dotnet build "./FilePocket.BlazorClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FilePocket.BlazorClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY FilePocket.BlazorClient/nginx.conf /etc/nginx/nginx.conf