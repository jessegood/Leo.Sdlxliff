using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

public class TagDefinitions
{
    public IDictionary<string, PlaceholderInfo> PlaceholderInfo { get; } = new Dictionary<string, PlaceholderInfo>();
    public IDictionary<string, StructureTagInfo> StructureTagInfo { get; } = new Dictionary<string, StructureTagInfo>();
    public IDictionary<string, TagPairInfo> TagPairInfo { get; } = new Dictionary<string, TagPairInfo>();
}
