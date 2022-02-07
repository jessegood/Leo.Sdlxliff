using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>doc-info</c> element.
/// Children are <c>cmt-defs</c>, <c>cmt-meta-defs</c>, <c>rep-defs</c>, and <c>rev-defs</c>
/// </summary>
public class DocumentInformation
{
    public IDictionary<string, CommentDefinition> CommentDefinitions { get; private set; } = new Dictionary<string, CommentDefinition>();
    public IDictionary<string, CommentMetaDefinition> CommentMetaDefinitions { get; private set; } = new Dictionary<string, CommentMetaDefinition>();
    public IDictionary<string, IList<Entry>> RepetitionDefinitions { get; private set; } = new Dictionary<string, IList<Entry>>();
    public IDictionary<string, RevisionDefinition> RevisionDefinitions { get; private set; } = new Dictionary<string, RevisionDefinition>();

    internal void AddCommentDefinitions(IDictionary<string, CommentDefinition> commentDefinitions)
    {
        CommentDefinitions = commentDefinitions;
    }

    internal void AddCommentMetaDefinitions(IDictionary<string, CommentMetaDefinition> commentMetaDefinitions)
    {
        CommentMetaDefinitions = commentMetaDefinitions;
    }

    internal void AddRepetitionDefinitions(IDictionary<string, IList<Entry>> repetitionDefinitions)
    {
        RepetitionDefinitions = repetitionDefinitions;
    }

    internal void AddRevisionDefinitions(IDictionary<string, RevisionDefinition> revisionDefinitions)
    {
        RevisionDefinitions = revisionDefinitions;
    }
}
