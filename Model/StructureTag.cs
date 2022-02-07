using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model;

public class StructureTag : TranslationUnitContent, ISubSegment
{
    public string Contents { get; set; } = string.Empty;

    /// <summary>
    /// For <c>x</c> elements. There are three possible content types:
    /// 1. Placeholder
    /// 2. StructureTag
    /// 3. LockedContent
    /// </summary>
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.StructureTag;

    /// <summary>
    /// <c>x</c> tags are self-closing, so this is always false.
    /// </summary>
    public override bool HasChildren => false;

    /// <summary>
    /// This id is used to look up the corresponding <c>tag</c> in the header.
    /// For locked content, this id starts with "locked" and refers to a previous translation unit.
    /// </summary>
    public override string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public Properties Properties { get; set; } = new Properties();

    /// <summary>
    /// Contains <c>sub</c> elements for localization, such as the values of attributes.
    /// </summary>
    public IList<SubSegment> SubSegments { get; set; } = new List<SubSegment>();

    /// <summary>
    /// The <c>xid</c> is present when referring to locked content or a subsegment element id.
    /// This is empty otherwise.
    /// </summary>
    public string XId { get; set; } = string.Empty;

    public override ITranslationUnitContent DeepCopy()
    {
        StructureTag copy = new()
        {
            Id = Id,
            Name = Name,
            Properties = Properties.DeepCopy(),
            XId = XId
        };

        foreach (var subSegment in SubSegments)
        {
            copy.SubSegments.Add(new SubSegment(subSegment.Id, subSegment.Length, subSegment.Offset));
        }

        return copy;
    }

    public override string ToString()
    {
        return Contents;
    }
}