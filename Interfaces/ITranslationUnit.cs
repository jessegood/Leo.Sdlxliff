using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Interfaces;

public interface ITranslationUnit
{
    ContextInformation? ContextInformation { get; }
    bool HasSegmentPairs { get; }
    bool IsLocked { get; }
    bool IsStructure { get; }
    LockType LockType { get; }
    SegmentationSource SegmentationSource { get; }
    Source Source { get; }
    Target Target { get; }
    Translate Translate { get; }
    string TranslationUnitId { get; set; }
    ISegmentPair GetSegmentPairById(string id);
    ICollection<ISegmentPair> GetSegmentPairs();
}
