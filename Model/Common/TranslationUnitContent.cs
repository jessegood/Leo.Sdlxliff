using Leo.Sdlxliff.Interfaces;

namespace Leo.Sdlxliff.Model.Common;

public abstract class TranslationUnitContent : ITranslationUnitContent
{
    public abstract TranslationUnitContentType ContentType { get; internal set; }
    public abstract bool HasChildren { get; }
    public abstract string Id { get; set; }
    public int Index => Parent != null ? Parent.Contents.IndexOf(this) : -1;
    public abstract TranslationUnitContentContainer? Parent { get; set; }
    public abstract ITranslationUnitContent DeepCopy();

    public void RemoveFromParent()
    {
        Parent?.Contents.Remove(this);
    }
}
