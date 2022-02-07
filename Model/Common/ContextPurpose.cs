namespace Leo.Sdlxliff.Model.Common;

/// <summary>
/// The purpose attribute specifies the purpose of a <c>cxt-def</c> element.
/// <seealso cref="Leo.Sdlxliff.Model.Xml.ContextDefinition"/>
/// </summary>
public enum ContextPurpose
{
    /// <summary>
    /// Indicates that the context is informational in nature,
    /// specifying for example, how a term should be translated.
    /// Thus, should be displayed to anyone editing the XLIFF document.
    /// </summary>
    Information,

    /// <summary>
    /// This type of context is used for localizable tag content that has
    /// been externalized into separate ParagraphUnits.
    /// Such ParagraphUnits contain a location context that indicates the original ParagraphUnit
    /// that the tag with localizable content appears in.
    /// </summary>
    Location,

    /// <summary>
    /// May be used for context-sensitive matching.
    /// </summary>
    Match
}
