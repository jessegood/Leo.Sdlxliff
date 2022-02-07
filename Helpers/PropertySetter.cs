using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Helpers;

public static class PropertySetter
{
    public static void SetTagDefinitionsForTagPair(TagPair tagPair, TagDefinitions tagDefinitions, IDictionary<string, RevisionDefinition> revisionDefinitions)
    {
        if (tagDefinitions.TagPairInfo.TryGetValue(tagPair.Id, out var tagPairInfo))
        {
            SetTagPairProperties(tagPair, tagPairInfo);
        }

        if (revisionDefinitions.TryGetValue(tagPair.StartRevisionId, out var startRevisionDefinition))
        {
            tagPair.StartTagRevisionDefinition = startRevisionDefinition;
        }

        if (revisionDefinitions.TryGetValue(tagPair.EndRevisionId, out var endRevisionDefinition))
        {
            tagPair.EndTagRevisionDefinition = endRevisionDefinition;
        }
    }

    private static void SetTagPairProperties(TagPair tagPair, TagPairInfo tagPairInfo)
    {
        tagPair.BeginPairedTag = tagPairInfo.BeginPairedTag;
        tagPair.SubSegments = tagPairInfo.BeginPairedTag.SubSegments;
        tagPair.BeginPairedTagProperties = tagPairInfo.BeginPairedTagProperties;
        tagPair.EndPairedTag = tagPairInfo.EndPairedTag;
        tagPair.EndPairedTagProperties = tagPairInfo.EndPairedTagProperties;
        tagPair.FormatId = tagPairInfo.FormatId;
    }
}