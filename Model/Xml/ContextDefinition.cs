using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// The <c>cxt-def</c> element in the header.
/// </summary>
public class ContextDefinition
{
    public ContextDefinition(string id)
    {
        Id = id;
    }

    /// <summary>
    /// A short string to identify the context (usually one or two letters).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The display color associated with the context (background color, etc.)
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Additional, more descriptive information on the context.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Id referencing a <c>fmt-def</c>.
    /// <see cref="Leo.Sdlxliff.Model.Xml.FormattingDefinition"/>
    /// </summary>
    public string FormatId { get; set; } = string.Empty;

    /// <summary>
    /// The formatting group of key-value pairs
    /// </summary>
    public FormattingDefinition FormattingDefinition { get; set; } = new FormattingDefinition();

    /// <summary>
    /// Context definition id
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// A short name used as the default display for the context.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Custom meta data
    /// </summary>
    public Properties Properties { get; set; } = new Properties();

    /// <summary>
    /// <see cref="Leo.Sdlxliff.Model.Common.ContextPurpose"/>
    /// Information is the default.
    /// </summary>
    public ContextPurpose Purpose { get; set; } = ContextPurpose.Information;

    /// <summary>
    /// A short string that identifies the type of context.
    /// </summary>
    public string Type { get; set; } = string.Empty;
}
