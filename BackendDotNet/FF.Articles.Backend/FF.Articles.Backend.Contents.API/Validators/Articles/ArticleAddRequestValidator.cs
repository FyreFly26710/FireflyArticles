namespace FF.Articles.Backend.Contents.API.Validators.Articles;

public class ArticleAddRequestValidator : AbstractValidator<ArticleAddRequest>
{
    public ArticleAddRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(400).WithMessage("Title cannot exceed 400 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(80000).WithMessage("Content cannot exceed 80000 characters");

        RuleFor(x => x.Abstract)
            .MaximumLength(1000).WithMessage("Abstract cannot exceed 1000 characters");

        RuleFor(x => x.ArticleType)
            .NotEmpty().WithMessage("Article type is required")
            .Must(BeValidArticleType).WithMessage("Article type must be one of: Article, SubArticle, or TopicArticle");

        RuleFor(x => x.ParentArticleId)
            .NotNull().When(x => x.ArticleType == ArticleTypes.SubArticle)
            .WithMessage("Parent article ID is required for sub-articles");

        RuleFor(x => x.TopicId)
            .NotEqual(0).WithMessage("Topic ID is required");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Cannot have more than 10 tags");
    }

    private bool BeValidArticleType(string articleType)
    {
        return articleType == ArticleTypes.Article ||
               articleType == ArticleTypes.SubArticle ||
               articleType == ArticleTypes.TopicArticle;
    }
}