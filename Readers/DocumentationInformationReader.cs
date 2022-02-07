using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Leo.Sdlxliff.Readers;

internal static class DocumentationInformationReader
{
    internal static IDictionary<string, CommentDefinition> HandleCmtDefs(XElement element)
    {
        var cmtDefs = new Dictionary<string, CommentDefinition>();

        foreach (var child in element.Elements())
        {
            var cmtDef = new CommentDefinition();
            var id = SdlxliffReaderHelper.GetIdAttribute(child);

            foreach (var descendant in child.Descendants(SdlxliffNames.Sdlxliff + SdlxliffNames.Comment))
            {
                var comment = new Comment(string.Join("", descendant.Nodes().OfType<XText>().Select(x => x.Value)));
                HandleCommentAttributes(descendant, comment);
                cmtDef.Comments.Add(comment);
            }
            cmtDefs.Add(id, cmtDef);
        }

        return cmtDefs;
    }

    internal static IDictionary<string, CommentMetaDefinition> HandleCmtMetaDefs(XElement element)
    {
        var cmtMetaDefs = new Dictionary<string, CommentMetaDefinition>();

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.CmtMetaDef:
                    var cmtMetaDef = new CommentMetaDefinition();
                    var id = SdlxliffReaderHelper.GetIdAttribute(child);
                    HandleCommentMetaDefinition(cmtMetaDef, child);
                    cmtMetaDefs.Add(id, cmtMetaDef);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return cmtMetaDefs;
    }

    internal static IDictionary<string, IList<Entry>> HandleRepDefs(XElement element)
    {
        var repDefs = new Dictionary<string, IList<Entry>>();

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.RepDef:
                    var repDef = new List<Entry>();
                    var id = SdlxliffReaderHelper.GetIdAttribute(child);
                    HandleEntries(child, repDef);
                    repDefs.Add(id, repDef);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }

        return repDefs;
    }

    internal static IDictionary<string, RevisionDefinition> HandleRevDefs(XElement element)
    {
        var revDefs = new Dictionary<string, RevisionDefinition>();

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.RevDef:
                    var revDef = new RevisionDefinition();
                    var id = SdlxliffReaderHelper.GetIdAttribute(child);
                    HandleRevisionDefinitionAttributes(child, revDef);
                    revDefs.Add(id, revDef);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
        return revDefs;
    }

    private static void HandleCommentAttributes(XElement element, Comment comment)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Severity:
                    comment.Severity = Enum.Parse<CommentSeverity>(attribute.Value);
                    break;

                case SdlxliffNames.User:
                    comment.User = attribute.Value;
                    break;

                case SdlxliffNames.Date:
                    comment.Date = attribute.Value;
                    break;

                case SdlxliffNames.Version:
                    comment.Version = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleCommentMetaDefinition(CommentMetaDefinition cmtMetaDef, XElement element)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.CmtMetaData:
                    var cmtMetaData = new CommentMetaData
                    {
                        Id = SdlxliffReaderHelper.GetIdAttribute(child)
                    };
                    SdlxliffReaderHelper.HandleValueElement(cmtMetaData, child);
                    cmtMetaDef.CommentMetaDatas.Add(cmtMetaData);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private static void HandleEntries(XElement element, IList<Entry> repDef)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Entry:
                    var entry = new Entry();
                    HandleEntryAttributes(child, entry);
                    repDef.Add(entry);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private static void HandleEntryAttributes(XElement element, Entry entry)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Seg:
                    entry.Seg = attribute.Value;
                    break;

                case SdlxliffNames.Tu:
                    entry.Tu = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }

    private static void HandleRevisionDefinitionAttributes(XElement element, RevisionDefinition revDef)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Id:
                    break;

                case SdlxliffNames.Type:
                    revDef.Type = Enum.Parse<RevisionType>(attribute.Value);
                    break;

                case SdlxliffNames.Author:
                    revDef.Author = attribute.Value;
                    break;

                case SdlxliffNames.Date:
                    revDef.Date = attribute.Value;
                    break;

                case SdlxliffNames.DocCategory:
                    revDef.DocCategory = attribute.Value;
                    break;

                case SdlxliffNames.FbCategory:
                    revDef.FbCategory = attribute.Value;
                    break;

                case SdlxliffNames.FbSeverity:
                    revDef.FbSeverity = attribute.Value;
                    break;

                case SdlxliffNames.FbReplacementId:
                    revDef.FbReplacementId = attribute.Value;
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }
}
