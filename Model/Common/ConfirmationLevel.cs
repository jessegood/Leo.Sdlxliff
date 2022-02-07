using System;

namespace Leo.Sdlxliff.Model.Common;

[Flags]
public enum ConfirmationLevel
{
    Unspecified = 0,
    NotTranslated = 1,
    Draft = 2,
    Translated = 4,
    RejectedTranslation = 8,
    ApprovedTranslation = 16,
    RejectedSignOff = 32,
    ApprovedSignOff = 64
}
