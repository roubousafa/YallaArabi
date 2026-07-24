using System.Text;

namespace YallaArabi.Domain.Services;

public static class TextNormalizer
{
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return ContainsArabic(input) ? NormalizeArabic(input) : NormalizeLatin(input);
    }

    public static bool ContainsArabic(string input) =>
        input.Any(ch => ch >= '\u0600' && ch <= '\u06FF');

    public static string NormalizeArabic(string input)
    {
        var sb = new StringBuilder(input.Length);

        foreach (var ch in input.Trim())
        {
            // Skip diacritics (tashkeel) and tatweel
            if ((ch >= '\u064B' && ch <= '\u0652') || ch == '\u0640' || ch == '\u0670')
                continue;

            sb.Append(ch switch
            {
                'أ' or 'إ' or 'آ' => 'ا',
                'ة' => 'ه',
                'ى' => 'ي',
                _ => ch
            });
        }

        return sb.ToString();
    }

    public static string NormalizeLatin(string input)
    {
        var lowered = input.Trim().ToLowerInvariant();
        var sb = new StringBuilder(lowered.Length + 4);

        foreach (var ch in lowered)
        {
            switch (ch)
            {
                case '2': sb.Append('a'); break;   // ء
                case '3': sb.Append('a'); break;   // ع
                case '5': sb.Append("kh"); break;  // خ
                case '6': sb.Append('t'); break;   // ط
                case '7': sb.Append('h'); break;   // ح
                case '8': sb.Append("gh"); break;  // غ
                case '9': sb.Append('s'); break;   // ص
                case ' ': sb.Append(' '); break;
                default:
                    if (ch >= 'a' && ch <= 'z')
                        sb.Append(ch);
                    break; // strip punctuation and anything else
            }
        }

        return CollapseTriplicates(sb.ToString());
    }

    private static string CollapseTriplicates(string input)
    {
        if (input.Length <= 2)
            return input;

        var sb = new StringBuilder(input.Length);

        foreach (var ch in input)
        {
            var len = sb.Length;
            if (len >= 2 && sb[len - 1] == ch && sb[len - 2] == ch)
                continue; // skip third+ consecutive repetition

            sb.Append(ch);
        }

        return sb.ToString();
    }
}