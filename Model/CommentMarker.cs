using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System;
using System.Text;

namespace Leo.Sdlxliff.Model;

public class CommentMarker : TranslationUnitContentContainer, IMarkerType
{
    public CommentDefinition CommentDefinition { get; set; } = new CommentDefinition();

    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.CommentsMarker;

    public override bool HasChildren => true;

    // Guid
    public override string Id { get; set; } = string.Empty;

    public string MarkerType => SdlxliffNames.XSdlComment;

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public override ITranslationUnitContent DeepCopy()
    {
        CommentMarker copy = new()
        {
            Id = Id
        };

        foreach (var content in Contents)
        {
            var child = content.DeepCopy();
            child.Parent = copy;
            copy.Contents.Add(child);
        }

        foreach (var comment in CommentDefinition.Comments)
        {
            copy.CommentDefinition.Comments.Add(comment.DeepCopy());
        }

        return copy;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder(@$"<mrk mtype=""{SdlxliffNames.XSdlComment}"" sdl:cid=""{Id}"">");

        foreach (var content in Contents)
        {
            stringBuilder.Append(content.ToString());
        }

        stringBuilder.Append("</mrk>");

        return stringBuilder.ToString();
    }
}