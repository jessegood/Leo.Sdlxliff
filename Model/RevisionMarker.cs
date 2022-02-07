using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Text;

namespace Leo.Sdlxliff.Model;

public class RevisionMarker : TranslationUnitContentContainer, IMarkerType
{
    public RevisionMarker(string mtype)
    {
        MarkerType = mtype;
    }

    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.RevisionMarker;
    public override bool HasChildren => true;

    public override string Id { get; set; } = string.Empty;

    public string MarkerType { get; }

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public RevisionDefinition RevisionDefinition { get; set; } = new RevisionDefinition();

    public override ITranslationUnitContent DeepCopy()
    {
        RevisionMarker copy = new(MarkerType)
        {
            Id = Id,
            RevisionDefinition = RevisionDefinition.DeepCopy()
        };

        return copy;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder(@$"<mrk mtype=""{MarkerType}"" sdl:revid=""{Id}"">");
        foreach (var content in Contents)
        {
            stringBuilder.Append(content.ToString());
        }
        stringBuilder.Append("</mrk>");

        return stringBuilder.ToString();
    }
}
