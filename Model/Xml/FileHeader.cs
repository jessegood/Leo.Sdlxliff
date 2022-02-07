namespace Leo.Sdlxliff.Model.Xml;

public class FileHeader
{
    public string DataType { get; internal set; } = string.Empty;
    public string Original { get; internal set; } = string.Empty;
    public string SourceLanguage { get; internal set; } = string.Empty;
    public string TargetLanguage { get; internal set; } = string.Empty;
}
