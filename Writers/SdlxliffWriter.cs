using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Leo.Sdlxliff.Writers;

public sealed class SdlxliffWriter
{
    private readonly ISdlxliffDocument sdlxliffDocument;
    private readonly XmlWriter xmlWriter;

    public SdlxliffWriter(string outputFile, ISdlxliffDocument sdlxliffDocument)
    {
        this.sdlxliffDocument = sdlxliffDocument;

        XmlWriterSettings settings = Settings();

        xmlWriter = XmlWriter.Create(outputFile, settings);
    }

    public SdlxliffWriter(StringBuilder sb, ISdlxliffDocument sdlxliffDocument)
    {
        this.sdlxliffDocument = sdlxliffDocument;

        XmlWriterSettings settings = Settings();

        xmlWriter = XmlWriter.Create(sb, settings);
    }

    public SdlxliffWriter(Stream stream, ISdlxliffDocument sdlxliffDocument)
    {
        this.sdlxliffDocument = sdlxliffDocument;

        XmlWriterSettings settings = Settings();

        xmlWriter = XmlWriter.Create(stream, settings);
    }

    public async Task WriteAsync()
    {
        try
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.Xliff, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(SdlxliffNames.Xmlns, SdlxliffNames.Sdl, null, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Xmlns, null, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Version, null, "1.2").ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Version, SdlxliffNames.Sdlxliff.NamespaceName, "1.0").ConfigureAwait(false);

