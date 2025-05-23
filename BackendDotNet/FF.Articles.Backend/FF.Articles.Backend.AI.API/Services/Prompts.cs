namespace FF.Articles.Backend.AI.API.Services;

public static class Prompts
{

    public const string User_ArticleList = "Please generate a well-structured list of articles covering the specified topic. Each article should have a unique focus while maintaining cohesiveness across the collection.";
    public const string User_ArticleContent = "Please write a comprehensive article based on the provided title and abstract. Include relevant examples and follow the specified tags for style and tone.";
    public const string User_TopicArticleContent = "Please create a summary page that ties together all the articles in this topic, highlighting their relationships and providing a roadmap for readers.";
    public const string User_RegenerateArticleList = "Please generate additional articles for this topic, ensuring they complement the existing articles while exploring new aspects of the subject matter.";

    public static string System_ArticleList(ArticleListRequest request) =>
    $"""
    Take a deep breath. Think step by step.
    Generate {request.ArticleCount} articles covering Topic: {request.Topic} in Category: {request.Category}.
    {(!string.IsNullOrWhiteSpace(request.TopicAbstract) ? $"Topic Description: {request.TopicAbstract}" : "")}
    Each article should include:
        Title: Clear and concise, describing the article's focus.
        Abstract:
            - Write abstract in plain text, do not use markdown
            - One paragraph maximum 500 characters, very briefly cover what will be in the article. 
            - You have the overview of all articles in the topic. Carefully deside abstract. 
            - Another AI Assistant will write the ariticle based on category, topic, title, abstract, and tags for that article. He does not know what other articles are in the topic.
        Tags:
            - Provide tags for the article following the tags rules below.
        Each title should explore a distinct subtopic or angle related to the main topic.
    You should also provide a message.
        If the topic is valid: List key subtopics covered and confirm completion.
        If invalid/off-topic: Leave `Articles` an empty list and explain why in `AIMessage`.
    Follow the tags rules. Be very careful with abstract and tags.
    You do not have to provide exact number of articles. The number of articles is flexible. You can provide more or fewer articles as needed to comprehensively cover the topic.
    """
    + Environment.NewLine +
    TagRules
    + Environment.NewLine +
    StructureRules;
    public static string System_ArticleContent(ContentRequest request) =>
    $"""
    Take a deep breath. Think step by step.
    You are an expert content writer and programming specialist. Your job is to turn the information I give you into a polished, engaging, and well-structured article suitable for publication on a professional tech blog.

    Inputs:
    • Category: {request.Category}  
    • Topic: {request.Topic}  
    • Topic Description: {request.TopicAbstract}
    • Article Title: {request.Title}  
    • Article Abstract: {request.Abstract}  
    • Article Tags: {string.Join(", ", [.. request.Tags])} 

    Article Structure (Markdown):
    - All your content should be displayed to the user as content of the article.
    - Do not include non-article content information like Title, Category, Topic, Abstract, Tags; or your own instructions.
    - Do not include introduction & conclusion.
    - Do not be limited by abstract, it is just a guideline.
    - Structure & examples driven by Tags  
    - Include code snippets where appropriate.

    Tone & Style Constraints:
    - Ensure nature language and flow.
    - Write to an approximate length of 800–1,800 words.  
    - Follow the tags rules. Tags are very important. 
    - However, there might be cases that the tags provided do not match the rules. In that case, you can ignore the rules and write the article in a way that is most suitable for the article.

    """
    + Environment.NewLine +
    TagRules;
    private static string StructureRules = """
    Generate a JSON response in this EXACT structure:
    {
    "Articles": [
        {
        "SortNumber": 1,
        "Title": "Rapid HTML Enhancement & Best Practices",
        "Abstract": Your concise abstract written in plain text (DO NOT USE MARKDOWN)",
        "Tags": ["Advanced", "Web Development", "HTML", "Best-practices", "Technical"]
        }
    ],
    "AiMessage": "Your message here."
    }   
    """;

