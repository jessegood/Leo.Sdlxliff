using System.Xml;
using System.Xml.Linq;

namespace Leo.Sdlxliff.Readers;

internal static class XmlExceptionHelper
{
    private const string lineNumberMessage = ", Line number: ";
    private const string linePositionMessage = ", Line position: ";
    private const string parentElementMessage = ", Parent: ";
    private const string unknownAttributeMessage = "Unknown attribute: ";
    private const string unknownElementMessage = "Unknown element: ";

    internal static void ThrowUnknownAttributeException(XAttribute attribute)
    {
        IXmlLineInfo info = attribute;
        var parent = GetParent(attribute);
        if (info.HasLineInfo())
        {
            throw new XmlException($"{unknownAttributeMessage}{attribute.Name.LocalName}{parentElementMessage}{parent}{lineNumberMessage}{info.LineNumber}{linePositionMessage}{info.LinePosition}");
        }

        throw new XmlException($"{unknownAttributeMessage}{attribute.Name.LocalName}{parentElementMessage}{parent}");
    }

    internal static void ThrowUnknownElementException(XElement element)
    {
        IXmlLineInfo info = element;
        var parent = GetParent(element);
        if (info.HasLineInfo())
        {
            throw new XmlException($"{unknownElementMessage}{element.Name.LocalName}{parentElementMessage}{parent}{lineNumberMessage}{info.LineNumber}{linePositionMessage}{info.LinePosition}");
        }

        throw new XmlException($"{unknownElementMessage}{element.Name.LocalName}{parentElementMessage}{parent}");
    }

    private static string GetParent(XObject obj)
    {
        return obj.Parent is null ? "Unknown" : obj.Parent.Name.LocalName;
    }
}
