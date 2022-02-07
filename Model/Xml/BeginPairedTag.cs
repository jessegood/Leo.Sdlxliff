using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>bpt</c> element. Child of <c>tag</c>.
/// </summary>
public class BeginPairedTag : CommonTagAttributes, ISegmentationHint, ISubSegment
{
    public string Contents { get; set; } = string.Empty;

    /// <summary>
    /// Formatting definition associated with the tag pair.
    /// This is used for display purposes in the editor.
    /// </summary>
    public FormattingDefinition? FormattingDefinition { get; set; }

    public SegmentationHint SegmentationHint { get; set; } = SegmentationHint.MayExclude;

    public IList<SubSegment> SubSegments { get; set; } = new List<SubSegment>();

    public BeginPairedTag DeepCopy()
    {
        BeginPairedTag copy = new()
        {
            LineWrap = LineWrap,
            CanHide = CanHide,
            Name = Name,
            WordEnd = WordEnd,
            Contents = Contents,
            FormattingDefinition = FormattingDefinition?.DeepCopy(),
            SegmentationHint = SegmentationHint
        };

        foreach (var subSegment in SubSegments)
        {
            copy.SubSegments.Add(new SubSegment(subSegment.Id, subSegment.Length, subSegment.Offset));
        }

        return copy;
    }
}