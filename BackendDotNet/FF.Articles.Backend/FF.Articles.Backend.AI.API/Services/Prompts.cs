using System;
using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;

namespace FF.Articles.Backend.AI.API.Services;

public static class Prompts
{
    private const string magicPrompt = "Take a deep breath. Think step by step.";

    public static string User_ArticleList(ArticleListRequest request) =>
    magicPrompt
    + Environment.NewLine +
    $"""
    Generate {request.ArticleCount} articles covering Topic: {request.Topic} in Category: {request.Category}.
    {(!string.IsNullOrWhiteSpace(request.TopicAbstract) ? $"Topic Description: {request.TopicAbstract}" : "")}
    Each article should include:
        Title: Clear and concise, describing the article's focus.
        Abstract:
            - Write abstract in plain text, do not use markdown
            - One paragraph maximum 300 characters, very briefly cover what will be in the article. 
            - You have the overview of all articles in the topic. Carefully deside abstract. 
            - Another AI Assistant will write the ariticle based on category, topic, title, abstract, and tags for that article. He does not know what other articles are in the topic.
        Tags:
            - Provide tags for the article following the tags rules below.
        Each title should explore a distinct subtopic or angle related to the main topic.
    You should also provide a message.
        If the topic is valid: List key subtopics covered and confirm completion.
        If invalid/off-topic: Leave `Articles` an empty list and explain why in `AIMessage`.
    Follow the tags rules. Be very careful with abstract and tags.
    You do not have to provide exact number of articles. The number of articles is flexible. You can provide +- 3 articles.
    """
    + Environment.NewLine +
    TagRules
    + Environment.NewLine +
    """
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
    "AIMessage": "Your message here."
    }    
    """;

    public static string User_ArticleContent(ContentRequest request) =>
    magicPrompt
    + Environment.NewLine +
    $"""
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

    private static string TagRules = """
    Tags list (exactly four/five, following the same order):
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
    → If not applicable, leave blank.

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
}
