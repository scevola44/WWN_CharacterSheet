# Stage 1: Build Node/React frontend
FROM node:22-alpine AS frontend-builder

WORKDIR /src/WWN.Web/client-app

COPY src/WWN.Web/client-app/package*.json ./

RUN npm ci

COPY src/WWN.Web/client-app .

RUN npm run build

# Stage 2: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS dotnet-builder

WORKDIR /src

# Copy solution and project files
COPY WWN_CharacterSheet.sln .
COPY src/WWN.Domain/WWN.Domain.csproj ./src/WWN.Domain/
COPY src/WWN.Application/WWN.Application.csproj ./src/WWN.Application/
COPY src/WWN.Infrastructure/WWN.Infrastructure.csproj ./src/WWN.Infrastructure/
COPY src/WWN.Web/WWN.Web.csproj ./src/WWN.Web/

# Restore dependencies
RUN dotnet restore WWN_CharacterSheet.sln

# Copy source code
COPY src ./src

# Copy built frontend assets into wwwroot
COPY --from=frontend-builder /src/WWN.Web/client-app/dist ./src/WWN.Web/wwwroot

# Build and publish
RUN dotnet publish src/WWN.Web/WWN.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

WORKDIR /app

# Copy published application
COPY --from=dotnet-builder /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "WWN.Web.dll"]
