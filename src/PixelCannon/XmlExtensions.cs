using System;
using System.Xml.Linq;

namespace PixelCannon
{
    internal static class XmlExtensions
    {
        public static int ValueAsInt32(this XElement element, int defaultValue = default)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!int.TryParse(element.Value, out int value))
                return defaultValue;

            return value;
        }

        public static int AttributeValueAsInt32(this XElement element, XName attributeName, int defaultValue = default)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var attribute = element.Attribute(attributeName);

            if (attribute == null || !int.TryParse(attribute.Value, out int value))
                return defaultValue;

            return value;
        }
    }
}
