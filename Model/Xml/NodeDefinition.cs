namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>node-def</c> element. Child of <c>header</c> element.
/// Provides structural information on how a context should act in a structural tree
/// </summary>
public class NodeDefinition
{
    public NodeDefinition(string id)
    {
        Id = id;
    }

    /// <summary>
    /// The <see cref="Leo.Sdlxliff.Model.Xml.ContextDefinition"/> that helps
    /// define the structure of the document
    /// </summary>
    public ContextDefinition ContextDefinition { get; set; } = new ContextDefinition(string.Empty);

    /// <summary>
    /// The id to the <c>cxt-def</c>
    /// </summary>
    public string ContextId { get; set; } = string.Empty;

    /// <summary>
    /// True if the display name provided by the context is the one that must
    /// always be used when displaying the context in a structural tree, etc.
    /// This is usually the case if a context does not have any other text associated
    /// with it, for example, an image or a table. If this is false, the client
    /// application can try and display some more useful information to the
    /// user, for example, the corresponding text in a paragraph context.
    /// </summary>
    public bool ForceName { get; set; } = true;

    /// <summary>
    /// Node definition id
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The parent structure info
    /// </summary>
    public NodeDefinition? Parent { get; set; } = null;

    /// <summary>
    /// The id referencing the parent structure info
    /// </summary>
    public string ParentId { get; set; } = string.Empty;
}
