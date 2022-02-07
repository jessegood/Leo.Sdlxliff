namespace Leo.Sdlxliff.Model.Xml;

public class TagPairInfo
{
    public BeginPairedTag BeginPairedTag { get; set; } = new BeginPairedTag();
    public BeginPairedTagProperties BeginPairedTagProperties { get; set; } = new BeginPairedTagProperties();
    public EndPairedTag EndPairedTag { get; set; } = new EndPairedTag();
    public EndPairedTagProperties EndPairedTagProperties { get; set; } = new EndPairedTagProperties();
    public string FormatId { get; set; } = string.Empty;
}
