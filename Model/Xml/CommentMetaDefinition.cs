using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

public class CommentMetaDefinition
{
    public IList<CommentMetaData> CommentMetaDatas { get; } = new List<CommentMetaData>();
}
