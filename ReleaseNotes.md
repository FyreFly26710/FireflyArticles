# FireflyArticles Release Notes

### To Do List
- Backend:
    - P5: Add Redis for caching or store identity API
    - P3: Refactor Contents.API to clean archtecture or DDD
    - P2: Add quick article generation
    - P2: Generate Topic Article (done)
    - P2: Generate existing topic
    - P2: Regarnerate Article (done)
    - P2: Add Prompts API (done)
    - P2: Article add page
    - P4: Fix Ai Gen possible codefence (done)
    - P4: Batch update article/topic/tag
- Frontend:
    - P3: Refactor Contexts, use reduxStore + hooks.
    - P3: Refactor localStorage


### Release App Version 0.0.8 (14/05/2025)
- Overview: Tag Management and UI Improvements
- Changes:
    - Feature:
        - Added Tag Management page in admin section
    - Improvements:
        - Improved Article Table UI
    - Bug fix:
        - Exclude AppHost from Dockerfile
        - Disable migrations in prod env

### Release App Version 0.0.7 (13/05/2025)
- Overview: Tag Selector Improvements
- Changes:
    - Feature:
        - Added tag group functionality
        - Improved tag selector UI
    - Improvements:
        - Removed V2 API endpoints
        - Removed Redis repository
        - Added backup folder to gitignore
    - Bug fix:
        - Fixed topic image display
        - Adjusted article table layout

### Release App Version 0.0.6 (11/05/2025)
- Overview: Nearly Ready.
- Changes:
    - Feature:
        - Add Elastic Search for improved search capabilities. 
        - Add Articles page UI and search function
        - Add Validators
    - Bug fix:
        - Reduced ElasticSearch RAM usage in dev environment
        - Disabled ElasticSearch temporarily for further testing
    - Improvements:
        - Fixed ArticlesPage
        - Implemented Global Exception Middleware
        - Added and organized global usings across all projects
        - Clean up backend codebase
- Fix:
    - Fixed dockerfile
    - Fixed scrollbar and Topic articles
    - Fixed ArticleTable

### Release App Version 0.0.5 (09/05/2025)
- Overview: Testing release.
- Changes:
    - Feature: 
        - Add Gemini AI
    - Improvements:
        - Add loading page
        - Completed CICD
        - Enable Authentication in AI Chat
        - Improved AI Article Gen process: 
            Ai.API send back the entire response to client. Client handle json parse.

### Release App Version 0.0.4 (04/05/2025)
- Overview: Testing release.
- Changes:
    - Bug fix: Fix Gmail OAuth and add more checks

### Release App Version 0.0.3 (27/04/2025)
- Overview: Testing release.
- Changes:
    - Feature: Added RabbitMQ
    - Improvements: 
        - Added dockerfile and docker compose for dev
        - Use generate articles using MQ





## Template

### Release App Version X.Y.Z (dd/mm/yyyy)
- Overview: Testing release.
- Changes:
    - Feature:
    - Bug fix:
    - Improvements:
- ToDo:


## TODO List:
- improve log
- add redis to frontend server
- modify controllerbase to capture errors and return error types instead of middleware.






