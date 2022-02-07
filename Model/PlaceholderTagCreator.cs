using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Xml;

namespace Leo.Sdlxliff.Model;

public static class PlaceholderTagCreator
{
    public static LockedContent CreateLockedContent((string, string) ids, ITranslationUnit lockedTranslationUnit)
    {
        return new LockedContent()
        {
            Id = ids.Item1,
            XId = ids.Item2,
            LockedTranslationUnit = lockedTranslationUnit,
        };
    }

    public static ITranslationUnitContent CreatePlaceholder(string id, PlaceholderInfo placeholder)
    {
        return new Placeholder()
        {
            Id = id,
            IsWhiteSpace = placeholder.IsWhiteSpace,
            CanHide = placeholder.CanHide,
            Contents = placeholder.Contents,
            EquivalentText = placeholder.EquivalentText,
            WordEnd = placeholder.WordEnd,
            LineWrap = placeholder.LineWrap,
            Name = placeholder.Name,
            Properties = placeholder.Properties,
            SegmentationHint = placeholder.SegmentationHint,
            SubSegments = placeholder.SubSegments,
        };
    }

    public static StructureTag CreateStructureTag((string, string) ids, StructureTagInfo structureTag)
    {
        return new StructureTag()
        {
            Id = ids.Item1,
            XId = ids.Item2,
            Contents = structureTag.Contents,
            Name = structureTag.Name,
            Properties = structureTag.Properties,
            SubSegments = structureTag.SubSegments
        };
    }
}
