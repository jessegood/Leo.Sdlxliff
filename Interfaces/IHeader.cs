using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Interfaces;

public interface IHeader
{
    IDictionary<string, ContextDefinition> ContextDefinitions { get; }
    FileInfo FileInfo { get; }
    FileLevelComments FileLevelComments { get; set; }
    FileTypeInfo FileTypeInfo { get; }
    IDictionary<string, FormattingDefinition> FormattingDefinitions { get; }
    bool HasFileLevelComments { get; }
    IDictionary<string, NodeDefinition> NodeDefinitions { get; }
    Reference Reference { get; }
    IList<ReferenceFile> ReferenceFiles { get; }
    TagDefinitions TagDefinitions { get; }
}