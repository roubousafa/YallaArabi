using FluentValidation;

namespace YallaArabi.Application.Features.Words.SearchWords;

public sealed class SearchWordsQueryValidator : AbstractValidator<SearchWordsQuery>
{
    public SearchWordsQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Search query is required")
            .MinimumLength(2).WithMessage("Search query must be at least 2 characters")
            .MaximumLength(100).WithMessage("Search query is too long");
    }
}