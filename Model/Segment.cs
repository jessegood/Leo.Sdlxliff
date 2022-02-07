using Leo.Sdlxliff.Interfaces;
using Leo.Sdlxliff.Model.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Leo.Sdlxliff.Model;

public class Segment : TranslationUnitContentContainer, ISegment
{
    public override TranslationUnitContentType ContentType { get; internal set; } = TranslationUnitContentType.Segment;

    public override bool HasChildren => true;

    // This id refers to the row number shown in the editor
    // Corresponds to the segment marker "mid" number
    public override string Id { get; set; } = string.Empty;

    /// <summary>
    /// Parent of this segment.
    /// Should be <c>source</c>, <c>seg-source</c> or <c>target</c> element.
    /// </summary>
    public override TranslationUnitContentContainer? Parent { get; set; }

    public SegmentType SegmentType { get; internal set; } = SegmentType.Source;

    public bool Contains(string searchString, bool caseSensitive, bool useRegex, bool searchInTags)
    {
        if (searchInTags)
        {
            var content = ToString();
            return PerformSearch(content, searchString, caseSensitive, useRegex);
        }

        return SearchContents(searchString, caseSensitive, useRegex);
    }

    public override ITranslationUnitContent DeepCopy()
    {
        Segment copy = new()
        {
            Id = Id,
            SegmentType = SegmentType
        };

        foreach (var content in Contents)
        {
            var child = content.DeepCopy();
            child.Parent = copy;
            copy.Contents.Add(child);
        }

        return copy;
    }

    private static bool PerformSearch(string input, string searchString, bool caseSensitive, bool useRegex)
    {
        if (useRegex)
        {
            return Regex.IsMatch(input, searchString, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
        }
        else
        {
            return input.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    private bool SearchContents(string searchString, bool caseSensitive, bool useRegex)
    {
        var stack = new Stack<ITranslationUnitContent>();
        stack.Push(this);
        while (stack.Count > 0)
        {
            var content = stack.Pop();
            if (content is IText text)
            {
                if (PerformSearch(text.Contents, searchString, caseSensitive, useRegex))
                {
                    return true;
                }
            }

            if (content.HasChildren)
            {
                foreach (var subcontent in ((TranslationUnitContentContainer)content).Contents)
                {
                    stack.Push(subcontent);
                }
            }
        }

        return false;
    }
}