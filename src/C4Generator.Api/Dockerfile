# Build and run from the solution root:
#   docker build -f src/C4Generator.Api/Dockerfile src/ -t c4generator-api
#   docker run -p 8080:8080 --env-file .env c4generator-api

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore (layer-cached unless .csproj files change)
COPY C4Generator.Domain/C4Generator.Domain.csproj C4Generator.Domain/
COPY C4Generator.Application/C4Generator.Application.csproj C4Generator.Application/
COPY C4Generator.Infrastructure/C4Generator.Infrastructure.csproj C4Generator.Infrastructure/
COPY C4Generator.Api/C4Generator.Api.csproj C4Generator.Api/

RUN dotnet restore C4Generator.Api/C4Generator.Api.csproj

# Copy all source and publish
COPY . .
RUN dotnet publish C4Generator.Api/C4Generator.Api.csproj \
    -c Release \
    --no-restore \
    -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Run as non-root user for security
RUN addgroup --system --gid 1001 appgroup \
    && adduser --system --uid 1001 --ingroup appgroup --no-create-home appuser

COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "C4Generator.Api.dll"]
