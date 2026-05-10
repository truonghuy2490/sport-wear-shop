FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/SportWearShop.APIs/SportWearShop.APIs.csproj", "SportWearShop.APIs/"]
COPY ["src/SportWearShop.BusinessLogics/SportWearShop.BusinessLogics.csproj", "SportWearShop.BusinessLogics/"]
COPY ["src/SportWearShop.Repositories/SportWearShop.Repositories.csproj", "SportWearShop.Repositories/"]

RUN dotnet restore "./SportWearShop.APIs/SportWearShop.APIs.csproj"

COPY . .
WORKDIR "/src/src/SportWearShop.APIs"
RUN dotnet publish "./SportWearShop.APIs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SportWearShop.APIs.dll"]