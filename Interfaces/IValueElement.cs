using System.Collections.Generic;

namespace Leo.Sdlxliff.Interfaces;

internal interface IValueElement
{
    public IDictionary<string, string> Values { get; }
}