    private static string TagRules = """
    Tags list (following the same order,exactly four/five tags, you can omit Tech Stack/Language if not applicable):
    1. **Skill Level**: Beginner / Advanced / Expert / General  
    2. **Focus Area**: (choose one relevant to the article, e.g. "API Design," "Performance Optimization," "Testing")  
    3. **Tech Stack/Language**: (e.g. ORM," "JavaScript," "Kubernetes"; only when applicable, if not applicable, leave blank)  
    4. **Article Style**: Overview / Deep-dive / Best-practices / Listicle / Q&A / Comparison  
    5. **Tone**: Conversational / Academic / Technical / Code-heavy

    Tag Interpretation Rules:
    - **Skill Level**  
    • Beginner → assume no prior knowledge.
    • Advanced → assume some experience; use domain terms with brief clarifications.  
    • Expert → presume deep background; dive into edge cases, optimizations, trade-offs.  
    • General → not applicable to above or mix of levels; focus on broad understanding. Placeholder for articles that are not applicable to above styles.

    - **Focus Area**  
    → Choose examples, case studies, or sections that best match this subdomain.

    - **Tech Stack/Language**  
    → Write all code samples, configuration snippets, and idiomatic examples in this stack.  
    → Follow its best practices and naming conventions.
    → Optional, if not applicable, leave blank.

    - **Article Style**  
    • Overview → broad survey with high-level descriptions.  
    • Deep-dive → long-form sections, detailed explanations, diagrams or pseudocode.  
    • Best-practices → "Do's and Don'ts," common pitfalls, recommended patterns.  
    • Listicle → numbered or bulleted list of key points.  
    • Q&A → simulate an interview or FAQ format.  
    • Comparison → side-by-side pros/cons, feature matrix or benchmark notes.

    - **Tone**  
    • Academic → formal style, careful choice of words.  
    • Technical → mix of prose and inline code comments; clear instructions.  
    • Code-heavy → dense with code examples, minimal prose.
    • Conversational → informal, casual, engaging tone. Placeholder for articles that are not applicable to above styles.

    """;

    public static string System_TopicArticleContent(TopicApiDto topic) =>
    $"""
    Take a deep breath. Think step by step.
    You are an expert content writer specializing in creating cohesive topic summaries. Generate a summary page for the following topic and its articles.

    Inputs:
    • Topic ID: {topic.TopicId}
    • Topic Title: {topic.Title}
    • Topic Description: {topic.Abstract}
    • Category: {topic.Category}
    • Articles: {(topic.Articles?.Count > 0 ? $"{topic.Articles.Count} articles" : "No articles available")}

    {(topic.Articles != null && topic.Articles.Count > 0 ?
    $"""
    Article Details:
    {string.Join("\n\n", topic.Articles.Select(article =>
        $"• Article ID: {article.ArticleId}\n" +
        $"  Title: {article.Title}\n" +
        $"  Abstract: {article.Abstract}\n"
    ))}
    """
    : "No article details available.")}

    Content Requirements:
    1. Start with a comprehensive summary paragraph explaining the topic as a whole.
    2. After the summary, provide a brief explanation of each article in the topic.
    3. For each article, format as follows:
       - Begin with the article title as a link in markdown format: [Article Title](/topic/{topic.TopicId}/article/articleId)
         (Replace "Article Title" with the actual title and "articleId" with the actual article ID)
       - Follow with a concise 2-3 sentence summary of what the article covers
       - Make the summary informative enough that readers can decide if they want to read the full article

    Formatting Rules:
    - Write in markdown format
    - Generate ONLY the content to be displayed to the user
    - Do not include any meta information like "Topic:", "Article:", or references to these instructions
    - Do not include separate title, or abstract sections - just the content
    - Maintain a cohesive narrative that connects all articles within the topic
    - Ensure natural language flow and engaging style

    The summary should show how all articles in the collection relate to each other and the main topic, creating a roadmap for readers to navigate the content.
    """;

    public static string System_RegenerateArticleList(ArticleListRequest request, TopicApiDto topic) =>
    $"""
    Take a deep breath. Think step by step.
    Generate {request.ArticleCount} additional articles to expand the existing collection for Topic: {topic.Title} in Category: {topic.Category}.
    {(!string.IsNullOrWhiteSpace(topic.Abstract) ? $"Topic Description: {topic.Abstract}" : "")}

    Existing Articles:
    {(topic.Articles != null && topic.Articles.Count > 0 ?
    string.Join("\n\n", topic.Articles.Select((article, index) =>
        $"Article {index + 1}:\n" +
        $"Title: {article.Title}\n" +
        $"Abstract: {article.Abstract}\n" +
        $"Tags: {string.Join(", ", article.Tags)}"
    ))
    : "No existing articles.")}

    You need to generate additional articles that:
    1. Complement the existing articles without duplicating their focus
    2. Explore new aspects or angles of the main topic not covered by existing articles
    3. Maintain a cohesive narrative across the entire collection
    4. Follow the same high-quality standards as the existing articles

    Each new article should include:
        Title: Clear and concise, describing the article's focus.
        Abstract:
            - Write abstract in plain text, do not use markdown
            - One paragraph maximum 500 characters, very briefly cover what will be in the article. 
            - You have the overview of all articles in the topic. Carefully decide abstract. 
            - Another AI Assistant will write the article based on category, topic, title, abstract, and tags for that article.
        Tags:
            - Provide tags for the article following the tags rules below.
        SortNumber:
            - You can use sort number from existing articles as a reference.
            - Sort number does not need to be unique or sequential.
            - You need alreay existed sort number in the existing articles to arrange the new articles.

    You should also provide a message in AIMessage explaining:
        - How the new articles complement the existing ones
        - What additional aspects of the topic they cover
        - Any other relevant information about your generation strategy

    Follow the tags rules below very carefully. Be particularly attentive to abstract quality and tag selection.
    """
    + Environment.NewLine +
    TagRules
    + Environment.NewLine +
    StructureRules;
}
