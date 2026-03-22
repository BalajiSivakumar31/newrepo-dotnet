# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY . .
RUN dotnet publish dotnet-folder.csproj -c Release -o /app/publish

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy AS final
WORKDIR /app

# Set ASP.NET to listen on Cloud Run port
ENV ASPNETCORE_URLS=http://+:8080

# Create non-root user
ARG USERNAME=devops-non
ARG USER_UID=1000
ARG USER_GID=1000

RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
    && apt-get update \
    && apt-get install -y ca-certificates \
    && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Copy entrypoint
COPY backendentrypoint.sh .
RUN chmod +x backendentrypoint.sh

# Set ownership
RUN chown -R $USERNAME:$USERNAME /app

# Switch to non-root user
USER $USERNAME

# Expose Cloud Run port
EXPOSE 8080

# Start app
ENTRYPOINT ["./backendentrypoint.sh"]
