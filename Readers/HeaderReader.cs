using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Leo.Sdlxliff.Readers;

internal class HeaderReader
{
    private readonly IDictionary<string, FormattingDefinition> formattingDefinitions = new Dictionary<string, FormattingDefinition>();

    internal static IDictionary<string, ContextDefinition> HandleContextDefinitions(XElement element)
    {
        var contextDefinitions = new Dictionary<string, ContextDefinition>();

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.CxtDef:
                    var id = SdlxliffReaderHelper.GetIdAttribute(child);
                    var contextDefinition = new ContextDefinition(id);
                    HandleContextDefinition(contextDefinition, child);
                    contextDefinitions.Add(id, contextDefinition);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return contextDefinitions;
    }

    internal static FileInfo HandleFileInfo(XElement element)
    {
        var fileInfo = new FileInfo();
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Value:
                    SdlxliffReaderHelper.HandleValueElement(fileInfo, child, true);
                    break;

                case SdlxliffNames.SniffInfo:
                    HandleSniffInfo(fileInfo, child);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return fileInfo;
    }

    internal static FileTypeInfo HandleFileTypeInfo(XElement element)
    {
        var fileTypeInfo = new FileTypeInfo();
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.FileTypeId:
                    fileTypeInfo.FileTypeId = child.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
        return fileTypeInfo;
    }

