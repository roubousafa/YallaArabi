using System.Security.Cryptography;

namespace YallaArabi.Domain.Common;

public static class FriendCodes
{
    public const string Prefix = "YA-";
    private const int CodeLength = 5;

    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    public static string Generate()
    {
        var chars = new char[CodeLength];

        for (var i = 0; i < CodeLength; i++)
        {
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return Prefix + new string(chars);
    }

    public static string Normalize(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return "";
        }

        var cleaned = code.Trim().ToUpperInvariant().Replace(" ", "").Replace("-", "");

        if (cleaned.StartsWith("YA"))
        {
            cleaned = cleaned[2..];
        }

        return cleaned.Length == CodeLength ? Prefix + cleaned : "";
    }

    public static bool IsValidFormat(string? code) =>
        !string.IsNullOrWhiteSpace(Normalize(code));
}