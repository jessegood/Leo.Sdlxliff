using Leo.Sdlxliff.Model.Xml;

namespace Leo.Sdlxliff.Model;

public class FileLevelComments
{
    public FileLevelComments(string id, CommentDefinition commentDefinition)
    {
        Id = id;
        CommentDefinition = commentDefinition;
    }

    public CommentDefinition CommentDefinition { get; } = new CommentDefinition();
    public string Id { get; } = string.Empty;
}
