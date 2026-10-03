namespace YallaArabi.Domain.Common;

public static class AudioSources
{
    public const string Human = "human";
    public const string Ai = "ai";

    public static bool IsSupported(string? source) =>
        source == Human || source == Ai;
}