using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

public class CommentMetaData : IValueElement
{
    public string Id { get; set; } = string.Empty;
    public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();
}
