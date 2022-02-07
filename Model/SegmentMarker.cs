using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Text;

namespace Leo.Sdlxliff.Model;

public class SegmentMarker : TranslationUnitContentContainer, IMarkerType
{
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.Segment;
    public override bool HasChildren => true;

    // This id refers to the row number shown in the editor
    public override string Id { get; set; } = string.Empty;

    public string MarkerType => SdlxliffNames.Seg;

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public SegmentDefinition SegmentDefinition { get; set; } = new SegmentDefinition();

    public override ITranslationUnitContent DeepCopy()
    {
        SegmentMarker copy = new()
        {
            Id = Id,
            SegmentDefinition = SegmentDefinition.DeepCopy()  
        };

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
        var stringBuilder = new StringBuilder(@$"<mrk mtype=""{SdlxliffNames.Seg}"" mid=""{Id}"">");
        foreach (var content in Contents)
        {
            stringBuilder.Append(content.ToString());
        }
        stringBuilder.Append("</mrk>");

        return stringBuilder.ToString();
    }
}
