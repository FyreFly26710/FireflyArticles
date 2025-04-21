using System;

namespace FF.Articles.Backend.AI.API.Services;

public static class Prompts
{
    private const string magicPrompt = "Take a deep breath. Think step by step. You are a helpful assistant.";

    public static string User_ArticleList(string topic, int articleCount) =>
    magicPrompt
    + Environment.NewLine +
    $"""
    Generate {articleCount} article titles covering the {topic} from beginner to expert level.  
    Each article should include:
        Title: Clear and concise, describing the article's focus.
        Abstract:
            - Provide 2-3 key points, each in a single concise sentence. 
            - It does not need to be proper full sentences. Just key words and very brief explanation.
        Tags:
            - Provide 2-4 tags for the article.
        Each title should explore a distinct subtopic or angle related to the main topic.
        Do not write too much articles about beginner levels.
    You should also provide a message.
        If the topic is valid: List key subtopics covered and confirm completion.
        If invalid/off-topic: Leave `Articles` an empty list and explain why in `AIMessage`.
    """
    + Environment.NewLine +
    """
    Generate a JSON response in this EXACT structure:
    {
    "Articles": [
        {
        "Id": 1,
        "Title": "Rapid HTML Enhancement & Best Practices",
        "Abstract": "**Semantic HTML** - Using elements that convey meaning for better accessibility and SEO.  \n**Metadata & SEO** - `<meta> tags`, `<title>`, how HTML influences search engines.  \n**Accessibility Considerations** - `aria-*` attributes, proper use of headings and labels.",
        "Tags": ["html", "seo", "web development"]
        },
        { 
        "Id": 2,
        "Title": "HTML5 Features & Best Practices",
        "Abstract": "**HTML5 Features** - New elements like `<header>`, `<footer>`, `<section>`, and `<article>`.  \n**Best Practices** - Use semantic HTML, optimize images, and ensure cross-browser compatibility.",
        "Tags": ["html5", "web development", "best practices"]
        }
    ],
    "AIMessage": "Your message here."
    }    
    """;

    public static string User_ArticleContent(string topic, string title, string abs, List<string> tags) => $"""
    {magicPrompt}
    We need you to write a detailed article based on the following information.
    Topic: {topic}
    Title: {title}
    Abstract: {abs}
    Tags: {string.Join(", ", [.. tags])}
    Respond ONLY with the raw markdown content following this exact structure:
    
        # Introduction
        Content...

        ## First Key Point (smaller point)
        Content...

        ## Second Key Point (larger point)
        Content...
        Content...
        Content...

        # Conclusion
        Content...
    
    Introduction and Conclusion contents should be 1 paragraphs.
    Each Key Points should have 1-3 paragraphs.

    DO NOT include: 
    - Title
    """;

}
