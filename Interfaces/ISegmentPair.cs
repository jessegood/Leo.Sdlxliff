using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Interfaces;

public interface ISegmentPair
{
    ConfirmationLevel ConfirmationLevel { get; set; }
    string Id { get; set; }
    bool Locked { get; set; }
    string Origin { get; set; }
    string OriginSystem { get; set; }
    ITranslationUnit? Parent { get; set; }
    byte Percent { get; set; }
    PreviousOrigin? Previous { get; set; }
    string RepetitionId { get; set; }
    RepetitionInfo RepetitionInfo { get; set; }
    string? SegmentDefinitionId { get; set; }
    ISegment SourceSegment { get; set; }
    bool StructMatch { get; set; }
    ISegment TargetSegment { get; set; }
    TextContextMatchLevel TextMatch { get; set; }
    IDictionary<string, string> Values { get; set; }
}
