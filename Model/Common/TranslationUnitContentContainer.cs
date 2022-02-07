using Leo.Sdlxliff.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace Leo.Sdlxliff.Model.Common;

public abstract class TranslationUnitContentContainer : TranslationUnitContent
{
    public IList<ITranslationUnitContent> Contents { get; set; } = new List<ITranslationUnitContent>();

    /// <summary>
    /// Gets all contents including sub-content of this container
    /// </summary>
    /// <returns>An enumerable of all contents including any subcontent</returns>
    public IEnumerable<ITranslationUnitContent> AllContent()
    {
        foreach (var content in Contents)
        {
            yield return content;

            if (content.HasChildren)
            {
                foreach (var subcontent in ((TranslationUnitContentContainer)content).AllContent())
                {
                    yield return subcontent;
                }
            }
        }
    }

    /// <summary>
    /// Inserts a collection into the list
    /// </summary>
    /// <param name="index">Index of where to insert the collection</param>
    /// <param name="contents">The collection to insert</param>
    public void InsertRange(int index, IEnumerable<ITranslationUnitContent> contents)
    {
        ((List<ITranslationUnitContent>)Contents).InsertRange(index, contents);
    }

    public void MergeAdjacentTextContent()
    {
        IText? previous = null;
        var textToRemove = new List<IText>();
        foreach (var content in Contents)
        {
            if (content is IText txt)
            {
                if (previous is null)
                {
                    previous = txt;
                    continue;
                }
                else
                {
                    // Merge text contents with previous one and then add it to the list of elements to remove
                    previous.Contents += txt.Contents;
                    textToRemove.Add(txt);
                }
            }
            else
            {
                // Reset the previous back to null if we
                // hit something other than a text node!
                previous = null;
            }
        }

        textToRemove.ForEach(t => t.RemoveFromParent());
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        foreach (var content in Contents)
        {
            stringBuilder.Append(content.ToString());
        }

        return stringBuilder.ToString();
    }
}
