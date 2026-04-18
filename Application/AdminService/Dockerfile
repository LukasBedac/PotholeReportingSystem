FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["Application/AdminService/AdminService.csproj", "Application/AdminService/"]
COPY ["GUIApps/GUIApps.Shared/GUIApps.Shared.csproj", "GUIApps/GUIApps.Shared/"]
RUN dotnet restore "Application/AdminService/AdminService.csproj"

# Copy everything and publish
COPY . .
WORKDIR "/src/Application/AdminService"
RUN dotnet publish "AdminService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AdminService.dll"]
