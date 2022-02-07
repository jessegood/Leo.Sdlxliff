using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model;
using Leo.Sdlxliff.Model.Common;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Helpers;

public static class SegmentConverterHelper
{
    public static string? ToStringWithoutCommentsAndRevisions(this ISegment segment)
    {
        List<TranslationUnitContentContainer> removeContainer = new();
        List<ITranslationUnitContent> removeAll = new();

        if (ContentLookupHelper.HasComment(segment.AllContent()) || ContentLookupHelper.HasRevision(segment.AllContent()))
        {
            foreach (var content in segment.AllContent())
            {
                if (content is CommentMarker commentMarker)
                {
                    removeContainer.Add(commentMarker);
                }

                if (content is RevisionMarker revisionMarker)
                {
                    switch (revisionMarker.RevisionDefinition.Type)
                    {
                        // For insertions, we treat them like comments
                        // and only remove the surrounding tags
                        case RevisionType.Insert:
                        case RevisionType.Unchanged:
                        case RevisionType.FeedbackComment:
                        case RevisionType.FeedbackAdded:
                            removeContainer.Add(revisionMarker);
                            break;

                        // For deletions, we remove the entire thing
                        // from the segment
                        case RevisionType.FeedbackDeleted:
                        case RevisionType.Delete:
                            removeAll.Add(revisionMarker);
                            break;
                    }
                }
            }
        }

        foreach (var container in removeContainer)
        {
            container.RemoveContentContainerFromParent();
        }

        foreach (var content in removeAll)
        {
            content.RemoveFromParent();
        }

        return segment.ToString();
    }
}