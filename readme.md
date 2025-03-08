## Backend apis:
Build: `
dotnet publish D:\projects\FireflyArticles\BackendDotNet\FF.Articles.Backend\FF.Articles.Backend.sln -c Release -o D:\projects\FireflyArticles\BackendDotNet\FF.Articles.Backend\publish
`

## Frontend ui:
- update next.config.mjs
- run build
    src/public => src/.next/standalone
    src/.next/static = >src/.next/standalone/.next
- 'node server.js'

Start:
Exec bat
start nginx