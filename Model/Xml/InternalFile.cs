using System;
using System.IO;

namespace Leo.Sdlxliff.Model.Xml;

public class InternalFile
{
    public InternalFile(string value)
    {
        Contents = value;
    }

    public string Contents { get; } = string.Empty;
    public string Form { get; set; } = string.Empty;

    /// <summary>
    /// Writes the internal file to the path you specify.
    /// Note that the file is a zip file, so the caller should specify the .zip extension.
    /// </summary>
    /// <param name="path">The save path. It is recommended to specify the .zip extension.</param>
    public void WriteToFile(string path)
    {
        if (string.IsNullOrWhiteSpace(Contents))
        {
            throw new InvalidOperationException("There no internal file");
        }

        var array = Convert.FromBase64String(Contents);
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        fs.Write(array, 0, array.Length);
    }
}
