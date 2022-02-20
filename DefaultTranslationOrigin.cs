namespace Leo.Sdlxliff;

public static class DefaultTranslationOrigin
{
    /// <summary>
    /// The segment has been manually adapted or translated from scratch.
    /// </summary>
    public const string Interactive = "interactive";

    /// <summary>
    /// Batch translation by applying a Context TM type tool
    /// like PerfectMatch(tm)
    /// </summary>
    public const string DocumentMatch = "document-match";

    /// <summary>
    /// Machine translated content
    /// </summary>
    public const string MachineTranslation = "mt";

    /// <summary>
    /// Batch pre-translation using a fuzzy or 100% match
    /// </summary>
    public const string TranslationMemory = "tm";

    /// <summary>
    /// The segment has not yet been translated. This is usually an empty segment.
    /// </summary>
    public const string NotTranslated = "not-translated";

    /// <summary>
    /// The segment has been translated using AutoPropagation from internal matches.
    /// </summary>
    public const string AutoPropagated = "auto-propagated";

    /// <summary>
    /// The segment has been translated by copying the source to the target.
    /// </summary>
    public const string Source = "source";

    /// <summary>
    /// The translated segment was created by an automated linguistic alignment of previously
    /// translated source and target content.
    /// </summary>
    public const string AutomatedAlignment = "auto-aligned";

    /// <summary>
    /// The segment was translated by an unknown tool - usually from a third party provider.
    /// </summary>
    public const string Unknown = "unknown";

    /// <summary>
    /// The segment was updated by ReverseAlignment process.
    /// </summary>
    public const string ReverseAlignment = "Retrofit";

    /// <summary>
    /// Adaptive machine translated content
    /// </summary>
    public const string AdaptiveMachineTranslation = "amt";

    /// <summary>
    /// Adaptive machine translated content
    /// </summary>
    public const string NeuralMachineTranslation = "nmt";

    /// <summary>
    /// Adaptive machine translated content
    /// </summary>
    public const string AutomaticTranslation = "automatic-translation";
}
