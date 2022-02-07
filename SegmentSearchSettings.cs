using Leo.Sdlxliff.Model.Common;
using System;

namespace Leo.Sdlxliff;

/// <summary>
/// Settings used when searching for segments
/// </summary>
public class SegmentSearchSettings
{
    /// <summary>
    /// Whether to perform a case sensitive search or not.
    /// The default search is case insensitive.
    /// </summary>
    public bool CaseSensitive { get; set; } = false;

    /// <summary>
    /// Whether to use and/or relationship when searching
    /// </summary>
    public bool IsLogicalAnd { get; set; } = true;

    /// <summary>
    /// Whether to search in tags or not.
    /// The default is false.
    /// </summary>
    public bool SearchInTags { get; set; } = false;

    /// <summary>
    /// Set this when you want to search by the status of the segment.
    /// For example, the match percentage or when it is locked or not.
    /// </summary>
    public SegmentStatus? SegmentStatus { get; set; }

    /// <summary>
    /// Whether to turn on regular expression searching or not.
    /// The default is false.
    /// </summary>
    public bool UseRegularExpressions { get; set; } = false;

    /// <summary>
    /// Set this to return segments with comments
    /// </summary>
    public bool WithComments { get; set; } = false;

    /// <summary>
    /// Set this to return segments with revisions
    /// </summary>
    public bool WithRevisions { get; set; } = false;
}

public class SegmentStatus
{
    public enum LockSearchSettings
    {
        Locked,
        Unlocked,
        Unspecified
    }

    [Flags]
    public enum MatchLevel
    {
        None = 0,
        PerfectMatch = 1,
        ContextMatch = 2,
        AutomatedTranslation = 4,
        NeuralMachineTranslation = 8,
        Interactive = 16,
        CopiedFromSource = 32,
        AutoPropagated = 64,
        FuzzyMatchRepair = 128
    }

    /// <summary>
    /// Search by the confirmation level.
    /// Multiple confirmation levels can be searched for.
    /// </summary>
    public ConfirmationLevel ConfirmationLevel { get; set; } = ConfirmationLevel.Unspecified;

    /// <summary>
    /// Search setting to search depending on the locked status
    /// </summary>
    public LockSearchSettings LockedStatus { get; set; } = LockSearchSettings.Unspecified;

    /// <summary>
    /// Can be used to search based on match percentage.
    /// Pass a lambda, etc. used for comparison.
    /// </summary>
    public Predicate<byte>? MatchPercentage { get; set; }

    /// <summary>
    /// Search by the match level, such as perfect match, context match, automated translation, etc.
    /// </summary>
    public MatchLevel MatchStatus { get; set; } = MatchLevel.None;

    /// <summary>
    /// Search by the repetition status, first occurence or other occurences
    /// </summary>
    public RepetitionInfo RepetitionStatus { get; set; } = RepetitionInfo.None;
}
