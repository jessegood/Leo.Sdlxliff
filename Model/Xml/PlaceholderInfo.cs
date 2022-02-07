using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>ph</c> element. Child of <c>tag</c>.
/// </summary>
public class PlaceholderInfo : CommonTagAttributes, ISegmentationHint, ISubSegment
{
    public string Contents { get; set; } = string.Empty;
    public string EquivalentText { get; set; } = string.Empty;

    /// <summary>
    /// If this is true, the ph element can be treated as normal whitespace,
    /// i.e. a line break can be used at this position.
    /// </summary>
    /// <remarks>
    /// <para>This is <c>false</c> by default.</para>
    /// </remarks>
    public bool IsWhiteSpace { get; set; } = false;

    public Properties Properties { get; set; } = new Properties();

    public SegmentationHint SegmentationHint { get; set; } = SegmentationHint.MayExclude;

    public IList<SubSegment> SubSegments { get; set; } = new List<SubSegment>();
}
