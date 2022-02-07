using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Model.Xml;

public class Comment
{
    public Comment(string comment)
    {
        Contents = comment;
    }

    public string Contents { get; set; }

    public string Date { get; set; } = string.Empty;

    public CommentSeverity Severity { get; set; } = CommentSeverity.Undefined;

    public string User { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public Comment DeepCopy()
    {
        Comment copy = new(Contents)
        {
            Date = Date,
            Severity = Severity,
            User = User,
            Version = Version
        };

        return copy;
    }
}