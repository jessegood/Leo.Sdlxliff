using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;

namespace Leo.Sdlxliff.Model.Xml;

/// <summary>
/// <c>fmt-def</c> element.
/// Contains a group of formatting items that together specify a
/// fully defined formatting or formatting override.
/// </summary>
public class FormattingDefinition : IValueElement
{
    /// <summary>
    /// Examples of formatting:
    /// 
    /// key = FontName, value = Times New Roman
    /// 
    /// key = Italic, value = True
    /// 
    /// key = TextColor, value = Olive
    /// </summary>
    public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();

    public FormattingDefinition DeepCopy()
    {
        FormattingDefinition copy = new();

        foreach (var pair in Values)
        {
            copy.Values.Add(pair.Key, pair.Value);
        }

        return copy;
    }
}
