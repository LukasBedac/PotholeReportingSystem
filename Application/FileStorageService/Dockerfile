FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Application/FileStorageService/FileStorageService.csproj", "Application/FileStorageService/"]
# COPY ["GUIApps/GUIApps.Shared/GUIApps.Shared.csproj", "GUIApps/GUIApps.Shared/"] 
# Uncomment above if you add reference to Shared
RUN dotnet restore "Application/FileStorageService/FileStorageService.csproj"

COPY . .
WORKDIR "/src/Application/FileStorageService"
RUN dotnet publish "FileStorageService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FileStorageService.dll"]
