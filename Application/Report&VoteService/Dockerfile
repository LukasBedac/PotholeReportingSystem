FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Application/Report&VoteService/ReportVoteService.csproj", "Application/Report&VoteService/"]
COPY ["GUIApps/GUIApps.Shared/GUIApps.Shared.csproj", "GUIApps/GUIApps.Shared/"]
RUN dotnet restore "Application/Report&VoteService/ReportVoteService.csproj"

COPY . .
WORKDIR "/src/Application/Report&VoteService"
RUN dotnet publish "ReportVoteService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ReportVoteService.dll"]
