using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.AddLessonQuestion;

public sealed record AddLessonQuestionCommand(
    Guid LessonId,
    string Type,
    string? QuestionText,
    string? OptionA,
    string? OptionB,
    string? OptionC,
    string? OptionD,
    string? CorrectOption,
    Guid? WordId,
    string? AudioUrl,
    string? ImageUrl
) : IRequest<Result<AddLessonQuestionResponse>>, ILessonQuestionFields;

public sealed record AddLessonQuestionResponse(
    Guid QuestionId,
    Guid LessonId,
    int OrderIndex,
    int TotalQuestions
);

public sealed class AddLessonQuestionCommandValidator : LessonQuestionFieldsValidator<AddLessonQuestionCommand>
{
}
