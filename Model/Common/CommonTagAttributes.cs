namespace Leo.Sdlxliff.Model.Common;

public abstract class CommonTagAttributes
{
    /// <summary>
    /// Indicates whether the tag is allowed to be hidden during editing operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Hidden tags can easily get deleted by mistake during editing operations.
    /// Always set this to <c>false</c> for tags that represent important content that the
    /// user should not delete unknowingly.
    /// </para>
    /// Setting this to <c>true</c> does not necessarily mean that the tag will always be hidden.
    /// This will be determined by the editor (i.e. the user may change this at runtime through a setting).
    /// </para>
    /// <para>This is <c>false</c> by default for all tag types.</para>
    /// </remarks>
    public bool CanHide { get; set; } = false;

    /// <summary>
    /// Indicates whether it is valid to break the line in front of this tag during word wrap.
    /// </summary>
    /// <remarks>
    /// <para>This is <c>true</c> by default for all tag types.</para>
    /// </remarks>
    public bool LineWrap { get; set; } = true;

    /// <summary>
    /// A shortened version of a tag that can be used in translation editors
    /// that implement full and abbreviated tag views. Often the GI (Generic Identifier) without attributes.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Determines whether the tag is considered to be a new word during segmentation. It also affects the way that the
    /// caret moves when using word movement keyboard actions such as CTRL+LEFT ARROW and CTRL+RIGHT ARROW in editors.
    /// </summary>
    /// <example>
    /// During segmentation, if a tag has IsWordStop false then "hello[tag]world[/tag]" is considered to be one word - "hello[tag]world[/tag]" and
    /// if a tag has IsWordStop true then "hello[tag]world[/tag]" is considered to be two words - "hello" and "[tag]world[/tag]".
    /// </example>
    /// <para>This is <c>true</c> by default for placeholder tags, and <c>false</c> by default
    /// for start and end tags.</para>
    public bool WordEnd { get; set; } = false;
}