            // doc-info
            await WriteDocumentInformationAsync().ConfigureAwait(false);
            // file
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.File, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Original, null, sdlxliffDocument.FileHeader.Original).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.DataType, null, sdlxliffDocument.FileHeader.DataType).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.SourceLanguage, null, sdlxliffDocument.FileHeader.SourceLanguage).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.TargetLanguage, null, sdlxliffDocument.FileHeader.TargetLanguage).ConfigureAwait(false);
            // header
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.Header, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
            await WriteHeaderAsync().ConfigureAwait(false);
            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // header
                                                                          // body
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.Body, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
            await WriteBodyAsync().ConfigureAwait(false);
            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // body

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // file
            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // xliff
        }
        finally
        {
            if (xmlWriter != null)
            {
                await xmlWriter.FlushAsync().ConfigureAwait(false);
                xmlWriter.Close();
                xmlWriter.Dispose();
            }
        }
    }

    private static XmlWriterSettings Settings()
    {
        return new XmlWriterSettings()
        {
            CloseOutput = true,
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            NewLineHandling = NewLineHandling.None,
            Async = true
        };
    }

    private bool HasDocumentInformation()
    {
        return sdlxliffDocument.RepetitionDefinitions.Count > 0
          || sdlxliffDocument.CommentMarkers.Count > 0
          || sdlxliffDocument.RevisionMarkers.Count > 0
          || sdlxliffDocument.Header.HasFileLevelComments;
    }

    private async Task WriteBodyAsync()
    {
        var units = new List<XElement>();
        ContextInformation? cxt = null;
        foreach (var translationUnit in sdlxliffDocument.TranslationUnitsAll)
        {
            if (translationUnit.ContextInformation != null)
            {
                // If context is same as last trans-unit, we add the trans-unit to the last group in the list
                if (ReferenceEquals(cxt, translationUnit.ContextInformation))
                {
                    units.Last().Add(XmlElementBuilder.BuildTranslationUnit(translationUnit));
                }
                // Otherwise, we create another group and add the current trans-unit to it
                else
                {
                    cxt = translationUnit.ContextInformation;
                    units.Add(new XElement(SdlxliffNames.Xliff12 + SdlxliffNames.Group,
                                    XmlElementBuilder.BuildContexts(translationUnit.ContextInformation),
                                    XmlElementBuilder.BuildTranslationUnit(translationUnit)
                                    )
                              );
                }
            }
            else
            {
                units.Add(XmlElementBuilder.BuildTranslationUnit(translationUnit));
            }
        }

        // Write all groups/trans-units to writer
        await foreach (var unit in units.ToAsyncEnumerable())
        {
            await unit.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
        }
    }

    private async Task WriteCommentDefinitionsAsync()
    {
        if (sdlxliffDocument.CommentMarkers.Count > 0 || sdlxliffDocument.Header.HasFileLevelComments)
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.CmtDefs, null).ConfigureAwait(false);

            // Segment level comments
            // Possible to have multiple comment markers referring to the same id
            HashSet<string> seenIds = new HashSet<string>();
            foreach (var commentMarker in sdlxliffDocument.CommentMarkers)
            {
                if (seenIds.Add(commentMarker.Id))
                {
                    var cmtDef = XmlElementBuilder.BuildCommentDefinition(commentMarker.Id, commentMarker.CommentDefinition);
                    await cmtDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
                }
            }

            // File level comments
            if (sdlxliffDocument.Header.HasFileLevelComments)
            {
                var fileLevelComment = sdlxliffDocument.Header.FileLevelComments;
                var cmtDef = XmlElementBuilder.BuildCommentDefinition(fileLevelComment.Id, fileLevelComment.CommentDefinition);
                await cmtDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // cmt-defs
        }
    }

    private async Task WriteCommentMetaDefinitionsAsync()
    {
        if (sdlxliffDocument.CommentMetaDefinitions.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.CmtMetaDefs, null).ConfigureAwait(false);

            foreach (var pair in sdlxliffDocument.CommentMetaDefinitions)
            {
                var cmtMetaDef = XmlElementBuilder.BuildCommentMetaDefinition(pair.Key, pair.Value);
                await cmtMetaDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // cmt-meta-defs
        }
    }

    private async Task WriteContextDefinitionsAsync()
    {
        var contextDefinitions = sdlxliffDocument.Header.ContextDefinitions;

        if (contextDefinitions.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(string.Empty, SdlxliffNames.CxtDefs, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Xmlns, null, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);

            foreach (var pair in contextDefinitions)
            {
                var cxtDef = XmlElementBuilder.BuildContextDefinition(pair.Key, pair.Value);
                await cxtDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // cxt-defs
        }
    }

    private async Task WriteDocumentInformationAsync()
    {
        if (HasDocumentInformation())
        {
            await xmlWriter.WriteStartElementAsync(string.Empty, SdlxliffNames.DocInfo, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Xmlns, null, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);

            await WriteRepetitionDefinitionsAsync().ConfigureAwait(false);
            await WriteRevisionDefinitionsAsync().ConfigureAwait(false);
            await WriteCommentDefinitionsAsync().ConfigureAwait(false);
            await WriteCommentMetaDefinitionsAsync().ConfigureAwait(false);

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // doc-info
        }
    }

    private async Task WriteFileInfoAsync()
    {
        var fileInfo = sdlxliffDocument.Header.FileInfo;
        if (fileInfo.Values.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(string.Empty, SdlxliffNames.FileInfo, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Xmlns, null, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);

            foreach (var value in XmlElementBuilder.BuildKeyValuePairs(fileInfo.Values))
            {
                await value.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }

            var sniffInfo = XmlElementBuilder.BuildSniffInfo(fileInfo.SniffInfo);
            await sniffInfo.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // file-info
        }
    }

    private async Task WriteFileLevelCommentsAsync()
    {
        if (sdlxliffDocument.Header.HasFileLevelComments)
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.Cmt, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Id, null, sdlxliffDocument.Header.FileLevelComments.Id).ConfigureAwait(false);
            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // sdl:cmt
        }
    }

    private async Task WriteFileTypeInfoAsync()
    {
        var fileTypeInfo = sdlxliffDocument.Header.FileTypeInfo;

        if (!string.IsNullOrEmpty(fileTypeInfo.FileTypeId))
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.FileTypeInfo, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.FileTypeId, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteStringAsync(fileTypeInfo.FileTypeId).ConfigureAwait(false);
            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // sdl:filetype-info
            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);  // sdl::filetype-id
        }
    }

    private async Task WriteFormattingDefinitionsAsync()
    {
        var formattingDefinitions = sdlxliffDocument.Header.FormattingDefinitions;

        if (formattingDefinitions.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(string.Empty, SdlxliffNames.FmtDefs, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Xmlns, null, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);

            foreach (var pair in formattingDefinitions)
            {
                var fmtDef = XmlElementBuilder.BuildFormattingDefinition(pair.Key, pair.Value);
                await fmtDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // fmt-defs
        }
    }

    private async Task WriteHeaderAsync()
    {
        await WriteReferenceAsync().ConfigureAwait(false);
        await WriteReferenceFilesAsync().ConfigureAwait(false);
        await WriteFileInfoAsync().ConfigureAwait(false);
        await WriteFileTypeInfoAsync().ConfigureAwait(false);
        await WriteFormattingDefinitionsAsync().ConfigureAwait(false);
        await WriteContextDefinitionsAsync().ConfigureAwait(false);
        await WriteNodeDefinitionsAsync().ConfigureAwait(false);
        await WriteTagDefinitionsAsync().ConfigureAwait(false);
        await WriteFileLevelCommentsAsync().ConfigureAwait(false);
    }

    private async Task WriteNodeDefinitionsAsync()
    {
        if (sdlxliffDocument.Header.NodeDefinitions.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(string.Empty, SdlxliffNames.NodeDefs, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Xmlns, null, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);

            foreach (var pair in sdlxliffDocument.Header.NodeDefinitions)
            {
                var nodeDef = XmlElementBuilder.BuildNodeDefinition(pair.Key, pair.Value);
                await nodeDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // node-defs
        }
    }

    private async Task WriteReferenceAsync()
    {
        await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.Reference, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
        var reference = sdlxliffDocument.Header.Reference;
        switch (reference.FileType)
        {
            case Model.Common.ReferenceFileType.externalfile:
                await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.ExternalFile, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
                await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.HRef, null, reference.ExternalFile.HRef).ConfigureAwait(false);
                await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Uid, null, reference.ExternalFile.UId).ConfigureAwait(false);
                break;

            case Model.Common.ReferenceFileType.internalfile:
                await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.InternalFile, SdlxliffNames.Xliff12.NamespaceName).ConfigureAwait(false);
                await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Form, null, reference.InternalFile.Form).ConfigureAwait(false);
                await xmlWriter.WriteStringAsync(reference.InternalFile.Contents).ConfigureAwait(false);
                break;

            default:
                throw new InvalidOperationException("Unknown reference file type");
        }

        await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // internal-file or external-file
        await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // reference
    }

    private async Task WriteReferenceFilesAsync()
    {
        var referenceFiles = sdlxliffDocument.Header.ReferenceFiles;
        if (referenceFiles.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.RefFiles, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            foreach (var referenceFile in referenceFiles)
            {
                var refFile = XmlElementBuilder.BuildReferenceFile(referenceFile);
                await refFile.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }
            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);  // sdl:ref-files
        }
    }

    private async Task WriteRepetitionDefinitionsAsync()
    {
        if (sdlxliffDocument.RepetitionDefinitions.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.RepDefs, null).ConfigureAwait(false);

            foreach (var pair in sdlxliffDocument.RepetitionDefinitions)
            {
                var repDef = XmlElementBuilder.BuildRepetitionDefinition(pair.Key, pair.Value);
                await repDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // rep-defs
        }
    }

    private async Task WriteRevisionDefinitionsAsync()
    {
        if (sdlxliffDocument.RevisionMarkers.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(null, SdlxliffNames.RevDefs, null).ConfigureAwait(false);

            // Possible to have multiple revision markers referring to the same id
            HashSet<string> seenIds = new HashSet<string>();
            foreach (var revMarker in sdlxliffDocument.RevisionMarkers)
            {
                if (seenIds.Add(revMarker.Id))
                {
                    var repDef = XmlElementBuilder.BuildRevisionDefinition(revMarker.Id, revMarker.RevisionDefinition);
                    await repDef.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
                }
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // rev-defs
        }
    }

    private async Task WriteTagDefinitionsAsync()
    {
        var tagPairs = sdlxliffDocument.Header.TagDefinitions.TagPairInfo;
        var placeholders = sdlxliffDocument.Header.TagDefinitions.PlaceholderInfo;
        var structureTags = sdlxliffDocument.Header.TagDefinitions.StructureTagInfo;

        if (placeholders.Count > 0 || structureTags.Count > 0 || tagPairs.Count > 0)
        {
            await xmlWriter.WriteStartElementAsync(string.Empty, SdlxliffNames.TagDefs, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);
            await xmlWriter.WriteAttributeStringAsync(null, SdlxliffNames.Xmlns, null, SdlxliffNames.Sdlxliff.NamespaceName).ConfigureAwait(false);

            if (tagPairs.Count > 0)
            {
                foreach (var pair in tagPairs)
                {
                    var tag = XmlElementBuilder.BuildTagDefinition(pair.Key, pair.Value);
                    await tag.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
                }
            }

            if (placeholders.Count > 0)
            {
                foreach (var pair in placeholders)
                {
                    var tag = XmlElementBuilder.BuildTagDefinition(pair.Key, pair.Value);
                    await tag.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
                }
            }

            if (structureTags.Count > 0)
            {
                foreach (var pair in structureTags)
                {
                    var tag = XmlElementBuilder.BuildTagDefinition(pair.Key, pair.Value);
                    await tag.WriteToAsync(xmlWriter, CancellationToken.None).ConfigureAwait(false);
                }
            }

            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false); // tag-defs
        }
    }
}
