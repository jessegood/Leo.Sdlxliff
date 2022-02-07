using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System;

namespace Leo.Sdlxliff.Model;

/// <summary>
/// A location marker represents a persistent location in the bilingual content.
/// They are created when merging segments and with other content processors.
/// </summary>
public class LocationMarker : TranslationUnitContent, IMarkerType
{
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.LocationMarker;
    public override bool HasChildren => false;
    public override string Id { get; set; } = string.Empty;

    public string MarkerType => SdlxliffNames.XSdlLocation;
    /// <summary>
    /// Parent of this content.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public override ITranslationUnitContent DeepCopy()
    {
        LocationMarker copy = new()
        {
            Id = Id
        };

        return copy;
    }

    public override string ToString()
    {
        return @$"<mrk mtype=""{SdlxliffNames.XSdlLocation}"" mid=""{Id}""/>";
    }
}
