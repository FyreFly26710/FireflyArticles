using System;

namespace FF.Articles.Backend.AI.API.Services;

public static class Prompts
{
    private const string magicPrompt = "Take a deep breath. Think step by step.";

    public static string User_ArticleList(string topic, int articleCount) =>
    magicPrompt
    + Environment.NewLine +
    $"""
    Generate {articleCount} article titles covering the {topic}.
    Each article should include:
        Title: Clear and concise, describing the article's focus.
        Abstract:
            - Provide 2-3 key points, each in a single concise sentence. 
            - It does not need to be proper full sentences. Just key words and very brief explanation.
        Tags:
            - Provide five tags for the article.
        Each title should explore a distinct subtopic or angle related to the main topic.
    You should also provide a message.
        If the topic is valid: List key subtopics covered and confirm completion.
        If invalid/off-topic: Leave `Articles` an empty list and explain why in `AIMessage`.
    Follow the tags rules. Be very careful with the tags.
    """
    + Environment.NewLine +
    TagRules
    + Environment.NewLine +
    """
    Generate a JSON response in this EXACT structure:
    {
    "Articles": [
        {
        "Id": 1,
        "Title": "Rapid HTML Enhancement & Best Practices",
        "Abstract": "**Semantic HTML** - Using elements that convey meaning for better accessibility and SEO.  \n**Metadata & SEO** - `<meta> tags`, `<title>`, how HTML influences search engines.  \n**Accessibility Considerations** - `aria-*` attributes, proper use of headings and labels.",
        "Tags": ["Advanced", "Web Development", "HTML", "Best-practices", "Technical"]
        }
    ],
    "AIMessage": "Your message here."
    }    
    """;

    public static string User_ArticleContent(string category, string topic, string title, string @abstract, List<string> tags) =>
    magicPrompt
    + Environment.NewLine +
    $"""
    You are an expert content writer and programming specialist. Your job is to turn the information I give you into a polished, engaging, and well-structured article suitable for publication on a professional tech blog.

    Inputs:
    • Category: {category}  
    • Topic: {topic}  
    • Title: {title}  
    • Abstract: {@abstract}  
    • Tags: {string.Join(", ", [.. tags])} 

    Article Structure (Markdown):
    - Do not include title, introduction & conclusion.
    - Do not be limited by abstract, it is just a guideline.
    - Structure & examples driven by Tags  
    - Include code snippets where appropriate.

    Tone & Style Constraints:
    - Ensure nature language and flow.
    - Adhere strictly to the five Tag values—do not invent or deviate.  
    - Write to an approximate length of 800–1,200 words.  

    """
    + Environment.NewLine +
    TagRules;

    private static string TagRules = """
    Tags (exactly five, following the same order; omit Tech Stack/Language if non-programming):
    1. **Skill Level**: Beginner / Advanced / Expert / General  
    2. **Focus Area**: (choose one relevant to the article, e.g. “API Design,” “Performance Optimization,” “Testing”)  
    3. **Tech Stack/Language**: (e.g. ORM,” “JavaScript,” “Kubernetes”; only for programming articles)  
    4. **Article Style**: Overview / Deep-dive / Best-practices / Listicle / Q&A / Comparison  
    5. **Tone**: Conversational / Analytical / Academic / Technical / Code-heavy

    Tag Interpretation Rules:
    - **Skill Level**  
    • Beginner → assume no prior knowledge.
    • Advanced → assume some experience; use domain terms with brief clarifications.  
    • Expert → presume deep background; dive into edge cases, optimizations, trade-offs.  
    • General → not applicable to above or mix of levels; focus on broad understanding.

    - **Focus Area**  
    → Choose examples, case studies, or sections that best match this subdomain.

    - **Tech Stack/Language**  
    → Write all code samples, configuration snippets, and idiomatic examples in this stack.  
    → Follow its best practices and naming conventions.

    - **Article Style**  
    • Overview → broad survey with high-level descriptions.  
    • Deep-dive → long-form sections, detailed explanations, diagrams or pseudocode.  
    • Best-practices → “Do’s and Don’ts,” common pitfalls, recommended patterns.  
    • Listicle → numbered or bulleted list of key points.  
    • Q&A → simulate an interview or FAQ format.  
    • Comparison → side-by-side pros/cons, feature matrix or benchmark notes.

    - **Tone**  
    • Conversational → use friendly tone.
    • Academic → formal style, careful choice of words.  
    • Technical → mix of prose and inline code comments; clear instructions.  
    """;
}
