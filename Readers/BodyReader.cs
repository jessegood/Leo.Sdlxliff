using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using static Leo.Sdlxliff.TranslationUnit;

namespace Leo.Sdlxliff.Readers;

internal class BodyReader
{
    private readonly IList<CommentMarker> commentMarkers = new List<CommentMarker>();
    private readonly DocumentInformation documentInformation;
    private readonly Header header;
    private readonly IDictionary<string, ITranslationUnit> lockedTranslationUnits = new Dictionary<string, ITranslationUnit>();
    private readonly IList<RevisionMarker> revisionMarkers = new List<RevisionMarker>();
    private readonly IDictionary<string, SegmentDefinition> segmentDefinitions = new Dictionary<string, SegmentDefinition>();
    private TranslationUnitBuilder translationUnitBuilder;

    internal BodyReader(DocumentInformation documentInformation, Header header)
    {
        this.documentInformation = documentInformation;
        this.header = header;

        translationUnitBuilder = new TranslationUnitBuilder(documentInformation.RepetitionDefinitions);
    }

    internal IList<CommentMarker> CommentMarkers => commentMarkers;

    internal IList<RevisionMarker> RevisionMarkers => revisionMarkers;

    internal IList<ITranslationUnit> HandleGroup(XElement element)
    {
        var translationUnits = new List<ITranslationUnit>();
        var contextInformation = new ContextInformation();

        HandleGroupRecurse(element, translationUnits, contextInformation);

        return translationUnits;
    }

    /// <summary>
    /// Used when <c>trans-unit</c> is not a child of <c>group</c>.
    /// </summary>
    /// <param name="element">A <c>trans-unit</c> element</param>
    /// <returns>TranslationUnit</returns>
    internal ITranslationUnit HandleTranslationUnit(XElement element)
    {
        translationUnitBuilder = new TranslationUnitBuilder(documentInformation.RepetitionDefinitions);

        HandleTranslationUnitAttributes(element);
        HandleTranslationUnitChildren(element);

        var translationUnit = translationUnitBuilder.Build();
        if (translationUnit.IsLocked)
        {
            lockedTranslationUnits.Add(translationUnit.TranslationUnitId, translationUnit);
        }

        return translationUnit;
    }

    private static ITranslationUnitContent CreateMarkerType(string type)
    {
        return type switch
        {
            SdlxliffNames.Seg => new SegmentMarker(),
            SdlxliffNames.XSdlLocation => new LocationMarker(),
            SdlxliffNames.XSdlComment => new CommentMarker(),
            SdlxliffNames.XSdlAdded => new RevisionMarker(SdlxliffNames.XSdlAdded),
            SdlxliffNames.XSdlDeleted => new RevisionMarker(SdlxliffNames.XSdlDeleted),
            SdlxliffNames.XSdlFeedbackAdded => new RevisionMarker(SdlxliffNames.XSdlFeedbackAdded),
            SdlxliffNames.XSdlFeedbackComment => new RevisionMarker(SdlxliffNames.XSdlFeedbackComment),
            SdlxliffNames.XSdlFeedbackDeleted => new RevisionMarker(SdlxliffNames.XSdlFeedbackDeleted),
            _ => new CustomMarker(type),
        };
    }

    private static ITranslationUnitContent GetMarkerElement(XElement element)
    {
        ITranslationUnitContent marker = new SegmentMarker();

        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.MarkerType:
                    marker = CreateMarkerType(attribute.Value);
                    break;

                case SdlxliffNames.MarkerId:
                case SdlxliffNames.CommentId:
                case SdlxliffNames.RevisionId:
                    marker.Id = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }

