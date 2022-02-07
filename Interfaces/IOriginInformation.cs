using Leo.Sdlxliff.Model.Common;
using Leo.Sdlxliff.Model.Xml;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Interfaces;

public interface IOriginInformation
{
    string Origin { get; set; }
    string OriginSystem { get; set; }
    byte Percent { get; set; }
    PreviousOrigin? Previous { get; set; }
    string RepetitionId { get; set; }
    bool StructMatch { get; set; }
    TextContextMatchLevel TextMatch { get; set; }
    IDictionary<string, string> Values { get; }
}
