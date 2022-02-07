using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Model.Xml;

public class Reference
{
    public ReferenceFileType FileType { get; set; } = ReferenceFileType.internalfile;
    public ExternalFile ExternalFile { get; set; } = new ExternalFile();
    public InternalFile InternalFile { get; set; } = new InternalFile(string.Empty);
}
