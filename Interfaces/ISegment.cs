using Leo.Sdlxliff.Model.Common;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Interfaces;

public interface ISegment
{
    IList<ITranslationUnitContent> Contents { get; set; }

    TranslationUnitContentType ContentType { get; }

    bool HasChildren { get; }

    string Id { get; set; }

    TranslationUnitContentContainer? Parent { get; set; }

    SegmentType SegmentType { get; }

    IEnumerable<ITranslationUnitContent> AllContent();

    bool Contains(string searchString, bool caseSensitive, bool useRegex, bool searchInTags);
    
    ITranslationUnitContent DeepCopy();

    void RemoveFromParent();
}
