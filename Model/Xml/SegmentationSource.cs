using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Model.Xml;

public class SegmentationSource : TranslationUnitContentContainer
{
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.SegmentSource;

    public override bool HasChildren => true;

    public override string Id { get; set; } = string.Empty;
    public override TranslationUnitContentContainer? Parent { get; set; }

    public override ITranslationUnitContent DeepCopy()
    {
        SegmentationSource copy = new()
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
}