using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Model.Xml;

public class RevisionDefinition
{
    public string Author { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string DocCategory { get; set; } = string.Empty;

    /// <summary>
    /// The feedback options are used when assessing quality using the TQA feature
    /// </summary>
    public string FbCategory { get; set; } = string.Empty;

    public string FbReplacementId { get; set; } = string.Empty;
    public string FbSeverity { get; set; } = string.Empty;

    /// <summary>
    /// The default revision type in Trados Studio is Insert.
    /// When the type is not specified, it is assumed to be an insert.
    /// </summary>
    public RevisionType Type { get; set; } = RevisionType.Insert;

    public RevisionDefinition DeepCopy()
    {
        RevisionDefinition copy = new()
        {
            Author = Author,
            Date = Date,
            DocCategory = DocCategory,
            FbCategory = FbCategory,
            FbReplacementId = FbReplacementId,
            FbSeverity = FbSeverity,
            Type = Type
        };

        return copy;
    }
}