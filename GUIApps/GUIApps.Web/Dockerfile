FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
# The context must be set to the GUIApps directory to access all related projects
COPY ["GUIApps/GUIApps.Web/GUIApps.Web.csproj", "GUIApps/GUIApps.Web/"]
COPY ["GUIApps/GUIApps.Web.Client/GUIApps.Web.Client.csproj", "GUIApps/GUIApps.Web.Client/"]
COPY ["GUIApps/GUIApps.Shared/GUIApps.Shared.csproj", "GUIApps/GUIApps.Shared/"]

RUN dotnet restore "GUIApps/GUIApps.Web/GUIApps.Web.csproj"

# Copy the full source code
COPY . .

# Build and publish
WORKDIR "/src/GUIApps/GUIApps.Web"
RUN dotnet publish "GUIApps.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "GUIApps.Web.dll"]
