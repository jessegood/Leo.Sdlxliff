namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// Corresponds to the <c>sub</c> element that is a child of <c>tag</c>
/// Used for localizable content, such as attributes
/// </summary>
public class SubSegment
{
    public SubSegment(string id, int length, int offset)
    {
        Id = id;
        Length = length;
        Offset = offset;
    }

    /// <summary>
    /// Reference to localizable translation unit for this subsegment
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Length in characters of the subsegment
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Point where the subsegment appears in the string
    /// </summary>
    public int Offset { get; }
}
