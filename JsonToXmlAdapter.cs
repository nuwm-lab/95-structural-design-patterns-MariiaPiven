using System;
using System.Text.Json;
using System.Xml.Linq;
using JsonToXmlAdapterExample.Contracts;
using JsonToXmlAdapterExample.Options;

namespace JsonToXmlAdapterExample.Adapters
{
    // Adapter that converts JSON to XML using ConverterOptions for customization
    public class JsonToXmlAdapter : IConverter
    {
    public string Convert(string input, ConverterOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input JSON is empty", nameof(input));

            if (options == null) options = new ConverterOptions();

            using var doc = JsonDocument.Parse(input);

            var rootElement = new XElement(options.RootName);
            ConvertElement(doc.RootElement, rootElement, options);

            var xdoc = new XDocument(rootElement);
            return xdoc.ToString();
        }

        private void ConvertElement(JsonElement element, XElement parent, ConverterOptions options)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        // Handle possible omission for null values
                        if (prop.Value.ValueKind == JsonValueKind.Null && options.NullHandling == NullHandling.OmitElement)
                        {
                            continue;
                        }

                        var child = new XElement(prop.Name);
                        parent.Add(child);
                        ConvertElement(prop.Value, child, options);
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Null && options.NullHandling == NullHandling.OmitElement)
                        {
                            continue;
                        }

                        var itemEl = new XElement(options.ArrayItemName);
                        parent.Add(itemEl);
                        ConvertElement(item, itemEl, options);
                    }
                    break;

                case JsonValueKind.String:
                    parent.Value = element.GetString();
                    break;

                case JsonValueKind.Number:
                    parent.Value = element.GetRawText();
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    parent.Value = FormatBoolean(element.GetBoolean(), options.BooleanHandling);
                    break;

                case JsonValueKind.Null:
                    switch (options.NullHandling)
                    {
                        case NullHandling.EmptyElement:
                            parent.Value = string.Empty;
                            break;
                        case NullHandling.LiteralNull:
                            parent.Value = "null";
                            break;
                        case NullHandling.OmitElement:
                            // The caller should avoid adding the element when OmitElement is configured.
                            parent.Remove();
                            break;
                    }
                    break;

                default:
                    parent.Value = element.GetRawText();
                    break;
            }
        }

        private static string FormatBoolean(bool value, BooleanHandling handling)
        {
            return handling switch
            {
                BooleanHandling.TrueFalse => value.ToString(),
                BooleanHandling.Lowercase => value.ToString().ToLowerInvariant(),
                BooleanHandling.Numeric => value ? "1" : "0",
                _ => value.ToString(),
            };
        }
    }
}

