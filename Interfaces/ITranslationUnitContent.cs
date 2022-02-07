using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Interfaces;

public interface ITranslationUnitContent
{
    public TranslationUnitContentType ContentType { get; }
    public bool HasChildren { get; }
    public string Id { get; set; }
    public int Index { get; }
    public TranslationUnitContentContainer? Parent { get; set; }
    public ITranslationUnitContent DeepCopy();
    public void RemoveFromParent();
}
