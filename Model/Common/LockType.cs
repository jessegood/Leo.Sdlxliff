namespace Leo.Sdlxliff.Model.Common;

[System.Flags]
public enum LockType
{
    /// <summary>
    /// Paragraph units that are unlocked.
    /// This is different than locked segments.
    /// </summary>
    Unlocked = 0,

    /// <summary>
    /// Indicates structure paragraph units
    /// </summary>
    Structure = 1,

    /// Applied to paragraph units in the bilingual API when a document has been split
    /// into multiple instances, so that multiple translators, for example, can work on it
    /// simultaneously. This lock type is applied automatically to paragraph units that
    /// should not be translated in this file (when they should be translated in a different
    /// file instead).
    Externalized = 2,

    /// <summary>
    /// A lock that was explicitly inserted, as opposed to an automatically generated lock
    /// </summary>
    /// <para>
    /// The structure and externalized locks are automatically generated. This flag
    /// should be used when someone explicitly locks a paragraph unit in the bilingual API.
    /// </para>
    /// </remarks>
    Manual = 4
}
