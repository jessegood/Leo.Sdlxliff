using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>header</c> element. Child of <c>file</c> element.
/// </summary>
public class Header : IHeader
{
    public IDictionary<string, ContextDefinition> ContextDefinitions { get; internal set; } = new Dictionary<string, ContextDefinition>();
    public FileInfo FileInfo { get; internal set; } = new FileInfo();
    public FileLevelComments FileLevelComments { get; set; } = new FileLevelComments(string.Empty, new CommentDefinition());
    public FileTypeInfo FileTypeInfo { get; internal set; } = new FileTypeInfo();
    public IDictionary<string, FormattingDefinition> FormattingDefinitions { get; internal set; } = new Dictionary<string, FormattingDefinition>();
    public bool HasFileLevelComments => !string.IsNullOrEmpty(FileLevelComments.Id);
    public IDictionary<string, NodeDefinition> NodeDefinitions { get; internal set; } = new Dictionary<string, NodeDefinition>();
    public Reference Reference { get; internal set; } = new Reference();
    public IList<ReferenceFile> ReferenceFiles { get; internal set; } = new List<ReferenceFile>();
    public TagDefinitions TagDefinitions { get; internal set; } = new TagDefinitions();
}
