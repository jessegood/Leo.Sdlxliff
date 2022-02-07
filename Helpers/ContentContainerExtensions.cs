using Leo.Sdlxliff.Model.Common;

namespace Leo.Sdlxliff.Helpers;

public static class ContentContainerExtensions
{
    public static void RemoveContentContainerFromParent<T>(this T content) where T : TranslationUnitContentContainer
    {
        var parent = content.Parent;
        if (parent != null)
        {
            parent.InsertRange(content.Index, content.Contents);

            // Need to update the parent, otherwise
            // when you call MergeAdjacentTextContent, it will only remove from the previous parent
            foreach (var c in content.Contents)
            {
                c.Parent = parent;
            }

            content.RemoveFromParent();
            parent.MergeAdjacentTextContent();
        }
    }
}
