 ## Backend apis:
Build: `dotnet publish -c Release -m:8 -o ./publish`
Start: 
D:\202502\FireflyArticles\BackendDotNet\FF.Articles.Backend\publish>dotnet FF.Articles.Backend.Contents.API.dll --urls "http://*:23000"
PS D:\202502\FireflyArticles\BackendDotNet\FF.Articles.Backend\publish> dotnet FF.Articles.Backend.Identity.API.dll --urls "http://*:22000"

Stop: 

## Frontend ui:
- update next.config.mjs
- run build
    src/public => src/.next/standalone
    src/.next/static = >src/.next/standalone/.next
- 'node server.js'
