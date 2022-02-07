using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Leo.Sdlxliff.Writers;

internal static class XmlElementBuilder
{
    internal static XElement BuildCommentDefinition(string id, CommentDefinition commentDefinition)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.CmtDef,
                            new XAttribute(SdlxliffNames.Id, id),
                            new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Comments,
                                from cmt in commentDefinition.Comments
                                select new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Comment,
                                    new XText(cmt.Contents),
                                    new XAttribute(SdlxliffNames.Severity, cmt.Severity),
                                    new XAttribute(SdlxliffNames.User, cmt.User),
                                    new XAttribute(SdlxliffNames.Date, cmt.Date),
                                    new XAttribute(SdlxliffNames.Version, cmt.Version)
                                    )
                                )
                            );
    }

    internal static XElement BuildCommentMetaDefinition(string id, CommentMetaDefinition commentMetaDefinition)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.CmtMetaDef,
                            new XAttribute(SdlxliffNames.Id, id),
                            from commentMetaData in commentMetaDefinition.CommentMetaDatas
                            select new XElement(SdlxliffNames.CmtMetaData,
                                        new XAttribute(SdlxliffNames.Id, commentMetaData.Id),
                                        BuildKeyValuePairs(commentMetaData.Values)
                                        )
                             );
    }

    internal static XElement BuildContextDefinition(string id, ContextDefinition contextDefinition)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.CxtDef,
                    new XAttribute(SdlxliffNames.Id, id),
                    new XAttribute(SdlxliffNames.Type, contextDefinition.Type),
                    GetOptionalAttribute(SdlxliffNames.Code, contextDefinition.Code),
                    GetOptionalAttribute(SdlxliffNames.Name, contextDefinition.Name),
                    GetOptionalAttribute(SdlxliffNames.Description, contextDefinition.Description),
                    GetOptionalAttribute(SdlxliffNames.Color, contextDefinition.Color),
                    GetOptionalAttribute(SdlxliffNames.Purpose, contextDefinition.Purpose),
                    new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Fmt,
                        new XAttribute(SdlxliffNames.Id, contextDefinition.FormatId)
                        )
                    );
    }

    internal static XElement BuildContexts(ContextInformation contextInformation)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Cxts,
                    // sdl:cxt
                    from cxt in contextInformation.Contexts
                    select new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Cxt,
                                new XAttribute(SdlxliffNames.Id, cxt.Id)
                                ),
                    // sdl:node
                    string.IsNullOrEmpty(contextInformation.NodeDefinition.Id) ? null :
                    new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Node,
                        new XAttribute(SdlxliffNames.Id, contextInformation.NodeDefinition.Id)
                        )
                            );
    }

    internal static XElement BuildFormattingDefinition(string id, FormattingDefinition formattingDefinition)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.FmtDef,
                        new XAttribute(SdlxliffNames.Id, id),
                        BuildKeyValuePairs(formattingDefinition.Values)
                        );
    }

    internal static IEnumerable<XElement> BuildKeyValuePairs(IDictionary<string, string> values)
    {
        return from value in values
               select new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Value,
                           new XText(value.Value),
                           new XAttribute(SdlxliffNames.Key, value.Key)
                           );
    }

    internal static XElement BuildNodeDefinition(string id, NodeDefinition nodeDefinition)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.NodeDef,
                    new XAttribute(SdlxliffNames.Id, id),
                    GetOptionalAttribute(SdlxliffNames.ForceName, nodeDefinition.ForceName, true),
                    GetOptionalAttribute(SdlxliffNames.Parent, nodeDefinition.ParentId),
                        string.IsNullOrEmpty(nodeDefinition.ContextId) ? null :
                        new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Cxt,
                            new XAttribute(SdlxliffNames.Id, nodeDefinition.ContextId)
                            )
                    );
    }

    internal static XElement BuildReferenceFile(ReferenceFile referenceFile)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.RefFile,
                    new XAttribute(SdlxliffNames.Uid, referenceFile.Uid),
                    new XAttribute(SdlxliffNames.Id, referenceFile.Id),
                    new XAttribute(SdlxliffNames.Name, referenceFile.Name),
                    new XAttribute(SdlxliffNames.OPath, referenceFile.OriginalPath),
                    new XAttribute(SdlxliffNames.Date, referenceFile.Date),
                    GetOptionalAttribute(SdlxliffNames.Description, referenceFile.Description),
                    GetOptionalAttribute(SdlxliffNames.RelPath, referenceFile.RelativePath),
                    new XAttribute(SdlxliffNames.ExpectedUse, referenceFile.ExpectedUse),
                    GetOptionalAttribute(SdlxliffNames.PrefRefType, referenceFile.PreferredReferenceType)
                    );
    }

    internal static XElement BuildRepetitionDefinition(string id, IList<Entry> entries)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.RepDef,
                    new XAttribute(SdlxliffNames.Id, id),
                    from entry in entries
                    select new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Entry,
                        new XAttribute(SdlxliffNames.Tu, entry.Tu),
                        new XAttribute(SdlxliffNames.Seg, entry.Seg)
                        )
                    );
    }

    internal static XElement BuildRevisionDefinition(string id, RevisionDefinition revisionDefinition)
    {
        var revDef = new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.RevDef,
                        new XAttribute(SdlxliffNames.Id, id),
                        GetOptionalAttribute(SdlxliffNames.Type, revisionDefinition.Type),
                        GetOptionalAttribute(SdlxliffNames.Author, revisionDefinition.Author),
                        GetOptionalAttribute(SdlxliffNames.Date, revisionDefinition.Date),
                        GetOptionalAttribute(SdlxliffNames.DocCategory, revisionDefinition.DocCategory),
                        GetOptionalAttribute(SdlxliffNames.FbCategory, revisionDefinition.FbCategory),
                        GetOptionalAttribute(SdlxliffNames.FbSeverity, revisionDefinition.FbSeverity),
                        GetOptionalAttribute(SdlxliffNames.FbReplacementId, revisionDefinition.FbReplacementId)
                        );

        return revDef;
    }

    internal static XElement BuildSniffInfo(SniffInfo sniffInfo)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.SniffInfo,
                    GetOptionalAttribute(SdlxliffNames.IsSupported, sniffInfo.IsSupported),
                    GetSniffInfoChildren(sniffInfo)
                    );
    }

    internal static XElement BuildTagDefinition(string id, StructureTagInfo structureTagInfo)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Tag,
                        new XAttribute(SdlxliffNames.Id, id),
                            GetTagChildren(structureTagInfo)
                        );
    }

    internal static XElement BuildTagDefinition(string id, PlaceholderInfo placeholderInfo)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Tag,
                        new XAttribute(SdlxliffNames.Id, id),
                            GetTagChildren(placeholderInfo)
                        );
    }

    internal static XElement BuildTagDefinition(string id, TagPairInfo tagPairInfo)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Tag,
                    new XAttribute(SdlxliffNames.Id, id),
                        GetTagChildren(tagPairInfo)
                    );
    }

    internal static XElement BuildTranslationUnit(ITranslationUnit translationUnit)
    {
        if (translationUnit.IsLocked)
        {
            return new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.TransUnit,
                        new XAttribute(SdlxliffNames.Id, translationUnit.TranslationUnitId),
                        GetOptionalAttribute(SdlxliffNames.Translate, translationUnit.Translate),
                        GetOptionalAttribute(SdlxliffNames.Locktype, translationUnit.LockType),
                        GetTranslationUnitChildren(translationUnit)
                        );
        }
        else
        {
            return new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.TransUnit,
                        GetOptionalAttribute(SdlxliffNames.Locktype, translationUnit.LockType),
                        GetOptionalAttribute(SdlxliffNames.Translate, translationUnit.Translate),
                        new XAttribute(SdlxliffNames.Id, translationUnit.TranslationUnitId),
                        GetTranslationUnitChildren(translationUnit)
                        );
        }
    }

    private static IEnumerable<XNode> BuildTagContent(string contents, IList<SubSegment> subSegments)
    {
        if (subSegments.Count > 0)
        {
            var nodes = new List<XNode>();
            int offset = 0;
            foreach (var subSegment in subSegments)
            {
#pragma warning disable IDE0057 // Use range operator
                nodes.Add(new XText(contents.Substring(startIndex: offset, length: subSegment.Offset - offset)));
#pragma warning restore IDE0057 // Use range operator
                nodes.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Sub,
                                   new XAttribute(SdlxliffNames.XId, subSegment.Id),
                                   new XText(contents.Substring(startIndex: subSegment.Offset, length: subSegment.Length))
                                   )
                         );
                offset = subSegment.Offset + subSegment.Length;
            }

            nodes.Add(new XText(contents[offset..]));

            return nodes;
        }

        return new[] { new XText(contents) };
    }

    private static IEnumerable<XNode> GetContainerChildren(IEnumerable<ITranslationUnitContent> children)
    {
        var nodes = new List<XNode>();

        foreach (var content in children)
        {
            switch (content)
            {
                case CommentMarker commentMarker:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Mrk,
                                    new XAttribute(SdlxliffNames.MarkerType, commentMarker.MarkerType),
                                    new XAttribute(SdlxliffNames.Sdlxliff + SdlxliffNames.CommentId, commentMarker.Id),
                                    GetContainerChildren(commentMarker.Contents)
                                    )
                              );
                    break;

                case LocationMarker locationMarker:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Mrk,
                                    new XAttribute(SdlxliffNames.MarkerType, locationMarker.MarkerType),
                                    new XAttribute(SdlxliffNames.MarkerId, locationMarker.Id)
                                    )
                              );
                    break;

                case CustomMarker customMarker:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Mrk,
                                    new XAttribute(SdlxliffNames.MarkerType, customMarker.MarkerType),
                                    GetOptionalAttribute(SdlxliffNames.MarkerId, customMarker.Id),
                                    GetContainerChildren(customMarker.Contents)
                                    )
                              );
                    break;

                case LockedContent lockedContent:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.X,
                                    new XAttribute(SdlxliffNames.Id, lockedContent.Id),
                                    new XAttribute(SdlxliffNames.XId, lockedContent.XId)
                                    )
                              );
                    break;

                case Placeholder placeholder:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.X,
                                    new XAttribute(SdlxliffNames.Id, placeholder.Id)
                                    )
                              );
                    break;

                case RevisionMarker revisionMarker:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Mrk,
                                    new XAttribute(SdlxliffNames.MarkerType, revisionMarker.MarkerType),
                                    new XAttribute(SdlxliffNames.Sdlxliff + SdlxliffNames.RevisionId, revisionMarker.Id),
                                    GetContainerChildren(revisionMarker.Contents)
                                    )
                              );
                    break;

                case SegmentMarker segmentMarker:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Mrk,
                                    new XAttribute(SdlxliffNames.MarkerType, segmentMarker.MarkerType),
                                    new XAttribute(SdlxliffNames.MarkerId, segmentMarker.Id),
                                    GetContainerChildren(segmentMarker.Contents)
                                    )
                              );
                    break;

                case TagPair tagPair:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.G,
                                new XAttribute(SdlxliffNames.Id, tagPair.Id),
                                GetContainerChildren(tagPair.Contents)
                                )
                             );
                    break;

                case StructureTag structureTag:
                    nodes.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.X,
                                new XAttribute(SdlxliffNames.Id, structureTag.Id),
                                GetOptionalAttribute(SdlxliffNames.XId, structureTag.XId)
                                )
                             );
                    break;

                case Text text:
                    nodes.Add(new XText(text.Contents));
                    break;

                default:
                    throw new System.InvalidOperationException($"Unknown content type: {content.ContentType}");
            }
        }

        return nodes;
    }

    private static XAttribute? GetOptionalAttribute(string name, LockType value)
    {
        if (value == LockType.Structure)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.Append(value.HasFlag(LockType.Structure) ? nameof(LockType.Structure) : string.Empty);
        if (sb.Length > 0)
        {
            sb.Append(" ");
        }
        sb.Append(value.HasFlag(LockType.Externalized) ? nameof(LockType.Externalized) : string.Empty);
        if (sb.Length > 0)
        {
            sb.Append(" ");
        }
        sb.Append(value.HasFlag(LockType.Manual) ? nameof(LockType.Manual) : string.Empty);

        if (sb.Length > 0)
        {
            return new XAttribute(SdlxliffNames.Sdlxliff + name, sb.ToString());
        }

        return null;
    }

    private static XAttribute? GetOptionalAttribute(string name, Translate value)
    {
        return value == Translate.Yes ? null : new XAttribute(name, "no");
    }

    private static XAttribute? GetOptionalAttribute(string name, string value)
    {
        return string.IsNullOrEmpty(value) ? null : new XAttribute(name, value);
    }

    private static XAttribute? GetOptionalAttribute(string name, RevisionType value)
    {
        return value == RevisionType.Insert ? null : new XAttribute(name, value);
    }

    private static XAttribute? GetOptionalAttribute(string name, ContextPurpose value)
    {
        return value == ContextPurpose.Information ? null : new XAttribute(name, value);
    }

    private static XAttribute? GetOptionalAttribute(string name, bool value, bool comparer)
    {
        return value == comparer ? null : new XAttribute(name, value);
    }

    private static XAttribute? GetOptionalAttribute(string name, SegmentationHint value)
    {
        return value == SegmentationHint.MayExclude ? null : new XAttribute(name, value);
    }

    private static XAttribute? GetOptionalAttribute(string name, ConfirmationLevel value)
    {
        return value == ConfirmationLevel.Unspecified ? null : new XAttribute(name, value);
    }

    private static XAttribute? GetOptionalAttribute(string name, byte value)
    {
        return value == 0 ? null : new XAttribute(name, value);
    }

    private static XAttribute? GetOptionalAttribute(string name, TextContextMatchLevel value)
    {
        return value == TextContextMatchLevel.None ? null : new XAttribute(name, value);
    }

    private static XElement GetProperties(IDictionary<string, string> values)
    {
        return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Properties,
                    BuildKeyValuePairs(values)
                    );
    }

    private static IEnumerable<XElement> GetSniffInfoChildren(SniffInfo sniffInfo)
    {
        var elements = new List<XElement>();

        if (!sniffInfo.DetectedEncoding.IsEmpty)
        {
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.DetectedEncoding,
                            new XAttribute(SdlxliffNames.DetectionLevel, sniffInfo.DetectedEncoding.DetectionLevel),
                            new XAttribute(SdlxliffNames.Encoding, sniffInfo.DetectedEncoding.Encoding)
                            )
                        );
        }

        if (!sniffInfo.DetectedSourceLanguage.IsEmpty)
        {
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.DetectedSourceLanguage,
                            new XAttribute(SdlxliffNames.DetectionLevel, sniffInfo.DetectedSourceLanguage.DetectionLevel),
                            new XAttribute(SdlxliffNames.Language, sniffInfo.DetectedSourceLanguage.Language)
                            )
                        );
        }

        if (!sniffInfo.DetectedTargetLanguage.IsEmpty)
        {
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.DetectedTargetLanguage,
                            new XAttribute(SdlxliffNames.DetectionLevel, sniffInfo.DetectedTargetLanguage.DetectionLevel),
                            new XAttribute(SdlxliffNames.Language, sniffInfo.DetectedTargetLanguage.Language)
                            )
                        );
        }

        if (!sniffInfo.SuggestedTargetEncoding.IsEmpty)
        {
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.SuggestedTargetEncoding,
                            new XAttribute(SdlxliffNames.Category, sniffInfo.SuggestedTargetEncoding.Category)
                            )
                        );
        }

        if (!sniffInfo.Properties.IsEmpty)
        {
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Properties,
                            BuildKeyValuePairs(sniffInfo.Properties.Values)
                            )
                        );
        }

        return elements;
    }

    private static IEnumerable<XElement> GetTagChildren(StructureTagInfo structureTagInfo)
    {
        var elements = new List<XElement>
            {
                new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.StructureTag,
                            BuildTagContent(structureTagInfo.Contents, structureTagInfo.SubSegments),
                            GetOptionalAttribute(SdlxliffNames.Name, structureTagInfo.Name)
                            )
            };

        if (!structureTagInfo.Properties.IsEmpty)
        {
            elements.Add(GetProperties(structureTagInfo.Properties.Values));
        }

        return elements;
    }

    private static IEnumerable<XElement> GetTagChildren(PlaceholderInfo placeholderInfo)
    {
        var elements = new List<XElement>
            {
                new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Placeholder,
                            BuildTagContent(placeholderInfo.Contents, placeholderInfo.SubSegments),
                            GetOptionalAttribute(SdlxliffNames.Name, placeholderInfo.Name),
                            GetOptionalAttribute(SdlxliffNames.CanHide, placeholderInfo.CanHide, false),
                            GetOptionalAttribute(SdlxliffNames.LineWrap, placeholderInfo.LineWrap, true),
                            GetOptionalAttribute(SdlxliffNames.WordEnd, placeholderInfo.WordEnd, true),
                            GetOptionalAttribute(SdlxliffNames.IsWhitespace, placeholderInfo.IsWhiteSpace, false),
                            GetOptionalAttribute(SdlxliffNames.EquivalentText, placeholderInfo.EquivalentText),
                            GetOptionalAttribute(SdlxliffNames.SegmentationHint, placeholderInfo.SegmentationHint)
                        )
            };

        if (!placeholderInfo.Properties.IsEmpty)
        {
            elements.Add(GetProperties(placeholderInfo.Properties.Values));
        }

        return elements;
    }

    private static IEnumerable<XElement> GetTagChildren(TagPairInfo tagPairInfo)
    {
        var elements = new List<XElement>();

        if (!string.IsNullOrEmpty(tagPairInfo.BeginPairedTag.Contents))
        {
            var beginPairedTag = tagPairInfo.BeginPairedTag;
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.BeginPairedTag,
                            BuildTagContent(beginPairedTag.Contents, beginPairedTag.SubSegments),
                            GetOptionalAttribute(SdlxliffNames.Name, beginPairedTag.Name),
                            GetOptionalAttribute(SdlxliffNames.SegmentationHint, beginPairedTag.SegmentationHint),
                            GetOptionalAttribute(SdlxliffNames.CanHide, beginPairedTag.CanHide, false),
                            GetOptionalAttribute(SdlxliffNames.LineWrap, beginPairedTag.LineWrap, true),
                            GetOptionalAttribute(SdlxliffNames.WordEnd, beginPairedTag.WordEnd, true)
                            )
                        );
        }

        if (tagPairInfo.BeginPairedTagProperties.Values.Count > 0)
        {
            var beginPairedTagProperties = tagPairInfo.BeginPairedTagProperties;
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.BeginPairedTagProperties,
                            BuildKeyValuePairs(beginPairedTagProperties.Values)
                            )
                        );
        }

        if (!string.IsNullOrEmpty(tagPairInfo.EndPairedTag.Contents))
        {
            var endPairedTag = tagPairInfo.EndPairedTag;
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.EndPairedTag,
                            new XText(endPairedTag.Contents),
                            GetOptionalAttribute(SdlxliffNames.Name, endPairedTag.Name),
                            GetOptionalAttribute(SdlxliffNames.CanHide, endPairedTag.CanHide, false),
                            GetOptionalAttribute(SdlxliffNames.LineWrap, endPairedTag.LineWrap, true),
                            GetOptionalAttribute(SdlxliffNames.WordEnd, endPairedTag.WordEnd, true)
                            )
                        );
        }

        if (tagPairInfo.EndPairedTagProperties.Values.Count > 0)
        {
            var endPairedTagProperties = tagPairInfo.EndPairedTagProperties;
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.EndPairedTagProperties,
                            BuildKeyValuePairs(endPairedTagProperties.Values)
                            )
                        );
        }

        if (!string.IsNullOrEmpty(tagPairInfo.FormatId))
        {
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Fmt,
                            new XAttribute(SdlxliffNames.Id, tagPairInfo.FormatId)
                            )
                        );
        }

        return elements;
    }

    private static IEnumerable<XElement> GetTranslationUnitChildren(ITranslationUnit translationUnit)
    {
        var elements = new List<XElement>
            {
                // source
                new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Source,
                            GetContainerChildren(translationUnit.Source.Contents)
                            )
            };

        // seg-source
        if (translationUnit.SegmentationSource.Contents.Count > 0)
        {
            elements.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.SegmentationSource,
                            GetContainerChildren(translationUnit.SegmentationSource.Contents)
                            )
                        );
        }

        // target
        if (translationUnit.Target.Contents.Count > 0)
        {
            elements.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Target,
                            GetContainerChildren(translationUnit.Target.Contents)
                            )
                        );
        }

        // sdl:seg-defs
        if (translationUnit.HasSegmentPairs)
        {
            elements.Add(new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.SegDefs,
                            from segPair in translationUnit.GetSegmentPairs()
                            select new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Seg,
                                        new XAttribute(SdlxliffNames.Id, segPair.Id),
                                        GetOptionalAttribute(SdlxliffNames.Locked, segPair.Locked, false),
                                        GetOptionalAttribute(SdlxliffNames.Confirmation, segPair.ConfirmationLevel),
                                        GetOptionalAttribute(SdlxliffNames.Origin, segPair.Origin),
                                        GetOptionalAttribute(SdlxliffNames.OriginSystem, segPair.OriginSystem),
                                        GetOptionalAttribute(SdlxliffNames.Percent, segPair.Percent),
                                        GetOptionalAttribute(SdlxliffNames.StructMatch, segPair.StructMatch, false),
                                        GetOptionalAttribute(SdlxliffNames.TextMatch, segPair.TextMatch),
                                        string.IsNullOrEmpty(segPair.RepetitionId) ? null :
                                        new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Rep,
                                            new XAttribute(SdlxliffNames.Id, segPair.RepetitionId)
                                            ),
                                        GetPreviousOrigin(segPair.Previous),
                                        BuildKeyValuePairs(segPair.Values)
                           )
                            )
                        );
        }

        return elements;

        static XElement? GetPreviousOrigin(PreviousOrigin? previousOrigin)
        {
            if (previousOrigin != null)
            {
                return new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.PreviousOrigin,
                                GetOptionalAttribute(SdlxliffNames.Origin, previousOrigin.Origin),
                                GetOptionalAttribute(SdlxliffNames.OriginSystem, previousOrigin.OriginSystem),
                                GetOptionalAttribute(SdlxliffNames.Percent, previousOrigin.Percent),
                                GetOptionalAttribute(SdlxliffNames.StructMatch, previousOrigin.StructMatch, false),
                                GetOptionalAttribute(SdlxliffNames.TextMatch, previousOrigin.TextMatch),
                                string.IsNullOrEmpty(previousOrigin.RepetitionId) ? null :
                                new XElement(SdlxliffNames.Sdlxliff + SdlxliffNames.Rep,
                                    new XAttribute(SdlxliffNames.Id, previousOrigin.RepetitionId)
                                    ),
                                GetPreviousOrigin(previousOrigin.Previous),
                                BuildKeyValuePairs(previousOrigin.Values)
                                );
            }

            return null;
        }
    }
}
