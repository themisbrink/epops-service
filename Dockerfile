# ---------------- BUILD STAGE ----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore first (faster caching)
COPY EpopsService.csproj ./
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Publish the application to /app/publish
RUN dotnet publish -c Release -o /out

# ---------------- RUNTIME STAGE ----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published application
COPY --from=build /out .

EXPOSE 80

ENTRYPOINT ["dotnet", "EpopsService.dll"]
