namespace FF.Articles.Backend.Contents.API.Validators.Articles;

public class ArticleAddRequestValidator : AbstractValidator<ArticleAddRequest>
{
    public ArticleAddRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(20000).WithMessage("Content cannot exceed 20000 characters");

        RuleFor(x => x.Abstract)
            .MaximumLength(800).WithMessage("Abstract cannot exceed 800 characters");

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