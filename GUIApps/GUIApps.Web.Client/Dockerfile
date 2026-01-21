FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["GUIApps.Web.Client.csproj", "./"]
RUN dotnet restore "GUIApps.Web.Client.csproj"

COPY . .
RUN dotnet publish "GUIApps.Web.Client.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "GUIApps.Web.Client.dll"]
