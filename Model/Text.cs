using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Model;

public class Text : TranslationUnitContent, IText
{
    public Text(string contents)
    {
        Contents = contents;
    }

    public string Contents { get; set; }

    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.Text;

    public override bool HasChildren => false;

    public override string Id { get; set; } = "Text";

    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public override ITranslationUnitContent DeepCopy()
    {
        Text copy = new(Contents);

        return copy;
    }

    public override string ToString()
    {
        return Contents;
    }
}
