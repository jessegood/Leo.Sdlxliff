using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Xml;
using System;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Helpers;
public static class TranslationUnitExtensions
{
    public static ITranslationUnit DeepCopy(this ITranslationUnit translationUnit)
    {
        var builder = new TranslationUnit.TranslationUnitBuilder(new Dictionary<string, IList<Entry>>());

        builder.SetSource(translationUnit.Source);
        builder.SetTarget(translationUnit.Target);
        builder.SetSegmentationSource(translationUnit.SegmentationSource);
        builder.SetLockType(translationUnit.LockType);
        builder.SetTranslationUnitId($"lockTU_{Guid.NewGuid()}");
        foreach (var sp in translationUnit.GetSegmentPairs())
        {
            var source = sp.SourceSegment;
            var target = sp.TargetSegment;
            builder.SetSourceSegment(source.Id, source, CreateSegmentDefinition(sp));
            builder.SetTargetSegment(target.Id, target, CreateSegmentDefinition(sp));
        }

        return builder.Build();
    }

    private static SegmentDefinition CreateSegmentDefinition(ISegmentPair segmentPair)
    {
        var segDef = new SegmentDefinition()
        {
            ConfirmationLevel = segmentPair.ConfirmationLevel,
            Locked = segmentPair.Locked,
            Origin = segmentPair.Origin,
            OriginSystem = segmentPair.OriginSystem,
            Percent = segmentPair.Percent,
            Previous = segmentPair.Previous,
            RepetitionId = segmentPair.RepetitionId,
            Id = segmentPair.SegmentDefinitionId ?? String.Empty,
            StructMatch = segmentPair.StructMatch,
            TextMatch = segmentPair.TextMatch,
        };

        foreach (var pair in segmentPair.Values)
        {
            segDef.Values.Add(pair);
        }

        return segDef;
    }
}
