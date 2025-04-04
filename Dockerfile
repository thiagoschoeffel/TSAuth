FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY TSAuth.sln ./
COPY TSAuth.Api/*.csproj TSAuth.Api/
RUN dotnet restore

COPY . ./
WORKDIR /app/TSAuth.Api
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/TSAuth.Api/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "TSAuth.Api.dll"]