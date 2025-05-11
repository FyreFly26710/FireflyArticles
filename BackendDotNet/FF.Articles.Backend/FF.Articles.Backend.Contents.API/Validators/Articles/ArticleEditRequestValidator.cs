namespace FF.Articles.Backend.Contents.API.Validators.Articles;

public class ArticleEditRequestValidator : AbstractValidator<ArticleEditRequest>
{
    public ArticleEditRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .When(x => x.Title != null);

        RuleFor(x => x.Abstract)
            .MaximumLength(800).WithMessage("Abstract cannot exceed 800 characters")
            .When(x => x.Abstract != null);

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