using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

public class FileInfo : IValueElement
{
    public SniffInfo SniffInfo { get; set; } = new SniffInfo();
    public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();
}
