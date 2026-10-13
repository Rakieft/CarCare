# Build and run CarCare in a container (works on Azure Container Apps, Render, Fly.io, etc.).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY CarCare/CarCare.csproj CarCare/
RUN dotnet restore CarCare/CarCare.csproj
COPY CarCare/ CarCare/
RUN dotnet publish CarCare/CarCare.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# The SQLite database is stored in /data. Mount a volume there to keep data between deploys.
ENV ConnectionStrings__DefaultConnection="DataSource=/data/app.db;Cache=Shared"
ENV ASPNETCORE_URLS=http://+:8080
RUN mkdir -p /data
EXPOSE 8080
ENTRYPOINT ["dotnet", "CarCare.dll"]
