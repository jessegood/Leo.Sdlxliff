using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static Leo.Sdlxliff.SdlxliffDocument;

namespace Leo.Sdlxliff.Readers;

public sealed class SdlxliffReader : IDisposable
{
    private readonly DocumentInformation documentInformation = new();
    private readonly Header header = new();
    private readonly SdlxliffDocumentBuilder sdlxliffDocumentBuilder = new();
    private readonly XmlReader xmlReader;
    private SdlxliffDocument? sdlxliffDocument;

    public SdlxliffReader(TextReader reader)
    {
        xmlReader = XmlReader.Create(reader, new XmlReaderSettings() { CloseInput = true, Async = true });
    }

    public SdlxliffReader(string filePath)
    {
        xmlReader = XmlReader.Create(filePath, new XmlReaderSettings() { CloseInput = true, Async = true });
    }

    public SdlxliffReader(Stream stream)
    {
        xmlReader = XmlReader.Create(stream, new XmlReaderSettings() { CloseInput = true, Async = true });
    }

    public ISdlxliffDocument SdlxliffDocument
    {
        get
        {
            if (sdlxliffDocument is null)
            {
                sdlxliffDocument = sdlxliffDocumentBuilder.
                    AddDocumentInformation(documentInformation).
                    AddHeader(header).
                    Build();
            }

            return sdlxliffDocument;
        }
    }

    public void Dispose()
    {
        xmlReader.Dispose();
    }

    public async Task ParseAsync()
    {
        await xmlReader.MoveToContentAsync().ConfigureAwait(false);

        do
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    if (!await HandleElementAsync().ConfigureAwait(false))
                    {
                        await xmlReader.ReadAsync().ConfigureAwait(false);
                    }
                    break;

                default:
                    await xmlReader.ReadAsync().ConfigureAwait(false);
                    break;
            }
        } while (!xmlReader.EOF);
    }

    private async Task HandleBodyAsync()
    {
        await xmlReader.ReadAsync().ConfigureAwait(false);

        var bodyReader = new BodyReader(documentInformation, header);
        var translationUnits = new List<ITranslationUnit>();
        bool finished = false;

        while (!finished)
        {
            switch (xmlReader.LocalName)
            {
                case SdlxliffNames.Group:
                    translationUnits.AddRange(bodyReader.HandleGroup((XElement)await XNode.ReadFromAsync(xmlReader, CancellationToken.None).ConfigureAwait(false)));
                    break;

                case SdlxliffNames.TransUnit:
                    translationUnits.Add(bodyReader.HandleTranslationUnit((XElement)await XNode.ReadFromAsync(xmlReader, CancellationToken.None).ConfigureAwait(false)));
                    break;

                default:
                    finished = true;
                    break;
            }
        }

        sdlxliffDocumentBuilder.AddTranslationUnits(translationUnits);
        sdlxliffDocumentBuilder.AddCommentMarkers(bodyReader.CommentMarkers);
        sdlxliffDocumentBuilder.AddRevisionMarkers(bodyReader.RevisionMarkers);
    }

    private void HandleDocInfo(XElement element)
    {
        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.RepDefs:
                    documentInformation.AddRepetitionDefinitions(DocumentationInformationReader.HandleRepDefs(child));
                    break;

                case SdlxliffNames.RevDefs:
                    documentInformation.AddRevisionDefinitions(DocumentationInformationReader.HandleRevDefs(child));
                    break;

                case SdlxliffNames.CmtDefs:
                    documentInformation.AddCommentDefinitions(DocumentationInformationReader.HandleCmtDefs(child));
                    break;

                case SdlxliffNames.CmtMetaDefs:
                    documentInformation.AddCommentMetaDefinitions(DocumentationInformationReader.HandleCmtMetaDefs(child));
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private async Task<bool> HandleElementAsync()
    {
        bool handled = false;
        switch (xmlReader.LocalName)
        {
            case SdlxliffNames.Xliff:
                break;

            case SdlxliffNames.DocInfo:
                HandleDocInfo((XElement)await XNode.ReadFromAsync(xmlReader, CancellationToken.None).ConfigureAwait(false));
                handled = true;
                break;

            case SdlxliffNames.File:
                HandleFileHeader();
                break;

            case SdlxliffNames.Header:
                HandleHeader((XElement)await XNode.ReadFromAsync(xmlReader, CancellationToken.None).ConfigureAwait(false));
                handled = true;
                break;

            case SdlxliffNames.Body:
                await HandleBodyAsync().ConfigureAwait(false);
                break;

            default:
                throw new XmlException($"Unknown element: {xmlReader.LocalName}.");
        }

        return handled;
    }

    private void HandleFileHeader()
    {
        var fileHeader = new FileHeader();

        for (int i = 0; i < xmlReader.AttributeCount; i++)
        {
            xmlReader.MoveToAttribute(i);
            switch (xmlReader.Name)
            {
                case SdlxliffNames.Original:
                    fileHeader.Original = xmlReader.Value;
                    break;

                case SdlxliffNames.DataType:
                    fileHeader.DataType = xmlReader.Value;
                    break;

                case SdlxliffNames.SourceLanguage:
                    fileHeader.SourceLanguage = xmlReader.Value;
                    break;

                case SdlxliffNames.TargetLanguage:
                    fileHeader.TargetLanguage = xmlReader.Value;
                    break;

                case SdlxliffNames.Xmlns:
                    // Ignore it if it appears
                    break;

                default:
                    throw new XmlException($"Unknown attribute: {xmlReader.Name}. Element name: file");
            }
        }

        sdlxliffDocumentBuilder.AddFileHeader(fileHeader);
    }

    private void HandleHeader(XElement element)
    {
        var headerReader = new HeaderReader();

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Reference:
                    header.Reference = HeaderReader.HandleReference(child);
                    break;

                case SdlxliffNames.RefFiles:
                    header.ReferenceFiles = HeaderReader.HandleReferenceFiles(child);
                    break;

                case SdlxliffNames.FileInfo:
                    header.FileInfo = HeaderReader.HandleFileInfo(child);
                    break;

                case SdlxliffNames.FileTypeInfo:
                    header.FileTypeInfo = HeaderReader.HandleFileTypeInfo(child);
                    break;

                case SdlxliffNames.FmtDefs:
                    header.FormattingDefinitions = headerReader.HandleFormattingDefinitions(child);
                    break;

                case SdlxliffNames.CxtDefs:
                    header.ContextDefinitions = HeaderReader.HandleContextDefinitions(child);
                    break;

                case SdlxliffNames.NodeDefs:
                    header.NodeDefinitions = HeaderReader.HandleNodeDefinitions(child);
                    break;

                case SdlxliffNames.TagDefs:
                    header.TagDefinitions = headerReader.HandleTagDefinitions(child);
                    break;

                case SdlxliffNames.Cmt:
                    var id = SdlxliffReaderHelper.GetIdAttribute(child);
                    if (documentInformation.CommentDefinitions.TryGetValue(id, out var commentDefinition))
                    {
                        header.FileLevelComments = new FileLevelComments(id, commentDefinition);
                    }
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }
}
