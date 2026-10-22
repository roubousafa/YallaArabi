using System.Text;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Infrastructure.Ai;

// Stand-in used until a Claude API key is configured: deterministic, free, and clearly marked as test output.
public class FakeDialectAiProvider : IDialectAiProvider
{
    public string Name => AiOptions.FakeProvider;

    public Task<DialectAiResult> GenerateAsync(string query, DialectDefinition dialect, CancellationToken ct = default)
    {
        if (!query.Any(char.IsLetter))
        {
            return Task.FromResult(new DialectAiResult(false, null, null, null));
        }

        var isArabic = query.Any(ch => ch >= '؀' && ch <= 'ۿ');
        var dialectText = isArabic ? query : $"[تجريبي] {query}";
        var meaning = $"[test] meaning of \"{query}\" in {dialect.NameEn}";
        var arabizi = isArabic ? ToArabizi(query) : query.ToLowerInvariant();

        // Only an English search gives a usable photo query without a real model.
        var imageQuery = isArabic ? null : query.Trim();

        return Task.FromResult(new DialectAiResult(true, dialectText, meaning, arabizi, imageQuery));
    }

    private static string ToArabizi(string text)
    {
        var sb = new StringBuilder();

        foreach (var ch in text)
        {
            sb.Append(ch switch
            {
                'ا' or 'أ' or 'إ' or 'آ' => "a",
                'ب' => "b", 'ت' => "t", 'ث' => "th", 'ج' => "j", 'ح' => "7", 'خ' => "5",
                'د' => "d", 'ذ' => "z", 'ر' => "r", 'ز' => "z", 'س' => "s", 'ش' => "sh",
                'ص' => "9", 'ض' => "d", 'ط' => "6", 'ظ' => "z", 'ع' => "3", 'غ' => "8",
                'ف' => "f", 'ق' => "2", 'ك' => "k", 'ل' => "l", 'م' => "m", 'ن' => "n",
                'ه' or 'ة' => "e", 'و' => "w", 'ي' or 'ى' => "y", 'ء' or 'ئ' or 'ؤ' => "2",
                ' ' => " ",
                _ => ""
            });
        }

        return sb.ToString();
    }
}
