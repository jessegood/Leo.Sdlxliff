using Leo.Sdlxliff.Interfaces;
using System.Linq;
using System.Xml.Linq;

namespace Leo.Sdlxliff.Readers;

internal static class SdlxliffReaderHelper
{
    internal static string GetIdAttribute(XElement element)
    {
        return (string?)element.Attribute(SdlxliffNames.Id) ?? string.Empty;
    }

    internal static void HandleValueElement(IValueElement valueElement, XElement element, bool isValueElement = false)
    {
        if (isValueElement)
        {
            GetKeyAndValue(valueElement, element);
            return;
        }

        foreach (var child in element.Elements())
        {
            switch (child.Name.LocalName)
            {
                case SdlxliffNames.Value:
                    GetKeyAndValue(valueElement, child);
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownElementException(child);
                    break;
            }
        }
    }

    private static void GetKeyAndValue(IValueElement valueElement, XElement element)
    {
        foreach (var attribute in element.Attributes())
        {
            switch (attribute.Name.LocalName)
            {
                case SdlxliffNames.Key:
                    var key = attribute.Value;
                    var value = string.Join("", element.Nodes().OfType<XText>().Select(x => x.Value));

                    if (key != null)
                    {
                        valueElement.Values.Add(key, value);
                    }
                    break;

                default:
                    XmlExceptionHelper.ThrowUnknownAttributeException(attribute);
                    break;
            }
        }
    }
}
