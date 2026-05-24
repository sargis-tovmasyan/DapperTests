FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["DapperSqlLearning.Api/DapperSqlLearning.Api.csproj", "DapperSqlLearning.Api/"]
RUN dotnet restore "DapperSqlLearning.Api/DapperSqlLearning.Api.csproj"

COPY . .
WORKDIR "/src/DapperSqlLearning.Api"
RUN dotnet publish "DapperSqlLearning.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DapperSqlLearning.Api.dll"]
