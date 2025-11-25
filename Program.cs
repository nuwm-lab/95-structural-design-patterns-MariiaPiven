using System;
using System.Text.Json;
using System.Xml.Linq;

namespace JsonToXmlAdapterExample
{
    // ==== Target ====
    public interface IConverter
    {
        string Convert(string input);
    }

    // ==== Adapter ====
    public class JsonToXmlAdapter : IConverter
    {
        public string Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input JSON is empty", nameof(input));

            using var doc = JsonDocument.Parse(input);

            var rootElement = new XElement("Root");
            ConvertElement(doc.RootElement, rootElement);

            var xdoc = new XDocument(rootElement);
            return xdoc.ToString();
        }

        private void ConvertElement(JsonElement element, XElement parent)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        var child = new XElement(prop.Name);
                        parent.Add(child);
                        ConvertElement(prop.Value, child);
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        var itemEl = new XElement("Item");
                        parent.Add(itemEl);
                        ConvertElement(item, itemEl);
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
                    parent.Value = element.GetBoolean().ToString();
                    break;

                case JsonValueKind.Null:
                    parent.Value = string.Empty;
                    break;

                default:
                    parent.Value = element.GetRawText();
                    break;
            }
        }
    }

    // ==== Client Code ====
    class Program
    {
        static void Main()
        {
            string json = @"{
                ""name"": ""Maria"",
                ""age"": 20,
                ""isStudent"": true,
                ""scores"": [10, 9, 8]
            }";

            IConverter converter = new JsonToXmlAdapter();

            string xml = converter.Convert(json);

            Console.WriteLine("=== XML Result ===");
            Console.WriteLine(xml);
        }
    }
}
