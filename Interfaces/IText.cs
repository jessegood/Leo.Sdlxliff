using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Interfaces;

public interface IText
{
    string Contents { get; set; }
    TranslationUnitContentType ContentType { get; }
    bool HasChildren { get; }
    string Id { get; set; }
    TranslationUnitContentContainer? Parent { get; set; }

    void RemoveFromParent();
    string ToString();
}
