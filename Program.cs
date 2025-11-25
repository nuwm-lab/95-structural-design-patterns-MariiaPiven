using System;
using LabWork.Adapters;
using System.Text.Json;
using System.Xml.Linq;

namespace LabWork.Adapters
{
    // Адаптер який реалізує IConverter і перетворює JSON у XML
    public class JsonToXmlAdapter : IConverter
    {
        public string Convert(string input)
        {
            // Демонстрація використання адаптера (JSON -> XML)
            string sampleJson = @"{
  ""person"": {
    ""name"": ""Maria Piven"",
    ""age"": 24,
    ""phones"": [""+380501234567"", ""+380671112233""],
    ""address"": {
      ""city"": ""Kyiv"",
      ""street"": ""Main St""
    }
  }
}";

            IConverter converter = new JsonToXmlAdapter();

            try
            {
                string xml = converter.Convert(sampleJson);
                Console.WriteLine("Converted XML:\n");
                Console.WriteLine(xml);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Conversion failed: " + ex.Message);
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
}
