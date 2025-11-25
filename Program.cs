using System;
using JsonToXmlAdapterExample.Contracts;
using JsonToXmlAdapterExample.Adapters;
using JsonToXmlAdapterExample.Options;

namespace JsonToXmlAdapterExample
{
    class Program
    {
        static void Main()
        {
            string json = @"{
                ""name"": ""Maria"",
                ""age"": 20,
                ""isStudent"": true,
                ""extra"": null,
                ""scores"": [10, 9, null, 8]
            }";

            // Configure conversion behavior
            var options = new ConverterOptions
            {
                RootName = "Person",
                ArrayItemName = "Score",
                NullHandling = NullHandling.EmptyElement,
                BooleanHandling = BooleanHandling.Lowercase
            };

            IConverter converter = new JsonToXmlAdapter();

            string xml = converter.Convert(json, options);

            Console.WriteLine("=== XML Result ===");
            Console.WriteLine(xml);
        }
    }
}
