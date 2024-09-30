FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/Farmru.IotMonitoring.Web.Host/Farmru.IotMonitoring.Web.Host.csproj", "src/Farmru.IotMonitoring.Web.Host/"]
COPY ["src/Farmru.IotMonitoring.Web.Core/Farmru.IotMonitoring.Web.Core.csproj", "src/Farmru.IotMonitoring.Web.Core/"]
COPY ["src/Farmru.IotMonitoring.Application/Farmru.IotMonitoring.Application.csproj", "src/Farmru.IotMonitoring.Application/"]
COPY ["src/Farmru.IotMonitoring.Core/Farmru.IotMonitoring.Core.csproj", "src/Farmru.IotMonitoring.Core/"]
COPY ["src/Farmru.IotMonitoring.EntityFrameworkCore/Farmru.IotMonitoring.EntityFrameworkCore.csproj", "src/Farmru.IotMonitoring.EntityFrameworkCore/"]
WORKDIR "/src/src/Farmru.IotMonitoring.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/Farmru.IotMonitoring.Web.Host", "src/Farmru.IotMonitoring.Web.Host"]
COPY ["src/Farmru.IotMonitoring.Web.Core", "src/Farmru.IotMonitoring.Web.Core"]
COPY ["src/Farmru.IotMonitoring.Application", "src/Farmru.IotMonitoring.Application"]
COPY ["src/Farmru.IotMonitoring.Core", "src/Farmru.IotMonitoring.Core"]
COPY ["src/Farmru.IotMonitoring.EntityFrameworkCore", "src/Farmru.IotMonitoring.EntityFrameworkCore"]
WORKDIR "/src/src/Farmru.IotMonitoring.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Farmru.IotMonitoring.Web.Host.dll"]
