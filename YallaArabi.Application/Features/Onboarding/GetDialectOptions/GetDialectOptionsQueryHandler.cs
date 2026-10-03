using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Onboarding.GetDialectOptions;

public sealed class GetDialectOptionsQueryHandler : IRequestHandler<GetDialectOptionsQuery, Result<DialectOptionsResponse>>
{
    public ValueTask<Result<DialectOptionsResponse>> Handle(GetDialectOptionsQuery request, CancellationToken cancellationToken)
    {
        var dialects = Dialects.All
            .Select(d => new DialectOptionItem(d.Key, d.NameAr, d.NameEn))
            .ToList();

        var genders = VoiceGenders.All
            .Select(g => new VoiceGenderOptionItem(g.Key, g.NameAr, g.NameEn))
            .ToList();

        return ValueTask.FromResult(
            Result<DialectOptionsResponse>.Success(
                new DialectOptionsResponse(dialects, genders)));
    }
}