namespace Leo.Sdlxliff.Model.Common;

/// <summary>
/// Used with textual context matches to indicate the level of
/// matching.
/// </summary>
public enum TextContextMatchLevel
{
    /// <summary>
    /// The textual context does not match.
    /// </summary>
    None,

    /// <summary>
    /// Source content matches with surrounding source content.
    /// </summary>
    Source,

    /// <summary>
    /// Source and target matches surrounding source and target content.
    /// </summary>
    SourceAndTarget
}
