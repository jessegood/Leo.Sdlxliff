using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>sdl:seg</c> element. Child of <c>sdl:seg-defs</c>.
/// </summary>
public class SegmentDefinition : IValueElement, IOriginInformation
{
    public ConfirmationLevel ConfirmationLevel { get; set; } = ConfirmationLevel.Unspecified;
    public string Id { get; set; } = string.Empty;
    public bool Locked { get; set; } = false;
    public string Origin { get; set; } = string.Empty;
    public string OriginSystem { get; set; } = string.Empty;
    public byte Percent { get; set; } = 0;
    public PreviousOrigin? Previous { get; set; }
    public string RepetitionId { get; set; } = string.Empty;
    public bool StructMatch { get; set; } = false;
    public TextContextMatchLevel TextMatch { get; set; } = TextContextMatchLevel.None;
    public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();

    public SegmentDefinition DeepCopy()
    {
        SegmentDefinition copy = new()
        {
            ConfirmationLevel = ConfirmationLevel,
            Id = Id,
            Locked = Locked,
            Origin = Origin,
            OriginSystem = OriginSystem,
            Percent = Percent,
            Previous = Previous?.DeepCopy(),
            RepetitionId = RepetitionId,
            StructMatch = StructMatch,
            TextMatch = TextMatch
        };

        foreach (var pair in Values)
        {
            copy.Values.Add(pair.Key, pair.Value);
        }

        return copy;
    }
}