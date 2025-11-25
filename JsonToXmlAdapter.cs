using System;
using System.Text.Json;
using System.Xml.Linq;
using System.Globalization;

namespace LabWork.Adapters
{
    // Адаптер який реалізує IConverter і перетворює JSON у XML
    public class JsonToXmlAdapter : IConverter
    {
        private readonly ConverterOptions _options;

        public JsonToXmlAdapter(ConverterOptions options = null)
        {
            _options = options ?? new ConverterOptions();
        }

        public string Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input JSON is empty", nameof(input));

            try
            {
                using var doc = JsonDocument.Parse(input);

                var rootElement = new XElement("Root");
                ConvertElement(doc.RootElement, rootElement);

                var xdoc = new XDocument(rootElement);
                return xdoc.ToString();
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"JSON parsing failed: {ex.Message}");
                throw new ArgumentException("Invalid JSON input: failed to parse JSON.", ex);
            }
        }

        private void ConvertElement(JsonElement element, XElement parent)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        var child = new XElement(prop.Name);
                        if (_options.IncludeTypeAttribute)
                            child.SetAttributeValue("type", MapJsonValueKind(prop.Value.ValueKind));
                        parent.Add(child);
                        ConvertElement(prop.Value, child);
                    }
                    break;
                case JsonValueKind.Array:
                    // Determine item element name based on strategy and parent's element name
                    var parentName = parent.Name.LocalName;
                    string itemName = DetermineItemName(parentName);

                    foreach (var item in element.EnumerateArray())
                    {
                        var itemEl = new XElement(itemName);
                        parent.Add(itemEl);
                        if (_options.IncludeTypeAttribute)
                            itemEl.SetAttributeValue("type", MapJsonValueKind(item.ValueKind));
                        ConvertElement(item, itemEl);
                    }
                    break;
                case JsonValueKind.String:
                    if (_options.IncludeTypeAttribute)
                        parent.SetAttributeValue("type", "string");
                    parent.Value = element.GetString() ?? string.Empty;
                    break;
                case JsonValueKind.Number:
                    if (_options.IncludeTypeAttribute)
                        parent.SetAttributeValue("type", "number");
                    // Format numbers using invariant culture to avoid culture-dependent separators
                    if (element.TryGetInt64(out long l))
                        parent.Value = l.ToString(CultureInfo.InvariantCulture);
                    else if (element.TryGetDouble(out double d))
                        parent.Value = d.ToString("G", CultureInfo.InvariantCulture);
                    else
                        parent.Value = element.GetRawText();
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    if (_options.IncludeTypeAttribute)
                        parent.SetAttributeValue("type", "boolean");
                    // Use lowercase invariant boolean representation
                    parent.Value = element.GetBoolean().ToString().ToLowerInvariant();
                    break;
                case JsonValueKind.Null:
                    if (_options.IncludeTypeAttribute)
                        parent.SetAttributeValue("type", "null");
                    if (_options.IncludeNullAttribute)
                        parent.SetAttributeValue("isNull", "true");
                    parent.Value = string.Empty;
                    break;
                default:
                    parent.Value = element.GetRawText();
                    break;
            }
        }

        private string DetermineItemName(string parentName)
        {
            return _options.ItemNaming switch
            {
                ItemNamingStrategy.Item => "Item",
                ItemNamingStrategy.Custom when !string.IsNullOrWhiteSpace(_options.CustomItemName) => _options.CustomItemName!,
                ItemNamingStrategy.RepeatPropertyName => Singularize(parentName),
                _ => "Item",
            };
        }

        private static string Singularize(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Item";
            // Simple heuristics: 'ies' -> 'y', trailing 's' -> remove
            if (name.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && name.Length > 3)
                return name.Substring(0, name.Length - 3) + "y";
            if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase) && name.Length > 1)
                return name.Substring(0, name.Length - 1);
            return name + "Item";
        }

        private static string MapJsonValueKind(JsonValueKind kind)
        {
            return kind switch
            {
                JsonValueKind.Object => "object",
                JsonValueKind.Array => "array",
                JsonValueKind.String => "string",
                JsonValueKind.Number => "number",
                JsonValueKind.True => "boolean",
                JsonValueKind.False => "boolean",
                JsonValueKind.Null => "null",
                _ => "unknown",
            };
        }
    }
}
