using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model;

public class Placeholder : TranslationUnitContent, ISubSegment
{
    /// <summary>
    /// Indicates whether the tag is allowed to be hidden during editing operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Hidden tags can easily get deleted by mistake during editing operations.
    /// Always set this to <c>false</c> for tags that represent important content that the
    /// user should not delete unknowingly.
    /// </para>
    /// Setting this to <c>true</c> does not necessarily mean that the tag will always be hidden.
    /// This will be determined by the editor (i.e. the user may change this at runtime through a setting).
    /// </para>
    /// <para>This is <c>false</c> by default for all tag types.</para>
    /// </remarks>
    public bool CanHide { get; set; } = false;

    public string Contents { get; set; } = string.Empty;

    /// <summary>
    /// For <c>x</c> elements. There are three possible content types:
    /// 1. Placeholder
    /// 2. StructureTag
    /// 3. LockedContent
    /// </summary>
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.Placeholder;

    public string EquivalentText { get; set; } = string.Empty;

    /// <summary>
    /// <c>x</c> tags are self-closing, so this is always false.
    /// </summary>
    public override bool HasChildren => false;

    /// <summary>
    /// This id is used to look up the corresponding <c>tag</c> in the header.
    /// For locked content, this id starts with "locked" and refers to a previous translation unit.
    /// </summary>
    public override string Id { get; set; } = string.Empty;

    /// <summary>
    /// If this is true, the ph element can be treated as normal whitespace,
    /// i.e. a line break can be used at this position
    /// </summary>
    public bool IsWhiteSpace { get; set; } = false;

    /// <summary>
    /// Indicates whether it is valid to break the line in front of this tag during word wrap.
    /// </summary>
    /// <remarks>
    /// <para>This is <c>true</c> by default for all tag types.</para>
    /// </remarks>
    public bool LineWrap { get; set; } = true;

    /// <summary>
    /// A shortened version of a tag that can be used in translation editors
    /// that implement full and abbreviated tag views. Often the GI (Generic Identifier) without attributes.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public Properties Properties { get; set; } = new Properties();

    public SegmentationHint SegmentationHint { get; set; } = SegmentationHint.MayExclude;

    /// <summary>
    /// Contains <c>sub</c> elements for localization, such as the values of attributes.
    /// </summary>
    public IList<SubSegment> SubSegments { get; set; } = new List<SubSegment>();

    /// <summary>
    /// Determines whether the tag is considered to be a new word during segmentation. It also affects the way that the
    /// caret moves when using word movement keyboard actions such as CTRL+LEFT ARROW and CTRL+RIGHT ARROW in editors.
    /// </summary>
    /// <example>
    /// During segmentation, if a tag has IsWordStop false then "hello[tag]world[/tag]" is considered to be one word - "hello[tag]world[/tag]" and
    /// if a tag has IsWordStop true then "hello[tag]world[/tag]" is considered to be two words - "hello" and "[tag]world[/tag]".
    /// </example>
    /// <para>This is <c>true</c> by default for placeholder tags, and <c>false</c> by default
    /// for start and end tags.</para>
    public bool WordEnd { get; set; } = false;

    /// <summary>
    /// The <c>xid</c> is present when referring to locked content.
    /// This is empty otherwise.
    /// </summary>
    public string XId { get; set; } = string.Empty;

    public override ITranslationUnitContent DeepCopy()
    {
        Placeholder copy = new()
        {
            Id = Id,
            CanHide = CanHide,
            Contents = Contents,
            ContentType = ContentType,
            EquivalentText = EquivalentText,
            LineWrap = LineWrap,
            Name = Name,
            Properties = Properties.DeepCopy(),
            SegmentationHint = SegmentationHint,
            SubSegments = CopySubSegments(),
            WordEnd = WordEnd,
            XId = XId
        };

        return copy;
    }

    public override string ToString()
    {
        return Contents;
    }

    private IList<SubSegment> CopySubSegments()
    {
        List<SubSegment> subSegments = new();

        foreach (SubSegment subSegment in SubSegments)
        {
            subSegments.Add(new SubSegment(subSegment.Id, subSegment.Length, subSegment.Offset));
        }

        return subSegments;
    }
}
