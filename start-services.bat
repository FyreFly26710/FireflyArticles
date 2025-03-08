@echo off
setlocal

set BACKEND_PATH=BackendDotNet\FF.Articles.Backend\publish
set FRONTEND_PATH=FrontendReact\ff-articles-frontend\.next\standalone

cd /d %~dp0

:: Run Contents API in Production mode
start "Contents API" /D "%BACKEND_PATH%" cmd /c "set ASPNETCORE_ENVIRONMENT=Production && dotnet FF.Articles.Backend.Contents.API.dll --urls http://*:23000"
:: Run Identity API in Production mode
start "Identity API" /D "%BACKEND_PATH%" cmd /c "set ASPNETCORE_ENVIRONMENT=Production && dotnet FF.Articles.Backend.Identity.API.dll --urls http://*:22000"
:: Run Frontend Server
start "Frontend Server" /D "%FRONTEND_PATH%" cmd /c "set PORT=3000 && node server.js"

endlocal
