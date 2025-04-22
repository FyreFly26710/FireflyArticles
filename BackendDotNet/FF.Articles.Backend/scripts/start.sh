#!/bin/bash
set -e

# Start each microservice in the background
dotnet /app/identity/FF.Articles.Backend.Identity.API.dll --urls=http://0.0.0.0:22000 &
dotnet /app/contents/FF.Articles.Backend.Contents.API.dll --urls=http://0.0.0.0:23000 &
dotnet /app/ai/FF.Articles.Backend.AI.API.dll --urls=http://0.0.0.0:24000 &

# Give microservices time to start
sleep 3

# Start the gateway
dotnet /app/gateway/FF.Articles.Backend.Gateway.API.dll --urls=http://0.0.0.0:21000 &

# Wait for all child processes
wait 