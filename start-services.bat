@echo off
setlocal

set BACKEND_PATH=BackendDotNet\FF.Articles.Backend\publish
set FRONTEND_PATH=FrontendReact\ff-articles-frontend\.next\standalone

cd /d %~dp0

start "Contents API" /D "%BACKEND_PATH%" dotnet FF.Articles.Backend.Contents.API.dll --urls "http://*:23000"
start "Identity API" /D "%BACKEND_PATH%" dotnet FF.Articles.Backend.Identity.API.dll --urls "http://*:22000"
start "Frontend Server" /D "%FRONTEND_PATH%" node server.js

endlocal
