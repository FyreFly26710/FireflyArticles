 ## Backend apis:
Build: `dotnet publish -c Release -m:8 -o ./publish`
Start: 
dotnet FF.Articles.Backend.Contents.API.dll --urls "http://*:23000"
dotnet FF.Articles.Backend.Identity.API.dll --urls "http://*:22000"

Stop: 

## Frontend ui:
- update next.config.mjs
- run build
    src/public => src/.next/standalone
    src/.next/static = >src/.next/standalone/.next
- 'node server.js'
