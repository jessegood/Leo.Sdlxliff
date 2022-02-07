using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Helpers;

/// <summary>
/// Helper class to perform segment searches
/// </summary>
public static class SegmentSearchHelper
{
    public static bool Search(string? sourceSearchString, string? targetSearchString, ISegmentPair segmentPair, SegmentSearchSettings settings)
    {
        bool isMatch = settings.IsLogicalAnd;

        if (settings.SegmentStatus != null)
        {
            isMatch = CheckRepetitionStatus(segmentPair, settings.SegmentStatus.RepetitionStatus, isMatch);
        }

        isMatch = DoSettingsMatchSegmentPairProperties(segmentPair, settings, isMatch);

        isMatch = CheckCommentsAndRevisions(segmentPair, settings, isMatch);

        if (string.IsNullOrEmpty(sourceSearchString) && string.IsNullOrEmpty(targetSearchString))
        {
            return isMatch;
        }

        if (ShouldContinue(isMatch, settings.IsLogicalAnd) && !string.IsNullOrEmpty(sourceSearchString))
        {
            isMatch = segmentPair.SourceSegment.Contains(sourceSearchString, settings.CaseSensitive, settings.UseRegularExpressions, settings.SearchInTags);
        }

        if (ShouldContinue(isMatch, settings.IsLogicalAnd) && !string.IsNullOrEmpty(targetSearchString))
        {
            isMatch = segmentPair.TargetSegment.Contains(targetSearchString, settings.CaseSensitive, settings.UseRegularExpressions, settings.SearchInTags);
        }

        return isMatch;
    }

    private static bool CheckCommentsAndRevisions(ISegmentPair segmentPair, SegmentSearchSettings settings, bool isMatch)
    {
        if (settings.WithComments && ShouldContinue(isMatch, settings.IsLogicalAnd))
        {
            isMatch = ContentLookupHelper.HasComment(segmentPair.SourceSegment.AllContent()) ||
                ContentLookupHelper.HasComment(segmentPair.TargetSegment.AllContent());
        }

        if (settings.WithRevisions && ShouldContinue(isMatch, settings.IsLogicalAnd))
        {
            isMatch = ContentLookupHelper.HasRevision(segmentPair.SourceSegment.AllContent()) ||
                ContentLookupHelper.HasRevision(segmentPair.TargetSegment.AllContent());
        }

        return isMatch;
    }

    private static bool CheckRepetitionStatus(ISegmentPair segmentPair, RepetitionInfo repetitionStatus, bool isMatch)
    {
        // All repetitions
        if (repetitionStatus.HasFlag(RepetitionInfo.FirstOccurence) && repetitionStatus.HasFlag(RepetitionInfo.NotFirstOccurence))
        {
            isMatch = segmentPair.RepetitionInfo == RepetitionInfo.FirstOccurence ||
                segmentPair.RepetitionInfo == RepetitionInfo.NotFirstOccurence;
        }
        // Unique occurences (excludes not first occurences)
        else if (repetitionStatus.HasFlag(RepetitionInfo.FirstOccurence) && repetitionStatus.HasFlag(RepetitionInfo.Unique))
        {
            isMatch = segmentPair.RepetitionInfo == RepetitionInfo.FirstOccurence ||
                segmentPair.RepetitionInfo == RepetitionInfo.Unique;
        }
        // First occurence only
        else if (repetitionStatus.HasFlag(RepetitionInfo.FirstOccurence))
        {
            isMatch = segmentPair.RepetitionInfo == RepetitionInfo.FirstOccurence;
        }
        // Not first occurence only
        else if (repetitionStatus.HasFlag(RepetitionInfo.NotFirstOccurence))
        {
            isMatch = segmentPair.RepetitionInfo == RepetitionInfo.NotFirstOccurence;
        }

        return isMatch;
    }

