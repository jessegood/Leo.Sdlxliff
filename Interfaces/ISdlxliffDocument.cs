using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Leo.Sdlxliff.Interfaces;

public interface ISdlxliffDocument
{
    /// <summary>
    /// A list of all comments in the file.
    /// </summary>
    IList<CommentMarker> CommentMarkers { get; }

    /// <summary>
    /// A dictionary of comment meta definitions.
    /// Only used if you want to add meta data in the file about the comments.
    /// For example, company name, etc.
    /// </summary>
    IDictionary<string, CommentMetaDefinition> CommentMetaDefinitions { get; }

    /// <summary>
    /// Information about the file, such as target and source language.
    /// This information is located in the attributes of the <c>file</c> element.
    /// </summary>
    FileHeader FileHeader { get; }

    /// <summary>
    /// Access to all the data in the <c>header</c> element.
    /// </summary>
    IHeader Header { get; }

    /// <summary>
    /// A list of all repetition definitions in the file.
    /// </summary>
    IDictionary<string, IList<Entry>> RepetitionDefinitions { get; }

    /// <summary>
    /// A list of all revision markers in the file.
    /// </summary>
    IList<RevisionMarker> RevisionMarkers { get; }

    /// <summary>
    /// Gets a list of translation units excluding locked content.
    /// For almost all purposes, you want to call this method instead of <see cref="TranslationUnitsAll"/>
    /// </summary>
    IList<ITranslationUnit> TranslationUnits { get; }

    /// <summary>
    /// Gets all the translation units.
    /// This includes locked content and structure content.
    /// </summary>
    IList<ITranslationUnit> TranslationUnitsAll { get; }


    IDictionary<string, RevisionDefinition> RevisionDefinitions { get; }

    /// <summary>
    /// Deletes all the comments in the file
    /// </summary>
    public void RemoveAllComments();

    /// <summary>
    /// <para>Removes the comment you specify.</para>
    /// <para>
    /// Note that there can be multiple comment markers with the same id.
    /// This happens when comments span over other tags.
    /// </para>
    /// </summary>
    /// <param name="id">The id of the comment to remove.</param>
    void RemoveSegmentLevelComment(string id);

    /// <summary>
    /// Finds a segment pair based on the search string.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    /// <param name="inSource">Whether to search in the source or target. By default, it searches in the source.</param>
    /// <returns></returns>
    IEnumerable<ISegmentPair> FindSegmentPairs(string? sourceSearchString = default, string? targetSearchString = default, SegmentSearchSettings? settings = default);

    /// <summary>
    /// Saves the sdlxliff document to the specified path.
    /// </summary>
    /// <param name="filePath">The full path including the name of the file.</param>
    Task SaveAsAsync(string filePath);

    /// <summary>
    /// Saves the sdlxliff document to the specified stream.
    /// </summary>
    /// <param name="filePath">The stream to save the data to.</param>
    Task SaveAsAsync(Stream stream);

    /// <summary>
    /// Saves the sdlxliff document to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="filePath">The string builder to save the data to.</param>
    Task SaveAsAsync(StringBuilder sb);
}
