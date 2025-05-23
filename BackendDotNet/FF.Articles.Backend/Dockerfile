FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FF.AI.Common/FF.AI.Common.csproj ./FF.AI.Common/
COPY FF.Articles.Backend.AI.API/FF.Articles.Backend.AI.API.csproj ./FF.Articles.Backend.AI.API/
COPY FF.Articles.Backend.Common/FF.Articles.Backend.Common.csproj ./FF.Articles.Backend.Common/
COPY FF.Articles.Backend.Contents.API/FF.Articles.Backend.Contents.API.csproj ./FF.Articles.Backend.Contents.API/
COPY FF.Articles.Backend.Gateway.API/FF.Articles.Backend.Gateway.API.csproj ./FF.Articles.Backend.Gateway.API/
COPY FF.Articles.Backend.Identity.API/FF.Articles.Backend.Identity.API.csproj ./FF.Articles.Backend.Identity.API/

# Restore each project individually
RUN dotnet restore FF.Articles.Backend.Common/FF.Articles.Backend.Common.csproj
RUN dotnet restore FF.AI.Common/FF.AI.Common.csproj
RUN dotnet restore FF.Articles.Backend.AI.API/FF.Articles.Backend.AI.API.csproj
RUN dotnet restore FF.Articles.Backend.Contents.API/FF.Articles.Backend.Contents.API.csproj
RUN dotnet restore FF.Articles.Backend.Identity.API/FF.Articles.Backend.Identity.API.csproj
RUN dotnet restore FF.Articles.Backend.Gateway.API/FF.Articles.Backend.Gateway.API.csproj

# Copy the rest of the application source code
COPY . .

# Build and publish each service
RUN dotnet publish "FF.Articles.Backend.Gateway.API/FF.Articles.Backend.Gateway.API.csproj" -c Release -o /app/gateway
RUN dotnet publish "FF.Articles.Backend.Identity.API/FF.Articles.Backend.Identity.API.csproj" -c Release -o /app/identity
RUN dotnet publish "FF.Articles.Backend.Contents.API/FF.Articles.Backend.Contents.API.csproj" -c Release -o /app/contents
RUN dotnet publish "FF.Articles.Backend.AI.API/FF.Articles.Backend.AI.API.csproj" -c Release -o /app/ai

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Set default environment variables for database credentials
ENV POSTGRES_PASSWORD=postgres \
    RABBITMQ_USERNAME=rabbitmq \
    RABBITMQ_PASSWORD=rabbitmq
# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS="http://+:21000" \
    ConnectionStrings__identitydb="Host=postgres;Port=5432;Username=postgres;Password=${POSTGRES_PASSWORD};Database=identitydb;Trust Server Certificate=True;" \
    ConnectionStrings__contentdb="Host=postgres;Port=5432;Username=postgres;Password=${POSTGRES_PASSWORD};Database=contentdb;Trust Server Certificate=True;" \
    ConnectionStrings__aidb="Host=postgres;Port=5432;Username=postgres;Password=${POSTGRES_PASSWORD};Database=aidb;Trust Server Certificate=True;" \
    ConnectionStrings__rabbitmq="amqp://${RABBITMQ_USERNAME}:${RABBITMQ_PASSWORD}@rabbitmq:5672" \
    # AllowedOrigins__0="http://localhost:3000" \
    # YARP Cluster Configuration
    Clusters__0__ClusterId="identityCluster" \
    Clusters__0__Address="http://127.0.0.1:22000" \
    Clusters__1__ClusterId="contentsCluster" \
    Clusters__1__Address="http://127.0.0.1:23000" \
    Clusters__2__ClusterId="aiCluster" \
    Clusters__2__Address="http://127.0.0.1:24000" \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
    # GmailOAuth__ClientId="xxx" \
    # GmailOAuth__ClientSecret="xxxx" \
    # GmailOAuth__RedirectUri="http://localhost:21000/api/identity/auth/signin-google"
    # AI Services
    # DeepSeek__ApiKey="sk-xxx" \
    Ollama__ApiUrl="http://host.docker.internal:11434"

WORKDIR /app/gateway
COPY --from=build /app/gateway .

WORKDIR /app/identity
COPY --from=build /app/identity .

WORKDIR /app/contents
COPY --from=build /app/contents .

WORKDIR /app/ai
COPY --from=build /app/ai .

# Expose the gateway port
EXPOSE 21000

CMD ["bash", "-c", "\
     dotnet /app/identity/FF.Articles.Backend.Identity.API.dll --urls=http://0.0.0.0:22000 & \
     dotnet /app/contents/FF.Articles.Backend.Contents.API.dll --urls=http://0.0.0.0:23000 & \
     dotnet /app/ai/FF.Articles.Backend.AI.API.dll --urls=http://0.0.0.0:24000 & \
     sleep 3 && \
     dotnet /app/gateway/FF.Articles.Backend.Gateway.API.dll --urls='http://0.0.0.0:21000' & \
     wait"] 