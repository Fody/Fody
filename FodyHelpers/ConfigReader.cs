using System.Xml;
using System.Xml.Linq;

namespace Fody
{
    /// <summary>
    /// Helper methods for reading from <see cref="BaseModuleWeaver.Config"/>.
    /// </summary>
    public static class ConfigReader
    {

        /// <summary>
        /// Read a bool from an attribute named <paramref name="name"/>.
        /// </summary>
        public static bool ReadBool(this XElement element, string name, bool defaultValue = false)
        {
            Guard.AgainstNull(nameof(element), element);
            Guard.AgainstNullAndEmpty(nameof(name), name);
            var attribute = element.Attribute(name);
            if (attribute == null)
            {
                return defaultValue;
            }

            var value = attribute.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new WeavingException($"Could not extract '{name}' from xml config. Null, empty, or whitespace are invalid.");
            }

            try
            {
               return XmlConvert.ToBoolean(value);
            }
            catch
            {
                throw new WeavingException($"Could not extract '{name}' from xml config. Value '{value}' could not be converted to a bool. Only '1', '0', 'true', or 'false' are valid.");
            }
        }
    }
}