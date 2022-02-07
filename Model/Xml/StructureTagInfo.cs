using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>st</c> element. Child of <c>tag</c> element in the header.
/// A structural tag does not appear as localizable content.
/// </summary>
public class StructureTagInfo : ISubSegment
{
    public string Contents { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Properties Properties { get; set; } = new Properties();
    public IList<SubSegment> SubSegments { get; set; } = new List<SubSegment>();
}
