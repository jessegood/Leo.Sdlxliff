using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Helpers;

public static class ContentLookupHelper
{
    public static bool HasComment(IEnumerable<ITranslationUnitContent> allContent)
    {
        foreach (var content in allContent)
        {
            if (content.ContentType == TranslationUnitContentType.CommentsMarker)
            {
                return true;
            }
        }

        return false;
    }

    public static bool HasRevision(IEnumerable<ITranslationUnitContent> allContent)
    {
        foreach (var content in allContent)
        {
            if (content.ContentType == TranslationUnitContentType.RevisionMarker)
            {
                return true;
            }
        }

        return false;
    }
}
