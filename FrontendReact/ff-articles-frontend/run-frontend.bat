@echo off
echo Removing existing container if it exists...
docker rm -f ff-frontend-container 2>nul

echo Building frontend container...
docker build -t ff-frontend .

echo Running frontend container...
docker run -d --name ff-frontend-container ^
  -p 3000:3000 ^
  --env NEXT_PUBLIC_API_URL=http://localhost:21000/api/identity/auth/signin-google ^
  --env NEXT_PUBLIC_GOOGLE_CLIENT_ID=xxx ^
  --env NEXT_PUBLIC_BASE_URL=http://localhost:21000/ ^
  --network ffarticlesbackend_fireflynet ^
  ff-frontend

echo Frontend container started. Access it at http://localhost:3000 