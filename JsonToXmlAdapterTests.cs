using System;
using System.Xml.Linq;
using LabWork.Adapters;
using Xunit;

namespace LabWork.Tests
{
    public class JsonToXmlAdapterTests
    {
        [Fact]
        public void Convert_PrimitiveNumber_ReturnsNumberAsValue()
        {
            var json = "{\"num\":123}";
            var adapter = new JsonToXmlAdapter();

            var xml = adapter.Convert(json);
            var doc = XDocument.Parse(xml);

            var numEl = doc.Root.Element("num");
            Assert.NotNull(numEl);
            Assert.Equal("123", numEl.Value);
        }

        [Fact]
        public void Convert_NullValue_ReturnsEmptyElement()
        {
            var json = "{\"value\":null}";
            var adapter = new JsonToXmlAdapter();

            var xml = adapter.Convert(json);
            var doc = XDocument.Parse(xml);

            var el = doc.Root.Element("value");
            Assert.NotNull(el);
            Assert.Equal(string.Empty, el.Value);
        }

        [Fact]
        public void Convert_Array_ReturnsItems()
        {
            var json = "{\"phones\":[\"a\",\"b\"]}";
            var adapter = new JsonToXmlAdapter();

            var xml = adapter.Convert(json);
            var doc = XDocument.Parse(xml);

            var phones = doc.Root.Element("phones");
            Assert.NotNull(phones);
            var items = phones.Elements("Item");
            Assert.Equal(2, System.Linq.Enumerable.Count(items));
        }

        [Fact]
        public void Convert_NestedObjects_PreservesStructure()
        {
            var json = "{\"person\":{\"name\":\"A\",\"address\":{\"city\":\"X\"}}}";
            var adapter = new JsonToXmlAdapter();

            var xml = adapter.Convert(json);
            var doc = XDocument.Parse(xml);

            var person = doc.Root.Element("person");
            Assert.NotNull(person);
            Assert.Equal("A", person.Element("name").Value);
            Assert.Equal("X", person.Element("address").Element("city").Value);
        }

        [Fact]
        public void Convert_InvalidJson_ThrowsArgumentException()
        {
            var json = "{ invalid json";
            var adapter = new JsonToXmlAdapter();

            Assert.Throws<ArgumentException>(() => adapter.Convert(json));
        }
    }
}