        return marker;
    }

    private static (string, string) GetXElementAttributes(XElement element)
    {
        var ids = (string.Empty, string.Empty);

        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Id:
                    ids.Item1 = attribute.Value;
                    break;

                case SdlxliffNames.XId:
                    ids.Item2 = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }

        return ids;
    }

    private static void HandleContextsElement(ContextsElement ctxts, XElement element)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Cxt:
                    var context = new Context()
                    {
                        Id = SdlxliffReaderHelper.GetIdAttribute(child)
                    };
                    ctxts.Contexts.Add(context);
                    break;

                case SdlxliffNames.Node:
                    ctxts.NodeId = SdlxliffReaderHelper.GetIdAttribute(child);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private static void HandleOriginInformationAttributes(IOriginInformation originInformation, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Id:
                    ((SegmentDefinition)originInformation).Id = attribute.Value;
                    break;

                case SdlxliffNames.Confirmation:
                    ((SegmentDefinition)originInformation).ConfirmationLevel = Enum.Parse<ConfirmationLevel>(attribute.Value);
                    break;

                case SdlxliffNames.Locked:
                    ((SegmentDefinition)originInformation).Locked = (bool)attribute;
                    break;

                case SdlxliffNames.Origin:
                    originInformation.Origin = attribute.Value;
                    break;

                case SdlxliffNames.OriginSystem:
                    originInformation.OriginSystem = attribute.Value;
                    break;

                case SdlxliffNames.Percent:
                    originInformation.Percent = (byte)(uint)attribute;
                    break;

                case SdlxliffNames.StructMatch:
                    originInformation.StructMatch = (bool)attribute;
                    break;

                case SdlxliffNames.TextMatch:
                    originInformation.TextMatch = Enum.Parse<TextContextMatchLevel>(attribute.Value);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleTagPairElementAttributes(TagPair tagPair, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Id:
                    tagPair.Id = attribute.Value;
                    break;

                case SdlxliffNames.Start:
                    tagPair.Start = (bool)attribute;
                    break;

                case SdlxliffNames.End:
                    tagPair.End = (bool)attribute;
                    break;

                case SdlxliffNames.StartRevisionId:
                    tagPair.StartRevisionId = attribute.Value;
                    break;

                case SdlxliffNames.EndRevisionId:
                    tagPair.EndRevisionId = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the context information for the paragraph unit
    /// </summary>
    /// <param name="contexts">The <c>sdl:cxt</c> elements of the group</param>
    private void AddContexts(ContextsElement contexts, ContextInformation contextInformation)
    {
        foreach (var cxt in contexts.Contexts)
        {
            SetContextDefinitions(cxt, contextInformation);
        }

        SetNodeDefinition(contexts, contextInformation);
    }

    private void AddSegment(SegmentMarker marker)
    {
        var segmentType = GetSegmentType(marker);

        var segment = new Segment()
        {
            Contents = marker.Contents,
            Id = marker.Id,
            Parent = marker.Parent,
            SegmentType = segmentType,
        };

        if (segmentType == SegmentType.Source || segmentType == SegmentType.SegmentationSource)
        {
            translationUnitBuilder.SetSourceSegment(segment.Id, segment, marker.SegmentDefinition);
        }
        else
        {
            translationUnitBuilder.SetTargetSegment(segment.Id, segment, marker.SegmentDefinition);
        }

        static SegmentType GetSegmentType(SegmentMarker marker)
        {
            var container = marker.Parent;

            if (container != null)
            {
                while (container.Parent != null)
                {
                    container = container.Parent;
                }
            }

            return container switch
            {
                Source _ => SegmentType.Source,
                SegmentationSource _ => SegmentType.SegmentationSource,
                Target _ => SegmentType.Target,
                _ => throw new ArgumentException("Unknown container")
            };
        }
    }

    private ITranslationUnitContent CreateTagAndSetDefinitions((string, string) ids)
    {
        if (translationUnitBuilder.IsStructure)
        {
            // The id refers to an <st>
            if (header.TagDefinitions.StructureTagInfo.TryGetValue(ids.Item1, out var structureTag))
            {
                return PlaceholderTagCreator.CreateStructureTag(ids, structureTag);
            }
        }
        else if (ids.Item1.StartsWith("locked"))
        {
            // The id refers to a locked <trans-unit>
            if (lockedTranslationUnits.TryGetValue(ids.Item2, out var lockedTranslationUnit))
            {
                lockedTranslationUnits.Remove(ids.Item2);
                return PlaceholderTagCreator.CreateLockedContent(ids, lockedTranslationUnit);
            }
        }
        else
        {
            // The id refers to a <ph> tag
            if (header.TagDefinitions.PlaceholderInfo.TryGetValue(ids.Item1, out var placeholder))
            {
                return PlaceholderTagCreator.CreatePlaceholder(ids.Item1, placeholder);
            }
        }

        throw new ArgumentException($"Ids do not exist in tag definitions. id: {ids.Item1} xid: {ids.Item2}");
    }

    private void HandleContainerContents(TranslationUnitContentContainer contentContainer, XElement element)
    {
        foreach (var node in element.Nodes())
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    HandleInlineElements(contentContainer, (XElement)node);
                    break;

                case XmlNodeType.Text:
                    var text = new Text(((XText)node).Value)
                    {
                        Parent = contentContainer
                    };
                    contentContainer.Contents.Add(text);
                    break;

                default:
                    break;
            }
        }
    }

    private void HandleGroupRecurse(XElement element,
                                    IList<ITranslationUnit> translationUnits,
                                    ContextInformation contextInformation)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Cxts:
                    var contexts = new ContextsElement();
                    HandleContextsElement(contexts, child);
                    AddContexts(contexts, contextInformation);
                    break;

                case SdlxliffNames.TransUnit:
                    translationUnitBuilder = new TranslationUnitBuilder(documentInformation.RepetitionDefinitions, contextInformation);

                    HandleTranslationUnitAttributes(child);
                    HandleTranslationUnitChildren(child);

                    var translationUnit = translationUnitBuilder.Build();
                    if (translationUnit.IsLocked)
                    {
                        lockedTranslationUnits.Add(translationUnit.TranslationUnitId, translationUnit);
                    }

                    translationUnits.Add(translationUnit);

                    break;

                case SdlxliffNames.Group:
                    HandleGroupRecurse(child, translationUnits, contextInformation);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private void HandleInlineElements(TranslationUnitContentContainer contentContainer, XElement element)
    {
        switch (element.Name.LocalName)
        {
            case SdlxliffNames.X:
                var ids = GetXElementAttributes(element);
                var tag = CreateTagAndSetDefinitions(ids);
                tag.Parent = contentContainer;
                contentContainer.Contents.Add(tag);
                break;

            case SdlxliffNames.G:
                var tagPair = new TagPair()
                {
                    Parent = contentContainer
                };
                HandleTagPairElementAttributes(tagPair, element);
                HandleContainerContents(tagPair, element);
                PropertySetter.SetTagDefinitionsForTagPair(tagPair, header.TagDefinitions, documentInformation.RevisionDefinitions);
                contentContainer.Contents.Add(tagPair);
                break;

            case SdlxliffNames.Mrk:
                ITranslationUnitContent marker = GetMarkerElement(element);
                if (marker.ContentType != TranslationUnitContentType.LocationMarker)
                {
                    HandleContainerContents((TranslationUnitContentContainer)marker, element);
                    SetMarkerInfo(marker);
                }
                marker.Parent = contentContainer;

                if (marker.ContentType == TranslationUnitContentType.Segment)
                {
                    AddSegment((SegmentMarker)marker);
                }

                contentContainer.Contents.Add(marker);
                break;

            default:
                XmlExceptionHelper.ThrowUnknownElementException(element);
                break;
        }
    }

    private void HandleOriginInformationChildren(IOriginInformation originInformation, XElement element)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Rep:
                    originInformation.RepetitionId = SdlxliffReaderHelper.GetIdAttribute(child);
                    break;

                case SdlxliffNames.PreviousOrigin:
                    var subPrevOrigin = new PreviousOrigin();
                    HandleOriginInformationAttributes(subPrevOrigin, child);
                    HandleOriginInformationChildren(subPrevOrigin, child);
                    originInformation.Previous = subPrevOrigin;
                    break;

                case SdlxliffNames.Value:
                    SdlxliffReaderHelper.HandleValueElement((IValueElement)originInformation, child, true);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private void HandleSegmentDefinitions(XElement element)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Seg:
                    var segDef = new SegmentDefinition();
                    HandleOriginInformationAttributes(segDef, child);
                    HandleOriginInformationChildren(segDef, child);
                    segmentDefinitions.Add(segDef.Id, segDef);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private void HandleTranslationUnitAttributes(XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Id:
                    translationUnitBuilder.SetTranslationUnitId(attribute.Value);
                    break;

                case SdlxliffNames.Translate:
                    translationUnitBuilder.SetTranslateAttribute(Enum.Parse<Translate>(attribute.Value, true));
                    break;

                case SdlxliffNames.Locktype:
                    translationUnitBuilder.SetLockType(Enum.Parse<LockType>(attribute.Value));
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }

        // After the attributes are read, need to call this to
        // check if the translation unit is a structure
        translationUnitBuilder.CheckIfStructure();
    }

    private void HandleTranslationUnitChildren(XElement element)
    {
        // Need to process sdl:seg-defs first
        var segDefs = element.Element(SdlxliffNames.Sdlxliff + SdlxliffNames.SegDefs);
        if (segDefs != null)
        {
            HandleSegmentDefinitions(segDefs);
        }

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.SegmentationSource:
                    var segSource = new SegmentationSource();
                    HandleContainerContents(segSource, child);
                    translationUnitBuilder.SetSegmentationSource(segSource);
                    break;

                case SdlxliffNames.Source:
                    var source = new Source();
                    HandleContainerContents(source, child);
                    translationUnitBuilder.SetSource(source);
                    break;

                case SdlxliffNames.SegDefs:
                    break;

                case SdlxliffNames.Target:
                    var target = new Target();
                    HandleContainerContents(target, child);
                    translationUnitBuilder.SetTarget(target);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private void SetContextDefinitions(Context cxt, ContextInformation contextInformation)
    {
        if (header.ContextDefinitions.TryGetValue(cxt.Id,
                                                  out var contextDefinition))
        {
            if (header.FormattingDefinitions.TryGetValue(contextDefinition.FormatId,
                                                         out var formattingDefinition))
            {
                contextDefinition.FormattingDefinition = formattingDefinition;
            }

            contextInformation.Contexts.Add(contextDefinition);
        }
    }

    private void SetMarkerComments(ITranslationUnitContent content)
    {
        if (documentInformation.CommentDefinitions.TryGetValue(content.Id, out var commentDefinition))
        {
            ((CommentMarker)content).CommentDefinition = commentDefinition;
            commentMarkers.Add((CommentMarker)content);
        }
    }

    private void SetMarkerInfo(ITranslationUnitContent content)
    {
        switch (content.ContentType)
        {
            case TranslationUnitContentType.CommentsMarker:
                SetMarkerComments(content);
                break;

            case TranslationUnitContentType.LocationMarker:
            case TranslationUnitContentType.CustomMarker:
                break;

            case TranslationUnitContentType.RevisionMarker:
                SetMarkerRevisions(content);
                break;

            case TranslationUnitContentType.Segment:
                SetMarkerSegmentDefinitions(content);
                break;

            default:
                throw new ArgumentException($"Invalid content type for marker: {content.ContentType}");
        }
    }

    private void SetMarkerRevisions(ITranslationUnitContent content)
    {
        if (documentInformation.RevisionDefinitions.TryGetValue(content.Id, out var revisionDefinition))
        {
            ((RevisionMarker)content).RevisionDefinition = revisionDefinition;
            revisionMarkers.Add((RevisionMarker)content);
        }
    }

    private void SetMarkerSegmentDefinitions(ITranslationUnitContent content)
    {
        if (segmentDefinitions.TryGetValue(content.Id, out var segmentDefinition))
        {
            ((SegmentMarker)content).SegmentDefinition = segmentDefinition;
        }
    }

    private void SetNodeDefinition(ContextsElement contexts, ContextInformation contextInformation)
    {
        if (header.NodeDefinitions.TryGetValue(contexts.NodeId,
                                               out var nodeDefinition))
        {
            if (header.NodeDefinitions.TryGetValue(nodeDefinition.ParentId,
                                                   out var nodeDefinitionParent))
            {
                nodeDefinition.Parent = nodeDefinitionParent;
            }
            else
            {
                nodeDefinition.Parent = new NodeDefinition(string.Empty);
            }

            if (header.ContextDefinitions.TryGetValue(nodeDefinition.ContextId,
                                                      out var contextDefinition))
            {
                nodeDefinition.ContextDefinition = contextDefinition;
            }

            contextInformation.NodeDefinition = nodeDefinition;
        }
    }
}