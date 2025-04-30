# Firefly Articles Frontend Documentation

## Tech Stack Overview

The Firefly Articles frontend is built using the following technologies:

- **Framework**: Next.js 14 (React-based framework)
- **UI Library**: Ant Design (antd) 
- **State Management**: Redux Toolkit, context
- **Styling**: Tailwind CSS with styled-components, no extra css files

## Project Structure

```
FrontendReact/ff-articles-frontend/
├── src/
│   ├── api/                # API integration modules
│   │   ├── ai/             # AI-related API calls
│   │   ├── identity/       # Authentication API calls
│   │   └── contents/       # Content-related API calls
│   ├── app/                # Next.js app router pages
│   │   ├── pagename/
│   │   │     ├──pagename.tsx   # page logics
│   │   │     └──page.tsx   # re-exporting page logic
│   │   ├── aichat/         # AI chat page
│   │   ├── aigen/          # AI generation page
│   │   ├── admin/          # Admin pages
│   │   ├── topic/          
│   │   │    └─ [topicid]   # Single topic page
│   │   │           ├─ article  # Sinlgle article page
│   │   │           └─ new      # create article page
│   │   ├── topics/         # Topics list page
│   │   ├── user/           # User/login/register pages
│   │   └── page.tsx        # Home page
│   ├── components/         # Reusable UI components & page.module.css
│   │   ├── shared/         # Shared components across multiple pages
│   │   ├── page specific/  # Components specific to the corresponding page
│   │   │    ├── component.tsx
│   │   │    ├── ...
│   │   │    └── pagename.module.css    # styles for both page and components
│   │   ├── ...
│   │   └── home/           # Components specific to home page
│   ├── hooks/              # Custom React hooks
│   │   ├── page specific/  # Components specific to the corresponding page
│   │   ├── ...
│   │   └── home/           # Hooks specific to home page
│   ├── layouts/            # Layout components
│   ├── libs/               # Utility libraries
│   └── stores/             # State management
│       ├── redux/          # Redux store
│       ├── localStorage/   # Local storage utilities
│       └── context/        # React Context API implementation
├── config/                 # Configuration files
├── public/                 # Static assets
└── ...                     # Configuration files
```

### Topic/Topics pages
- Topics/: list topics and articles table (filter by category/topic/tags)
- Topic: On hover, list category-topics, click redirect to topicid page
- Topic/[topicId]/: article page displaying topic summary article, left sidebar navigate to articles or new page
- Topic/[topicId]/article/[articleId]/: article page, display article, enable edit
- Topic/[topicId]/new: article page to create new article

## Organization Structure

### Page Organization
- Each page has its own folder in the `app` directory
- Page structure includes:
  - `page.tsx`: Main page component (exported by default)
  - `PageName.tsx`: Main page implementation

### Component Organization
- One folder in `components` for each page
- Page-specific components are stored in their respective folders
- Shared components are in `components/shared`

### Hooks Organization
- Dedicated `hooks` folder with subfolders for each page
- Page-specific hooks in their respective folders
- Shared hooks in `hooks/shared`

### Store Organization
- Redux store for global state management
- Local storage for persisting data
- Context for component-level state sharing

### Core Features

#### 1. Article Management

The application allows users to create, view, edit, and manage articles with:
- Markdown editor with syntax highlighting
- Tag management
- Categories and organization

#### 2. AI Integration

AI features include:
- Chat interface
- Content generation capabilities
- AI-assisted editing

## Components Styling

- Ant design components library
- Tailwind css
- Styled components