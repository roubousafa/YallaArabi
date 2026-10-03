using System.Text;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Admin.ExportRecordingScript;

public sealed class ExportRecordingScriptQueryHandler
    : IRequestHandler<ExportRecordingScriptQuery, Result<RecordingScriptResponse>>
{
    private readonly IRecordingSessionRepository _sessionRepository;

    public ExportRecordingScriptQueryHandler(IRecordingSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async ValueTask<Result<RecordingScriptResponse>> Handle(
        ExportRecordingScriptQuery request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return Result<RecordingScriptResponse>.Failure("session_not_found", "Recording session not found");
        }

        var items = await _sessionRepository.GetItemsAsync(session.Id, cancellationToken);

        var dialect = Dialects.Get(session.Dialect);
        var gender = VoiceGenders.All.FirstOrDefault(g => g.Key == session.Gender);

        var builder = new StringBuilder();

        builder.AppendLine("جلسة تسجيل صوتي — يلا عربي");
        builder.AppendLine();
        builder.AppendLine($"اللهجة   : {dialect?.NameAr ?? session.Dialect}");
        builder.AppendLine($"الصوت    : {gender?.NameAr ?? session.Gender}");
        builder.AppendLine($"العدد    : {items.Count} مفردة");
        builder.AppendLine();
        builder.AppendLine("التعليمات:");
        builder.AppendLine("  • اقرأ المفردات بالترتيب دون تخطٍّ");
        builder.AppendLine("  • اصمت ثانيتين كاملتين بعد كل مفردة");
        builder.AppendLine("  • إن أخطأت، أعد المفردة نفسها ثم أكمل");
        builder.AppendLine("  • انطق بطبيعتك كما تتحدث يومياً");
        builder.AppendLine("  • استرح كل نصف ساعة");
        builder.AppendLine();
        builder.AppendLine(new string('─', 40));
        builder.AppendLine();

        foreach (var item in items)
        {
            builder.AppendLine($"{item.OrderNumber:D3} — {item.TextToRead}");
            builder.AppendLine();
        }

        builder.AppendLine(new string('─', 40));
        builder.AppendLine("انتهت الجلسة. شكراً لك.");

        var utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var textBytes = Encoding.UTF8.GetBytes(builder.ToString());
        var content = utf8Bom.Concat(textBytes).ToArray();

        var fileName = $"recording-script-{session.Dialect}-{session.Gender}-{DateTime.UtcNow:yyyyMMdd}.txt";

        return Result<RecordingScriptResponse>.Success(
            new RecordingScriptResponse(content, fileName, items.Count));
    }
}