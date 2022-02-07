using Leo.Sdlxliff.Interfaces;

namespace Leo.Sdlxliff.Model.Xml;

public class DetectedLanguage : IOptionalElement
{
    public string DetectionLevel { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;

    public bool IsEmpty => string.IsNullOrEmpty(DetectionLevel) && string.IsNullOrEmpty(Language);
}
