using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Interfaces;

/// <summary>
/// A subsegment is localizable content of a tag. For example, when you want to translate the attribute values.
/// The element name is <c>sub</c>.
/// The <c>bpt</c>, <c>ph</c> and <c>st</c> tags can contain subsegments.
/// </summary>
public interface ISubSegment
{
    /// <summary>
    /// List of subsegments for this tag
    /// </summary>
    public IList<SubSegment> SubSegments { get; }
}
