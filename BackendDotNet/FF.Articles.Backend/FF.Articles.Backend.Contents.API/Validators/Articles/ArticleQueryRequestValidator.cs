namespace FF.Articles.Backend.Contents.API.Validators.Articles;

public class ArticleQueryRequestValidator : AbstractValidator<ArticleQueryRequest>
{
    public ArticleQueryRequestValidator()
    {
        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("Keyword cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));
    }
}