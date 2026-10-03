using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Onboarding.GetDialectOptions;

public sealed record GetDialectOptionsQuery : IRequest<Result<DialectOptionsResponse>>;

public sealed record DialectOptionsResponse(
    IReadOnlyList<DialectOptionItem> Dialects,
    IReadOnlyList<VoiceGenderOptionItem> VoiceGenders
);

public sealed record DialectOptionItem(
    string Key,
    string NameAr,
    string NameEn
);

public sealed record VoiceGenderOptionItem(
    string Key,
    string NameAr,
    string NameEn
);