FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY FF.AI.Common/FF.AI.Common.csproj ./FF.AI.Common/
COPY FF.Articles.AppHost/FF.Articles.AppHost.csproj ./FF.Articles.AppHost/
COPY FF.Articles.Backend.AI.API/FF.Articles.Backend.AI.API.csproj ./FF.Articles.Backend.AI.API/
COPY FF.Articles.Backend.Common/FF.Articles.Backend.Common.csproj ./FF.Articles.Backend.Common/
COPY FF.Articles.Backend.Contents.API/FF.Articles.Backend.Contents.API.csproj ./FF.Articles.Backend.Contents.API/
COPY FF.Articles.Backend.Gateway.API/FF.Articles.Backend.Gateway.API.csproj ./FF.Articles.Backend.Gateway.API/
COPY FF.Articles.Backend.Identity.API/FF.Articles.Backend.Identity.API.csproj ./FF.Articles.Backend.Identity.API/
# COPY FF.Articles.Backend.RabbitMQ/FF.Articles.Backend.RabbitMQ.csproj ./FF.Articles.Backend.RabbitMQ/
# COPY FF.Articles.Backend.ServiceDefaults/FF.Articles.Backend.ServiceDefaults.csproj ./FF.Articles.Backend.ServiceDefaults/

RUN dotnet restore

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
    ConnectionStrings__redis="redis:6379" \
    ConnectionStrings__rabbitmq="amqp://${RABBITMQ_USERNAME}:${RABBITMQ_PASSWORD}@rabbitmq:5672" \
    # AllowedOrigins__0="http://localhost:3000" \
    # YARP Route Configuration
    ReverseProxy__Routes__contents-route__ClusterId=contentsCluster \
    ReverseProxy__Routes__contents-route__Match__Path="/api/contents/{**catch-all}" \
    ReverseProxy__Routes__identity-route__ClusterId=identityCluster \
    ReverseProxy__Routes__identity-route__Match__Path="/api/identity/{**catch-all}" \
    ReverseProxy__Routes__ai-route__ClusterId=aiCluster \
    ReverseProxy__Routes__ai-route__Match__Path="/api/ai/{**catch-all}" \
    # YARP Cluster Configuration
    ReverseProxy__Clusters__contentsCluster__Destinations__destination1__Address="http://127.0.0.1:23000" \
    ReverseProxy__Clusters__identityCluster__Destinations__destination1__Address="http://127.0.0.1:22000" \
    ReverseProxy__Clusters__aiCluster__Destinations__destination1__Address="http://127.0.0.1:24000" \
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