    internal IDictionary<string, FormattingDefinition> HandleFormattingDefinitions(XElement element)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.FmtDef:
                    HandleFormattingDefinition(formattingDefinitions, child);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return formattingDefinitions;
    }

    internal static Dictionary<string, NodeDefinition> HandleNodeDefinitions(XElement element)
    {
        var nodeDefinitions = new Dictionary<string, NodeDefinition>();

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.NodeDef:
                    var id = SdlxliffReaderHelper.GetIdAttribute(child);
                    var nodeDefinition = new NodeDefinition(id);
                    HandleNodeDefinition(nodeDefinition, child);
                    nodeDefinitions.Add(id, nodeDefinition);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return nodeDefinitions;
    }

    internal static Reference HandleReference(XElement element)
    {
        var reference = new Reference();
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.InternalFile:
                    var internalFile = new InternalFile(child.Value);
                    HandleInternalFileAttributes(child, internalFile);
                    reference.InternalFile = internalFile;
                    break;

                case SdlxliffNames.ExternalFile:
                    var externalFile = new ExternalFile();
                    HandleExternalFileAttributes(child, externalFile);
                    reference.FileType = ReferenceFileType.externalfile;
                    reference.ExternalFile = externalFile;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return reference;
    }

    internal static IList<ReferenceFile> HandleReferenceFiles(XElement element)
    {
        var refFiles = new List<ReferenceFile>();
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.RefFile:
                    var refFile = new ReferenceFile();
                    HandleReferenceFileAttributes(child, refFile);
                    refFiles.Add(refFile);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
        return refFiles;
    }

    internal TagDefinitions HandleTagDefinitions(XElement element)
    {
        var tagDefinitions = new TagDefinitions();

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Tag:
                    HandleTagElements(tagDefinitions, child);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return tagDefinitions;
    }

    private static void CreateTag(TagDefinitions tagDefinitions, string id, TagInfoCreator tagCreator)
    {
        switch (tagCreator.Type)
        {
            case TagInfoCreator.TagType.TagPair:
                tagDefinitions.TagPairInfo.Add(id, tagCreator.CreateTagPairInfo());
                break;

            case TagInfoCreator.TagType.Placeholder:
                tagDefinitions.PlaceholderInfo.Add(id, tagCreator.CreatePlaceholderInfo());
                break;

            case TagInfoCreator.TagType.StructureTag:
                tagDefinitions.StructureTagInfo.Add(id, tagCreator.CreateStructureTagInfo());
                break;

            default:
                throw new InvalidOperationException($"Unknown type: {tagCreator.Type}");
        }
    }

    private static void HandleBeginPairedTag(BeginPairedTag beginPairedTag, XElement element)
    {
        beginPairedTag.Contents = element.Value;
        HandleCommonTagAttributes(beginPairedTag, element);
        HandleSubSegments(beginPairedTag, element);
    }

    private static void HandleCommonTagAttributes(CommonTagAttributes commonTag, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Name:
                    commonTag.Name = attribute.Value;
                    break;

                case SdlxliffNames.CanHide:
                    commonTag.CanHide = (bool)attribute;
                    break;

                case SdlxliffNames.LineWrap:
                    commonTag.LineWrap = (bool)attribute;
                    break;

                case SdlxliffNames.WordEnd:
                    commonTag.WordEnd = (bool)attribute;
                    break;

                case SdlxliffNames.SegmentationHint:
                    ((ISegmentationHint)commonTag).SegmentationHint = Enum.Parse<SegmentationHint>(attribute.Value);
                    break;

                case SdlxliffNames.IsWhitespace:
                    ((PlaceholderInfo)commonTag).IsWhiteSpace = (bool)attribute;
                    break;

                case SdlxliffNames.EquivalentText:
                    ((PlaceholderInfo)commonTag).EquivalentText = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleContextDefinition(ContextDefinition contextDefinition, XElement element)
    {
        HandleContextDefinitionAttributes(contextDefinition, element);

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Fmt:
                    contextDefinition.FormatId = SdlxliffReaderHelper.GetIdAttribute(child);
                    break;

                case SdlxliffNames.Properties:
                    var properties = new Properties();
                    SdlxliffReaderHelper.HandleValueElement(properties, child);
                    contextDefinition.Properties = properties;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private static void HandleContextDefinitionAttributes(ContextDefinition contextDefinition, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Id:
                    break;

                case SdlxliffNames.Type:
                    contextDefinition.Type = attribute.Value;
                    break;

                case SdlxliffNames.Code:
                    contextDefinition.Code = attribute.Value;
                    break;

                case SdlxliffNames.Name:
                    contextDefinition.Name = attribute.Value;
                    break;

                case SdlxliffNames.Description:
                    contextDefinition.Description = attribute.Value;
                    break;

                case SdlxliffNames.Color:
                    contextDefinition.Color = attribute.Value;
                    break;

                case SdlxliffNames.Purpose:
                    contextDefinition.Purpose = Enum.Parse<ContextPurpose>(attribute.Value);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleDetectedEncodingAttributes(DetectedEncoding detectedEncoding, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.DetectionLevel:
                    detectedEncoding.DetectionLevel = attribute.Value;
                    break;

                case SdlxliffNames.Encoding:
                    detectedEncoding.Encoding = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleDetectedLanguageAttributes(DetectedLanguage detectedLanguage, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.DetectionLevel:
                    detectedLanguage.DetectionLevel = attribute.Value;
                    break;

                case SdlxliffNames.Language:
                    detectedLanguage.Language = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleEndPairedTag(EndPairedTag endPairedTag, XElement element)
    {
        endPairedTag.Contents = element.Value;
        HandleCommonTagAttributes(endPairedTag, element);
    }

    private static void HandleFormattingDefinition(IDictionary<string, FormattingDefinition> formattingDefinitions, XElement element)
    {
        var formattingDefinition = new FormattingDefinition();
        var id = SdlxliffReaderHelper.GetIdAttribute(element);
        SdlxliffReaderHelper.HandleValueElement(formattingDefinition, element);
        formattingDefinitions.Add(id, formattingDefinition);
    }

    private static void HandleInternalFileAttributes(XElement element, InternalFile internalFile)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Form:
                    internalFile.Form = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleNodeDefinition(NodeDefinition nodeDefinition, XElement element)
    {
        HandleNodeDefinitionAttributes(nodeDefinition, element);

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Cxt:
                    nodeDefinition.ContextId = SdlxliffReaderHelper.GetIdAttribute(child);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private static void HandleNodeDefinitionAttributes(NodeDefinition nodeDefinition, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Id:
                    break;

                case SdlxliffNames.ForceName:
                    nodeDefinition.ForceName = (bool)attribute;
                    break;

                case SdlxliffNames.Parent:
                    nodeDefinition.ParentId = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandlePlaceholder(PlaceholderInfo placeholder, XElement element)
    {
        placeholder.Contents = element.Value;
        HandleCommonTagAttributes(placeholder, element);
        HandleSubSegments(placeholder, element);
    }

    private static void HandleReferenceFileAttributes(XElement element, ReferenceFile refFile)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Uid:
                    refFile.Uid = attribute.Value;
                    break;

                case SdlxliffNames.Id:
                    refFile.Id = attribute.Value;
                    break;

                case SdlxliffNames.Name:
                    refFile.Name = attribute.Value;
                    break;

                case SdlxliffNames.OPath:
                    refFile.OriginalPath = attribute.Value;
                    break;

                case SdlxliffNames.Date:
                    refFile.Date = attribute.Value;
                    break;

                case SdlxliffNames.Description:
                    refFile.Description = attribute.Value;
                    break;

                case SdlxliffNames.ExpectedUse:
                    refFile.ExpectedUse = attribute.Value;
                    break;

                case SdlxliffNames.RelPath:
                    refFile.RelativePath = attribute.Value;
                    break;

                case SdlxliffNames.PrefRefType:
                    refFile.PreferredReferenceType = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleSniffInfo(FileInfo fileInfo, XElement element)
    {
        var sniffInfo = new SniffInfo();
        HandleSniffInfoAttributes(sniffInfo, element);

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.DetectedEncoding:
                    var detectedEncoding = new DetectedEncoding();
                    HandleDetectedEncodingAttributes(detectedEncoding, child);
                    sniffInfo.DetectedEncoding = detectedEncoding;
                    break;

                case SdlxliffNames.DetectedSourceLanguage:
                    var detectedSourceLanguage = new DetectedLanguage();
                    HandleDetectedLanguageAttributes(detectedSourceLanguage, child);
                    sniffInfo.DetectedSourceLanguage = detectedSourceLanguage;
                    break;

                case SdlxliffNames.DetectedTargetLanguage:
                    var detectedTargetLanguage = new DetectedLanguage();
                    HandleDetectedLanguageAttributes(detectedTargetLanguage, child);
                    sniffInfo.DetectedTargetLanguage = detectedTargetLanguage;
                    break;

                case SdlxliffNames.SuggestedTargetEncoding:
                    var suggestedTargetEncoding = new SuggestedTargetEncoding();
                    HandleSuggestedTargetEncodingAttributes(suggestedTargetEncoding, child);
                    sniffInfo.SuggestedTargetEncoding = suggestedTargetEncoding;
                    break;

                case SdlxliffNames.Properties:
                    var properties = new Properties();
                    SdlxliffReaderHelper.HandleValueElement(properties, child);
                    sniffInfo.Properties = properties;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        fileInfo.SniffInfo = sniffInfo;
    }

    private static void HandleSniffInfoAttributes(SniffInfo sniffInfo, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.IsSupported:
                    sniffInfo.IsSupported = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleStructureTag(StructureTagInfo structureTag, XElement element)
    {
        structureTag.Contents = element.Value;
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Name:
                    structureTag.Name = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
        HandleSubSegments(structureTag, element);
    }

    private static void HandleSubSegments(ISubSegment subSegment, XElement element)
    {
        int offset = 0;
        foreach (var child in element.Nodes())
        {
            switch (child.NodeType)
            {
                case XmlNodeType.Element:
                    var elem = (XElement)child;
                    if (elem.Name.LocalName == SdlxliffNames.Sub)
                    {
                        var id = elem.Attribute("xid")?.Value ?? string.Empty;
                        var length = elem.Value.Length;
                        subSegment.SubSegments.Add(new SubSegment(id, length, offset));
                        offset += length;
                    }
                    else
                    {
                        XmlExceptionHelper.ThrowUnknownElementException((XElement)child);
                    }
                    break;

                case XmlNodeType.Text:
                    offset += ((XText)child).Value.Length;
                    break;

                default:
                    throw new XmlException($"Unexpected node type: {child.NodeType}");
            }
        }
    }

    private static void HandleSuggestedTargetEncodingAttributes(SuggestedTargetEncoding suggestedTargetEncoding, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Category:
                    suggestedTargetEncoding.Category = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleExternalFileAttributes(XElement element, ExternalFile externalFile)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.HRef:
                    externalFile.HRef = attribute.Value;
                    break;

                case SdlxliffNames.Uid:
                    externalFile.UId = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private void HandleTagElements(TagDefinitions tagDefinitions, XElement element)
    {
        var id = SdlxliffReaderHelper.GetIdAttribute(element);
        var tagInfoCreator = new TagInfoCreator(formattingDefinitions);

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.BeginPairedTag:
                    var beginPairedTag = new BeginPairedTag();
                    HandleBeginPairedTag(beginPairedTag, child);
                    tagInfoCreator.AddBeginPairedTag(beginPairedTag);
                    break;

                case SdlxliffNames.BeginPairedTagProperties:
                    var beginPairedTagProperties = new BeginPairedTagProperties();
                    SdlxliffReaderHelper.HandleValueElement(beginPairedTagProperties, child);
                    tagInfoCreator.AddBeginPairedTagProperties(beginPairedTagProperties);
                    break;

                case SdlxliffNames.EndPairedTag:
                    var endPairedTag = new EndPairedTag();
                    HandleEndPairedTag(endPairedTag, child);
                    tagInfoCreator.AddEndPairedTag(endPairedTag);
                    break;

                case SdlxliffNames.EndPairedTagProperties:
                    var endPairedTagProperties = new EndPairedTagProperties();
                    SdlxliffReaderHelper.HandleValueElement(endPairedTagProperties, child);
                    tagInfoCreator.AddEndPairedTagProperties(endPairedTagProperties);
                    break;

                case SdlxliffNames.StructureTag:
                    var structureTag = new StructureTagInfo();
                    HandleStructureTag(structureTag, child);
                    tagInfoCreator.AddStructureTagInfo(structureTag);
                    break;

                case SdlxliffNames.Placeholder:
                    var placeHolder = new PlaceholderInfo()
                    {
                        // True by default for placeholder tags
                        WordEnd = true
                    };
                    HandlePlaceholder(placeHolder, child);
                    tagInfoCreator.AddPlaceholderInfo(placeHolder);
                    break;

                case SdlxliffNames.Properties:
                    var properties = new Properties();
                    SdlxliffReaderHelper.HandleValueElement(properties, child);
                    tagInfoCreator.AddProperties(properties);
                    break;

                case SdlxliffNames.Fmt:
                    tagInfoCreator.AddFormatId(SdlxliffReaderHelper.GetIdAttribute(child));
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        CreateTag(tagDefinitions, id, tagInfoCreator);
    }
}
