@echo off
echo Removing existing container if it exists...
docker rm -f ff-frontend-container 2>nul

rem Set your environment variables here
set API_URL=https://test.firefly-26710.com/api/identity/auth/signin-google
set GOOGLE_CLIENT_ID=xxx
set BASE_URL=https://test.firefly-26710.com

echo Cleaning Docker cache...
docker system prune -f --filter "label=com.docker.compose.project=ff-frontend"

echo Building frontend container...
docker build -t firefly26710/ff-frontend ^
  --no-cache ^
  --build-arg NEXT_PUBLIC_API_URL=%API_URL% ^
  --build-arg NEXT_PUBLIC_GOOGLE_CLIENT_ID=%GOOGLE_CLIENT_ID% ^
  --build-arg NEXT_PUBLIC_BASE_URL=%BASE_URL% ^
  .

echo Running frontend container...
docker run -d --name ff-frontend-container ^
  -p 3000:3000 ^
  firefly26710/ff-frontend

echo Frontend container started. Access it at http://localhost:3000 

pause