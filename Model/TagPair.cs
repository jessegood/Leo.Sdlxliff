using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;
using System.Text;

namespace Leo.Sdlxliff.Model;

public class TagPair : TranslationUnitContentContainer, ISubSegment
{
    public BeginPairedTag BeginPairedTag { get; set; } = new BeginPairedTag();

    public BeginPairedTagProperties BeginPairedTagProperties { get; set; } = new BeginPairedTagProperties();

    /// <summary>
    /// <c>g</c> elements always use the PairedTag content type
    /// </summary>
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.PairedTag;

    /// <summary>
    /// <para>When true, the end tag of the tag pair has ghost status.</para>
    /// <para>
    /// Ghost tags occur when either the start tag or the end tag of a tag pair has been deleted.
    /// Such operations would leave the tag pair in an invalid state until either the remaining start or end
    /// tag has been removed too (provided that the entire tag pair can be deleted properly). SDL Trados Studio supports
    /// this behavior in the editor by leaving the tag pair in place until both start
    /// and end tags have been deleted. While only one tag of a pair has been deleted, the "deleted" tag is
    /// temporarily marked as a (transparent) "ghost" tag, but retains its position.
    /// </para>
    /// </summary>
    public bool End { get; set; } = false;

    public EndPairedTag EndPairedTag { get; set; } = new EndPairedTag();

    public EndPairedTagProperties EndPairedTagProperties { get; set; } = new EndPairedTagProperties();

    /// <summary>
    /// An id referring to any revision information related to the end tag.
    /// </summary>
    public string EndRevisionId { get; set; } = string.Empty;

    /// <summary>
    /// Revision information directly associated with the end tag.
    /// Can be null.
    /// </summary>
    public RevisionDefinition? EndTagRevisionDefinition { get; set; }

    public string FormatId { get; set; } = string.Empty;

    /// <summary>
    /// True if this element contains content
    /// </summary>
    public override bool HasChildren => Contents.Count > 0;

    /// <summary>
    /// This id is used to look up the corresponding <c>tag</c> in the header.
    /// </summary>
    public override string Id { get; set; } = string.Empty;

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    /// <summary>
    /// <para>When true, the start tag of the tag pair has ghost status.</para>
    /// <para>
    /// Ghost tags occur when either the start tag or the end tag of a tag pair has been deleted.
    /// Such operations would leave the tag pair in an invalid state until either the remaining start or end
    /// tag has been removed too (provided that the entire tag pair can be deleted properly). SDL Trados Studio supports
    /// this behavior in the editor by leaving the tag pair in place until both start
    /// and end tags have been deleted. While only one tag of a pair has been deleted, the "deleted" tag is
    /// temporarily marked as a (transparent) "ghost" tag, but retains its position.
    /// </para>
    /// </summary>
    public bool Start { get; set; } = false;

    /// <summary>
    /// An id referring to any revision information related to the start tag.
    /// </summary>
    public string StartRevisionId { get; set; } = string.Empty;

    /// <summary>
    /// Revision information that is directly associated with the start tag.
    /// Can be null.
    /// </summary>
    public RevisionDefinition? StartTagRevisionDefinition { get; set; }

    /// <summary>
    /// Contains <c>sub</c> elements for localization, such as the values of attributes.
    /// </summary>
    public IList<SubSegment> SubSegments { get; set; } = new List<SubSegment>();

    public override ITranslationUnitContent DeepCopy()
    {
        TagPair copy = new()
        {
            BeginPairedTag = BeginPairedTag.DeepCopy(),
            BeginPairedTagProperties = BeginPairedTagProperties.DeepCopy(),
            EndPairedTag = new EndPairedTag() { Contents = EndPairedTag.Contents },
            EndPairedTagProperties = EndPairedTagProperties.DeepCopy(),
            End = End,
            EndRevisionId = EndRevisionId,
            EndTagRevisionDefinition = EndTagRevisionDefinition?.DeepCopy(),
            FormatId = FormatId,
            StartTagRevisionDefinition = StartTagRevisionDefinition?.DeepCopy(),
            Id = Id,
            Start = Start,
            StartRevisionId = StartRevisionId
        };

        foreach (var subSegment in SubSegments)
        {
            copy.SubSegments.Add(new SubSegment(subSegment.Id, subSegment.Length, subSegment.Offset));
        }

        foreach (var content in Contents)
        {
            var child = content.DeepCopy();
            child.Parent = copy;
            copy.Contents.Add(child);
        }

        return copy;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder(BeginPairedTag.Contents);
        foreach (var content in Contents)
        {
            stringBuilder.Append(content.ToString());
        }

        stringBuilder.Append(EndPairedTag.Contents);

        return stringBuilder.ToString();
    }
}