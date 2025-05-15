namespace FF.Articles.Backend.Contents.API.Validators.Articles;

public class ArticleEditRequestValidator : AbstractValidator<ArticleEditRequest>
{
    public ArticleEditRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(400).WithMessage("Title cannot exceed 400 characters");

        RuleFor(x => x.Content)
            .MaximumLength(80000).WithMessage("Content cannot exceed 80000 characters");

        RuleFor(x => x.Abstract)
            .MaximumLength(1000).WithMessage("Abstract cannot exceed 1000 characters");

        RuleFor(x => x.ArticleType)
            .Must(BeValidArticleType).WithMessage("Article type must be one of: Article, SubArticle, or TopicArticle")
            .When(x => x.ArticleType != null);

        RuleFor(x => x.ParentArticleId)
            .NotNull().When(x => x.ArticleType == ArticleTypes.SubArticle)
            .WithMessage("Parent article ID is required for sub-articles");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Cannot have more than 10 tags");
    }

    private bool BeValidArticleType(string? articleType)
    {
        if (articleType == null) return true;

        return articleType == ArticleTypes.Article ||
               articleType == ArticleTypes.SubArticle ||
               articleType == ArticleTypes.TopicArticle;
    }

}