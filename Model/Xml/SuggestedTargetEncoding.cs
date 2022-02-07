using Leo.Sdlxliff.Interfaces;

namespace Leo.Sdlxliff.Model;

public class SuggestedTargetEncoding : IOptionalElement
{
    public string Category { get; set; } = string.Empty;
    public bool IsEmpty => string.IsNullOrEmpty(Category);
}
