using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// Custom meta data for <see cref="Leo.Sdlxliff.Model.Xml.ContextDefinition"/>
/// and <see cref="Leo.Sdlxliff.Model.Xml.SniffInfo"/>
/// </summary>
public class Properties : IValueElement, IOptionalElement
{
    public bool IsEmpty => Values.Count == 0;
    public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();

    public Properties DeepCopy()
    {
        Properties copy = new();

        foreach (var pair in Values)
        {
            copy.Values.Add(pair.Key, pair.Value);
        }

        return copy;
    }
}
