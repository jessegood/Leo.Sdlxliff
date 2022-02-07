using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

public class EndPairedTagProperties : IValueElement
{
    public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();

    public EndPairedTagProperties DeepCopy()
    {
        EndPairedTagProperties copy = new();

        foreach (var pair in Values)
        {
            copy.Values.Add(pair.Key, pair.Value);
        }

        return copy;
    }
}