using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System;
using System.Text;

namespace Leo.Sdlxliff.Model;

/// <summary>
/// A custom <c>mrk</c> element. Trados studio marker types are prefixed with <c>x-sdl-</c>.
/// </summary>
public class CustomMarker : TranslationUnitContentContainer, IMarkerType
{
    public CustomMarker(string mtype)
    {
        MarkerType = mtype;
    }

    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.CustomMarker;

    public override bool HasChildren => true;

    public override string Id { get; set; } = string.Empty;

    public string MarkerType { get; set; } = string.Empty;

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public override ITranslationUnitContent DeepCopy()
    {
        CustomMarker copy = new(MarkerType)
        {
            Id = Id
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
        var stringBuilder = new StringBuilder();
        if (Contents.Count > 0)
        {
            stringBuilder.Append(@$"<mrk mtype=""{MarkerType}"" mid=""{Id}"">");
            foreach (var content in Contents)
            {
                stringBuilder.Append(content.ToString());
            }
            stringBuilder.Append("</mrk>");
        }
        else
        {
            stringBuilder.Append(@$"<mrk mtype=""{MarkerType}"" mid=""{Id}""/>");
        }

        return stringBuilder.ToString();
    }
}
