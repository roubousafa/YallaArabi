using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Onboarding.SetDialect;

public sealed record SetDialectCommand(
    string DialectKey,
    string? VoiceGender
) : IRequest<Result<SetDialectResponse>>;

public sealed record SetDialectResponse(
    string DialectKey,
    string NameAr,
    string NameEn,
    string VoiceGender
);