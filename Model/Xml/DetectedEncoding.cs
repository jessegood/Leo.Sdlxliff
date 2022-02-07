using Leo.Sdlxliff.Interfaces;

namespace Leo.Sdlxliff.Model.Xml;

public class DetectedEncoding : IOptionalElement
{
    public string DetectionLevel { get; set; } = string.Empty;
    public string Encoding { get; set; } = string.Empty;
    public bool IsEmpty => string.IsNullOrEmpty(DetectionLevel) && string.IsNullOrEmpty(Encoding);
}
