using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model;

public class SegmentPair : IValueElement, IOriginInformation, ISegmentPair
{
    public ConfirmationLevel ConfirmationLevel { get; set; } = ConfirmationLevel.Unspecified;
    public string Id { get; set; } = string.Empty;
    public bool Locked { get; set; } = false;
    public string Origin { get; set; } = string.Empty;
    public string OriginSystem { get; set; } = string.Empty;
    public ITranslationUnit? Parent { get; set; }
    public byte Percent { get; set; } = 0;
    public PreviousOrigin? Previous { get; set; }
    public string RepetitionId { get; set; } = string.Empty;
    public RepetitionInfo RepetitionInfo { get; set; } = RepetitionInfo.None;
    public string? SegmentDefinitionId { get; set; }
    public ISegment SourceSegment { get; set; } = new Segment();
    public bool StructMatch { get; set; } = false;
    public ISegment TargetSegment { get; set; } = new Segment();
    public TextContextMatchLevel TextMatch { get; set; } = TextContextMatchLevel.None;
    public IDictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
}
