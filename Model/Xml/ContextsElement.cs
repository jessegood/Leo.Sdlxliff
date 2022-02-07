using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>sdl:cxts</c> element. Child of <c>group</c> element.
/// </summary>
public class ContextsElement
{
    /// <summary>
    /// List of <c>sdl:cxt</c>.
    /// <see cref="Leo.Sdlxliff.Model.Xml.Context"/>
    /// </summary>
    public IList<Context> Contexts { get; set; } = new List<Context>();
    /// <summary>
    /// Id referring to <c>node-def</c> element.
    /// <see cref="Leo.Sdlxliff.Model.Xml.NodeDefinition"/>
    /// </summary>
    public string NodeId { get; set; } = string.Empty;
}
