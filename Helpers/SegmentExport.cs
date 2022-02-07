using Leo.Sdlxliff.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Leo.Sdlxliff.Helpers;

/// <summary>
/// A helper class that exports segments to various formats
/// </summary>
public static class SegmentExport
{
    public static async Task ToTabSeparatedValuesAsync(string filePath, IEnumerable<ISegmentPair> pairs)
    {
        var stringBuilder = new StringBuilder();
        using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        foreach (var pair in pairs)
        {
            stringBuilder.Append($"{pair.SourceSegment}\t{pair.TargetSegment}{Environment.NewLine}");
        }
        await file.WriteAsync(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
    }
}
