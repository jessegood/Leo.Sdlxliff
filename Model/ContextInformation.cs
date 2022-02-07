using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model;

/// <summary>
/// Contains the context information for the translation unit
/// </summary>
public class ContextInformation
{
    /// <summary>
    /// List of <c>ctx-def</c>.
    /// The most relevant context is first in the list.
    /// </summary>
    public IList<ContextDefinition> Contexts { get; } = new List<ContextDefinition>();

    public NodeDefinition NodeDefinition { get; set; } = new NodeDefinition(string.Empty);
}
