using Leo.Sdlxliff.Model.Xml;
using System;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model;

internal class TagInfoCreator
{
    private readonly IDictionary<string, FormattingDefinition> formattingDefinitions;

    private BeginPairedTag? beginPairedTag;

    private BeginPairedTagProperties? beginPairedTagProperties;

    private EndPairedTag? endPairedTag;

    private EndPairedTagProperties? endPairedTagProperties;

    private string? formatId;

    private PlaceholderInfo? placeholder;

    private Properties? properties;

    private StructureTagInfo? structureTag;

    internal TagInfoCreator(IDictionary<string, FormattingDefinition> formattingDefinitions)
    {
        this.formattingDefinitions = formattingDefinitions;
    }

    internal enum TagType
    {
        TagPair,
        Placeholder,
        StructureTag
    }

    internal TagType Type { get; set; } = TagType.TagPair;

    internal void AddBeginPairedTag(BeginPairedTag beginPairedTag)
    {
        this.beginPairedTag = beginPairedTag;
    }

    internal void AddBeginPairedTagProperties(BeginPairedTagProperties beginPairedTagProperties)
    {
        this.beginPairedTagProperties = beginPairedTagProperties;
    }

    internal void AddEndPairedTag(EndPairedTag endPairedTag)
    {
        this.endPairedTag = endPairedTag;
    }

    internal void AddEndPairedTagProperties(EndPairedTagProperties endPairedTagProperties)
    {
        this.endPairedTagProperties = endPairedTagProperties;
    }

    internal void AddFormatId(string formatId)
    {
        this.formatId = formatId;

        if (formattingDefinitions.TryGetValue(this.formatId, out var formattingDefinition))
        {
            if (beginPairedTag != null)
            {
                beginPairedTag.FormattingDefinition = formattingDefinition;
            }
        }
    }

    internal void AddPlaceholderInfo(PlaceholderInfo placeholder)
    {
        this.placeholder = placeholder;
        Type = TagType.Placeholder;
    }

    internal void AddProperties(Properties properties)
    {
        this.properties = properties;
    }

    internal void AddStructureTagInfo(StructureTagInfo structureTag)
    {
        this.structureTag = structureTag;
        Type = TagType.StructureTag;
    }

    internal PlaceholderInfo CreatePlaceholderInfo()
    {
        if (placeholder == null)
        {
            throw new InvalidOperationException("Cannot create placeholder information");
        }

        if (properties != null)
        {
            placeholder.Properties = properties;
        }

        return placeholder;
    }

    internal StructureTagInfo CreateStructureTagInfo()
    {
        if (structureTag == null)
        {
            throw new InvalidOperationException("Cannot create structure tag information");
        }

        if (properties != null)
        {
            structureTag.Properties = properties;
        }

        return structureTag;
    }

    internal TagPairInfo CreateTagPairInfo()
    {
        return new TagPairInfo()
        {
            BeginPairedTag = beginPairedTag ?? new BeginPairedTag(),
            BeginPairedTagProperties = beginPairedTagProperties ?? new BeginPairedTagProperties(),
            EndPairedTag = endPairedTag ?? new EndPairedTag(),
            EndPairedTagProperties = endPairedTagProperties ?? new EndPairedTagProperties(),
            FormatId = formatId ?? string.Empty,
        };
    }
}
