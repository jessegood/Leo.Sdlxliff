namespace Leo.Sdlxliff.Model.Xml;

public class SniffInfo
{
    public DetectedEncoding DetectedEncoding { get; set; } = new DetectedEncoding();
    public DetectedLanguage DetectedSourceLanguage { get; set; } = new DetectedLanguage();
    public DetectedLanguage DetectedTargetLanguage { get; set; } = new DetectedLanguage();
    public string IsSupported { get; set; } = string.Empty;
    public Properties Properties { get; set; } = new Properties();
    public SuggestedTargetEncoding SuggestedTargetEncoding { get; set; } = new SuggestedTargetEncoding();
}
