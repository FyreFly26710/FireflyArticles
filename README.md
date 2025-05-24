# Firefly Articles: AI-Powered Content Generation

## Overview

Firefly Articles is a web application designed to streamline content creation using the power of Artificial Intelligence. 

## Features

* **AI-Powered Article Generation:** Automatically create multiple articles based on your inputs. Generation tasks are sent to message queues, everything will be handled in the background.
* **Intuitive Interface:** Easy-to-use web interface for viewing, searching and generating articles.
* **Article Search:** Filter articles using category/topic/tags or search using keywords.
* **Aricle Review & Editing:** Review and further refine or regenerate the AI-generated content.
* **AI Chat:** Chat with a varity of AIs and manage chat history


## Technology Stack
* **Frontend:** 
    - Next.js
    - Server side rendering
    - Redux & Redux Persist
    - Axios 
* **Backend:** 
    - .Net 8.0
    - .Net Aspire
    - RabbitMQ
    - EF Core
    - Microservices Architecture
    - RESTful API
    - AI Integration
* **Database:** 
    - PostgreSQL
    - Elastic Search
    - Redis
* **DevOps:** 
    - GitActions CICD
    - Docker
    - Nginx

![Architect.png](docs/images/Architect.png)



## Getting Started

To do


## Screenshots
### Generate Article List
- Inputs to generate article list

![GenerateArticleList_Begin.png](docs/images/GenerateArticleList_Begin.png)

---
- Return full Ai response as a string in case some ai responses could not be parsed properly. Users can edit raw json to modify prompts for article.

![GenerateArticleList.png](docs/images/GenerateArticleList.png)

---

### Generate Article
- Generate topic summary page.

![GenerateArticle_TopicSummary.png](docs/images/GenerateArticle_TopicSummary.png)

---
- Generated summary page, include links to articles 

![GenerateArticle_TopicSummary_Complete.png](docs/images/GenerateArticle_TopicSummary_Complete.png)
