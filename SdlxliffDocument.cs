using Leo.Sdlxliff.Helpers;
using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using Leo.Sdlxliff.Readers;
using Leo.Sdlxliff.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leo.Sdlxliff;

public sealed class SdlxliffDocument : ISdlxliffDocument
{
    private SdlxliffDocument(DocumentInformation documentInformation, FileHeader fileHeader, Header header,
                            IList<ITranslationUnit> translationUnits, IList<CommentMarker> commentMarkers, IList<RevisionMarker> revisionMarkers)
    {
        RepetitionDefinitions = documentInformation.RepetitionDefinitions;
        CommentMetaDefinitions = documentInformation.CommentMetaDefinitions;
        RevisionDefinitions = documentInformation.RevisionDefinitions;
        FileHeader = fileHeader;
        Header = header;
        TranslationUnitsAll = translationUnits;
        CommentMarkers = commentMarkers;
        RevisionMarkers = revisionMarkers;
    }

    public IList<CommentMarker> CommentMarkers { get; }

    public IDictionary<string, CommentMetaDefinition> CommentMetaDefinitions { get; }
    public FileHeader FileHeader { get; }
    public IHeader Header { get; }
    public IDictionary<string, IList<Entry>> RepetitionDefinitions { get; }
    public IDictionary<string, RevisionDefinition> RevisionDefinitions { get; }
    public IList<RevisionMarker> RevisionMarkers { get; }

    /// <summary>
    /// Contains the list of localizable translation units
    /// </summary>
    public IList<ITranslationUnit> TranslationUnits => TranslationUnitsAll.Where(tu => !tu.IsLocked).ToList();

    /// <summary>
    /// Contains the list of all translation units, including locked content
    /// </summary>
    public IList<ITranslationUnit> TranslationUnitsAll { get; }

    public static async Task<ISdlxliffDocument> LoadAsync(Stream stream)
    {
        using var reader = new SdlxliffReader(stream);
        await reader.ParseAsync().ConfigureAwait(false);
        return reader.SdlxliffDocument;
    }

    public static async Task<ISdlxliffDocument> LoadAsync(string filePath)
    {
        try
        {
            using var reader = new SdlxliffReader(filePath);
            await reader.ParseAsync().ConfigureAwait(false);
            return reader.SdlxliffDocument;
        }
        catch (Exception e)
        {
            e.Data.Add("FileName", Path.GetFileName(filePath));
            throw;
        }
    }

    public static async Task<ISdlxliffDocument> LoadAsync(TextReader textReader)
    {
        using var reader = new SdlxliffReader(textReader);
        await reader.ParseAsync().ConfigureAwait(false);
        return reader.SdlxliffDocument;
    }

    /// <summary>
    /// Searches for segment pairs based on the criteria you specify.
    /// </summary>
    /// <param name="sourceSearchString">The source string to search.</param>
    /// <param name="targetSearchString">The target string to search.</param>
    /// <param name="settings">The search settings to use. <see cref="SegmentSearchSettings"/></param>
    /// <returns>A collection of segment pairs that matched the search criteria.</returns>
    public IEnumerable<ISegmentPair> FindSegmentPairs(string? sourceSearchString = default, string? targetSearchString = default, SegmentSearchSettings? settings = default)
    {
        if (sourceSearchString is null && targetSearchString is null && settings is null)
        {
            throw new ArgumentNullException("Cannot specify null for all the settings");
        }

        if (settings is null)
        {
            settings = new SegmentSearchSettings();
        }

        foreach (var transUnit in TranslationUnits.Where(tu => tu.HasSegmentPairs))
        {
            foreach (var segPair in transUnit.GetSegmentPairs())
            {
                if (SegmentSearchHelper.Search(sourceSearchString, targetSearchString, segPair, settings))
                {
                    yield return segPair;
                }
            }
        }

        yield break;
    }

    /// <summary>
    /// Removes all comments from the file, including file level comments
    /// </summary>
    public void RemoveAllComments()
    {
        // Insert contents of comment marker into parent and remove it
        foreach (var commentMarker in CommentMarkers)
        {
            commentMarker.RemoveContentContainerFromParent();
        }

        // Clear all comment markers from list
        CommentMarkers.Clear();

        // Set file level comments to empty class
        Header.FileLevelComments = new FileLevelComments(string.Empty, new CommentDefinition());
    }

    /// <summary>
    /// <para>Removes the comment you specify.</para>
    /// <para>
    /// Note that there can be multiple comment markers with the same id.
    /// This happens when comments span over other tags.
    /// </para>
    /// </summary>
    /// <param name="id">The id of the comment to remove.</param>
    public void RemoveSegmentLevelComment(string id)
    {
        foreach (var commentMarker in CommentMarkers.Where(c => c.Id == id))
        {
            commentMarker.RemoveContentContainerFromParent();
        }

        RemoveFromCollectionById(id, CommentMarkers);
    }

    /// <summary>
    /// Saves the file to the specified path.
    /// This will overwrite the file if it already exists.
    /// </summary>
    /// <param name="filePath">The specified path to save the file.</param>
    public async Task SaveAsAsync(string filePath)
    {
        var writer = new SdlxliffWriter(filePath, this);
        await writer.WriteAsync();
    }

    /// <summary>
    /// Saves the file to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write the file to.</param>
    public async Task SaveAsAsync(Stream stream)
    {
        var writer = new SdlxliffWriter(stream, this);
        await writer.WriteAsync();
    }

    /// <summary>
    /// Writes the file to the specified string builder.
    /// Note that the string builder is UTF-16.
    /// </summary>
    /// <param name="sb">The string builder to write the file to.</param>
    public async Task SaveAsAsync(StringBuilder sb)
    {
        var writer = new SdlxliffWriter(sb, this);
        await writer.WriteAsync();
    }

    private void RemoveFromCollectionById<T>(string id, IList<T> collection) where T : TranslationUnitContentContainer
    {
        for (int i = collection.Count - 1; i >= 0; i--)
        {
            if (collection[i].Id == id)
            {
                collection.RemoveAt(i);
            }
        }
    }

    internal sealed class SdlxliffDocumentBuilder
    {
        private readonly IList<ITranslationUnit> translationUnits = new List<ITranslationUnit>();
        private IList<CommentMarker> commentMarkers = new List<CommentMarker>();
        private DocumentInformation documentInformation = new();
        private FileHeader fileHeader = new();
        private Header header = new();
        private IList<RevisionMarker> revisionMarkers = new List<RevisionMarker>();

        public SdlxliffDocument Build()
        {
            return new SdlxliffDocument(documentInformation, fileHeader, header,
                                   translationUnits, commentMarkers, revisionMarkers);
        }

        internal void AddCommentMarkers(IList<CommentMarker> commentMarkers)
        {
            this.commentMarkers = commentMarkers;
        }

        internal SdlxliffDocumentBuilder AddDocumentInformation(DocumentInformation documentInformation)
        {
            this.documentInformation = documentInformation;
            return this;
        }

        internal void AddFileHeader(FileHeader fileHeader)
        {
            this.fileHeader = fileHeader;
        }

        internal SdlxliffDocumentBuilder AddHeader(Header header)
        {
            this.header = header;
            return this;
        }

        internal void AddRevisionMarkers(IList<RevisionMarker> revisionMarkers)
        {
            this.revisionMarkers = revisionMarkers;
        }

        internal void AddTranslationUnits(IList<ITranslationUnit> translationUnits)
        {
            ((List<ITranslationUnit>)this.translationUnits).AddRange(translationUnits);
        }
    }
}