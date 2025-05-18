# FireflyArticles Release Notes

### To Do List
- Backend:
    - P5: Add Redis for caching or store identity API
    - P3: Refactor Contents.API to clean archtecture or DDD
    - P1: Add quick article generation (to do)
    - P4: Article add page
    - P3: Batch update article/topic/tag
    - P3: Improve Admin page
    - P1: Fix 2 minutes time out error (done)
    - P5: A summary of entire content database
    - P1: Update remove code fence rule (done)
    - P2: Get Pending tasks from MQ
- Frontend:
    - P3: Refactor Contexts, use reduxStore + hooks.
    - P3: Refactor localStorage 
    - P1: Remove focus area / tech stack tags (done)
    - P1: Move regen button to article card (done)
    - P1: Edit prompt in prompt drawer
    - P3: Remove api url base away from image
    - P3: sider auto collpase
    - P1: Add topic image in ai gen page (done)
    - P1: Fix search by removing initial state (done)
- Other:
    - Add readme.md
    - Add mermaid diagram
    - Change CICD branch naming convension. (done)
    - Amend docker file for quick deployment
---


### App Version 0.1.0
- Overview: Release

---
### App Version 0.0.11
- Overview: Fixed bugs, minor ajustments

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

