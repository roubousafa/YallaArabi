using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Admin.AddLessonQuestion;

namespace YallaArabi.Application.Features.Admin.UpdateLessonQuestion;

public sealed record UpdateLessonQuestionCommand(
    Guid QuestionId,
    string Type,
    string? QuestionText,
    string? OptionA,
    string? OptionB,
    string? OptionC,
    string? OptionD,
    string? CorrectOption,
    Guid? WordId,
    string? AudioUrl,
    string? ImageUrl,
    int OrderIndex
) : IRequest<Result<Unit>>, ILessonQuestionFields;

public sealed class UpdateLessonQuestionCommandValidator : LessonQuestionFieldsValidator<UpdateLessonQuestionCommand>
{
    public UpdateLessonQuestionCommandValidator()
    {
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0).WithMessage("Order index cannot be negative");
    }
}
