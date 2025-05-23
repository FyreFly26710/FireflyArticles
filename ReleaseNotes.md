# FireflyArticles Release Notes

### To Do List
- Backend improvements:
    - P5: Add Redis for caching.
    - P1: Refactor Contents.API to clean archtecture & DDD
    - P4: Article add page
    - P3: Batch update article/topic/tag
- Frontend improvements:
    - P2: Edit prompt in prompt drawer
    - P3: Remove api url base away from image
    - P1: Fix aichat page, there are bugs around sessionid. 
- Feature: Visualize message queues
    - article list generation message
        - db: store messages in db: Category-Topic, input params, ai response, date, type (new/existing), status (pending/processing/completed)
        - API: get, update, remove
        - Front: get topicId, dispatch message, view messages from db, parse raw json to objects, dispatch article generation message
    - article generation message
        - db: store messages in db: Title, input params, date, type, status
        - API: get, update, remove
        - Front: view messages

- Other:
    - Amend docker file for quick start


---

### App Version 0.3.0 (24/05/2025)
- Overview: UI Improvements
- Changes: 
    - Improvements:
        - Made article page fit mobile screens
        - Changed tag selector to single select. API returns articles matching all tags
        - Added article sider auto collapse
        - Improved ai gen form layout and tips
    - Other:
        - Added readme and license


### App Version 0.2.0 (23/05/2025)
- Overview: Refactored frontend codebase
- Changes: 
    - Improvements:
        - split components logic into hooks
        - remove context, use redux
        - remove localstorage, use redux persist
        - Improve api gateway config
    - Bug fix:
        - cancel message consuming outside processing window
        - set longer timeout for api gateway
- HotFix:
    - 0.2.1: add missing dependency
    - 0.2.2: fix minor issues in prompts

### App Version 0.1.0 (18/05/2025)
- Overview: Release, completed all major features.
- Changes: 
    - Feature: 
        - Enable adding topic image in AI Gen page
    - Improvements:
        - Improved YARP settings
        - Remove focus area / tech stack from tag selectors
        - Add Generate button in article page for quick generation
        - Changed CICD branch naming strategy
    - Bug fix: 
        - Fix Ai Gen page 100 seconds time out error caused by YARP
- To do: 
    - Refactor codebase

---
### App Version 0.0.9 (17/05/2025)
- Overview: Enhanced AI Article Generation and Prompts System
- Changes:
    - Feature:
        - Added Topic Article Generation capability
        - Added Article List Regeneration for existing topics
        - Added Single Article Regeneration
        - Implemented AI Prompts API and UI drawer
    - Improvements:
        - Enhanced AI generation with better code fence handling
        - Added preview capability for AI prompts before generation
        - Improved URL encoding for special characters in API requests
        - CICD: Switched to Cloudflare tunnel
    - Bug fix:
        - Fixed code fence issues in AI-generated content
        - Fixed Title length being too short

---
### App Version 0.0.8 (14/05/2025)
- Overview: Tag Management and UI Improvements
- Changes:
    - Feature:
        - Added Tag Management page in admin section
    - Improvements:
        - Improved Article Table UI
    - Bug fix:
        - Exclude AppHost from Dockerfile
        - Disable migrations in prod env

---
### App Version 0.0.7 (13/05/2025)
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

---
### App Version 0.0.6 (11/05/2025)
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

---
### App Version 0.0.5 (09/05/2025)
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

---
### App Version 0.0.4 (04/05/2025)
- Overview: Testing release.
- Changes:
    - Bug fix: Fix Gmail OAuth and add more checks

---
### App Version 0.0.3 (27/04/2025)
- Overview: Testing release.
- Changes:
    - Feature: Added RabbitMQ
    - Improvements: 
        - Added dockerfile and docker compose for dev
        - Use generate articles using MQ

