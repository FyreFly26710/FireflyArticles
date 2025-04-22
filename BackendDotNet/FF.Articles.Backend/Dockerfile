FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for better caching
COPY ["FF.Articles.Backend.Identity.API/FF.Articles.Backend.Identity.API.csproj", "FF.Articles.Backend.Identity.API/"]
COPY ["FF.Articles.Backend.Contents.API/FF.Articles.Backend.Contents.API.csproj", "FF.Articles.Backend.Contents.API/"]
COPY ["FF.Articles.Backend.AI.API/FF.Articles.Backend.AI.API.csproj", "FF.Articles.Backend.AI.API/"]
COPY ["FF.Articles.Backend.Gateway.API/FF.Articles.Backend.Gateway.API.csproj", "FF.Articles.Backend.Gateway.API/"]
COPY ["FF.Articles.Backend.Common/FF.Articles.Backend.Common.csproj", "FF.Articles.Backend.Common/"]
COPY ["FF.Articles.Backend.ServiceDefaults/FF.Articles.Backend.ServiceDefaults.csproj", "FF.Articles.Backend.ServiceDefaults/"]
COPY ["FF.AI.Common/FF.AI.Common.csproj", "FF.AI.Common/"]
COPY ["FF.Articles.Backend.sln", "."]

# Restore as distinct layers
RUN dotnet restore "FF.Articles.Backend.Gateway.API/FF.Articles.Backend.Gateway.API.csproj"
RUN dotnet restore "FF.Articles.Backend.Identity.API/FF.Articles.Backend.Identity.API.csproj"
RUN dotnet restore "FF.Articles.Backend.Contents.API/FF.Articles.Backend.Contents.API.csproj"
RUN dotnet restore "FF.Articles.Backend.AI.API/FF.Articles.Backend.AI.API.csproj"

# Copy everything else
COPY . .

# Build and publish each service
RUN dotnet publish "FF.Articles.Backend.Gateway.API/FF.Articles.Backend.Gateway.API.csproj" -c Release -o /app/gateway
RUN dotnet publish "FF.Articles.Backend.Identity.API/FF.Articles.Backend.Identity.API.csproj" -c Release -o /app/identity
RUN dotnet publish "FF.Articles.Backend.Contents.API/FF.Articles.Backend.Contents.API.csproj" -c Release -o /app/contents
RUN dotnet publish "FF.Articles.Backend.AI.API/FF.Articles.Backend.AI.API.csproj" -c Release -o /app/ai

# Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Install required tools
RUN apt-get update && apt-get install -y curl net-tools

# Copy published apps to the final image
WORKDIR /app/gateway
COPY --from=build /app/gateway .

WORKDIR /app/identity
COPY --from=build /app/identity .

WORKDIR /app/contents
COPY --from=build /app/contents .

WORKDIR /app/ai
COPY --from=build /app/ai .

# Create and copy the startup script
WORKDIR /app
COPY scripts/start.sh .
RUN chmod +x /app/start.sh

# Expose the gateway port
EXPOSE 21000

# Start all services via the script
CMD ["/app/start.sh"] 