    private static bool DoesMatchStatusSearchSettingsMatch(ISegmentPair segmentPair, SegmentStatus.MatchLevel matchStatus)
    {
        bool isMatch = false;
        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.PerfectMatch))
        {
            isMatch = IsPerfectMatch(segmentPair);
        }

        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.ContextMatch))
        {
            isMatch = IsContextMatch(segmentPair);
        }

        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.AutomatedTranslation))
        {
            isMatch = segmentPair.Origin == DefaultTranslationOrigin.AutomaticTranslation
                || segmentPair.Origin == DefaultTranslationOrigin.MachineTranslation;
        }

        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.NeuralMachineTranslation))
        {
            isMatch = segmentPair.Origin == DefaultTranslationOrigin.NeuralMachineTranslation;
        }

        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.Interactive))
        {
            isMatch = segmentPair.Origin == DefaultTranslationOrigin.Interactive;
        }

        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.CopiedFromSource))
        {
            isMatch = segmentPair.Origin == DefaultTranslationOrigin.Source;
        }

        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.AutoPropagated))
        {
            isMatch = segmentPair.Origin == DefaultTranslationOrigin.AutoPropagated;
        }

        if (!isMatch && matchStatus.HasFlag(SegmentStatus.MatchLevel.FuzzyMatchRepair))
        {
            isMatch = (segmentPair.Origin == DefaultTranslationOrigin.TranslationMemory) &&
                    segmentPair.Values.ContainsKey("FuzzyMatchRepair");
        }

        return isMatch;
    }

    private static bool DoLockSearchSettingsMatch(bool locked, SegmentStatus.LockSearchSettings lockedStatus)
    {
        return lockedStatus switch
        {
            SegmentStatus.LockSearchSettings.Unlocked when !locked => true,
            SegmentStatus.LockSearchSettings.Locked when locked => true,
            _ => false
        };
    }

    private static bool DoSettingsMatchSegmentPairProperties(ISegmentPair segmentPair, SegmentSearchSettings settings, bool isMatch)
    {
        if (settings.SegmentStatus != null)
        {
            var status = settings.SegmentStatus;
            if (status.ConfirmationLevel != ConfirmationLevel.Unspecified &&
                ShouldContinue(isMatch, settings.IsLogicalAnd))
            {
                isMatch = segmentPair.ConfirmationLevel.HasFlag(status.ConfirmationLevel);
            }

            if (status.LockedStatus != SegmentStatus.LockSearchSettings.Unspecified &&
                ShouldContinue(isMatch, settings.IsLogicalAnd))
            {
                isMatch = DoLockSearchSettingsMatch(segmentPair.Locked, status.LockedStatus);
            }

            if (status.MatchPercentage != null && ShouldContinue(isMatch, settings.IsLogicalAnd))
            {
                isMatch = status.MatchPercentage.Invoke(segmentPair.Percent);

                if (isMatch && segmentPair.Percent >= 100)
                {
                    if (IsContextMatch(segmentPair))
                    {
                        isMatch = status.MatchStatus.HasFlag(SegmentStatus.MatchLevel.ContextMatch);
                    }
                    else if (IsPerfectMatch(segmentPair))
                    {
                        isMatch = status.MatchStatus.HasFlag(SegmentStatus.MatchLevel.PerfectMatch);
                    }
                }
            }

            if (ShouldContinue(isMatch, settings.IsLogicalAnd) && status.MatchStatus != SegmentStatus.MatchLevel.None)
            {
                isMatch = DoesMatchStatusSearchSettingsMatch(segmentPair, status.MatchStatus);
            }
        }

        return isMatch;
    }

    public static bool IsContextMatch(ISegmentPair segmentPair)
    {
        if (segmentPair.Origin != DefaultTranslationOrigin.DocumentMatch && segmentPair.Percent >= 100)
        {
            return segmentPair.TextMatch == TextContextMatchLevel.SourceAndTarget;
        }

        return false;
    }

    public static bool IsPerfectMatch(ISegmentPair segmentPair)
    {
        return segmentPair.Origin == DefaultTranslationOrigin.DocumentMatch;
    }

    private static bool ShouldContinue(bool isMatch, bool isAndLogicalAnd)
    {
        if (isAndLogicalAnd)
        {
            return isMatch;
        }

        return !isMatch;
    }
}
