FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY my-course-ms.sln ./
COPY src/SearchService/SearchService.csproj ./src/SearchService/
COPY src/Contracts/Contracts.csproj ./src/Contracts/

# Restore package deps
RUN dotnet restore ./src/SearchService/SearchService.csproj

COPY . .

RUN dotnet publish ./src/SearchService/SearchService.csproj -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "SearchService.dll" ]
