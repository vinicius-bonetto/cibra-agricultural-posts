# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["API/Cibra.AgriculturalPosts.Web.csproj", "API/"]
RUN dotnet restore "API/Cibra.AgriculturalPosts.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/API"
RUN dotnet build "Cibra.AgriculturalPosts.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Cibra.AgriculturalPosts.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cibra.AgriculturalPosts.API.dll"]