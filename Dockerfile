# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy all source (including server.sln and all .csproj/.cs files)
COPY . .

# restore the entire solution
RUN dotnet restore server.sln

# publish the WebAPI project
RUN dotnet publish WebAPI/WebAPI.csproj -c Release -o /app/out

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# have Kestrel listen on port 80
ENV ASPNETCORE_URLS=http://+:80

# copy the published output
COPY --from=build /app/out ./

EXPOSE 80
ENTRYPOINT ["dotnet", "WebAPI.dll"]