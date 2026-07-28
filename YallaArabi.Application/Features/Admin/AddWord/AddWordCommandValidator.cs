using FluentValidation;

namespace YallaArabi.Application.Features.Admin.AddWord;

public sealed class AddWordCommandValidator : AbstractValidator<AddWordCommand>
{
    public AddWordCommandValidator()
    {
        RuleFor(x => x.LevelCode)
            .NotEmpty().WithMessage("Level code is required");

        RuleFor(x => x.ArabicText)
            .NotEmpty().WithMessage("Arabic text is required")
            .MaximumLength(200).WithMessage("Arabic text is too long");

        RuleFor(x => x.Transliteration)
            .NotEmpty().WithMessage("Transliteration is required")
            .MaximumLength(200).WithMessage("Transliteration is too long");

        RuleFor(x => x.EnglishMeaning)
            .NotEmpty().WithMessage("English meaning is required")
            .MaximumLength(300).WithMessage("English meaning is too long");
    }
}