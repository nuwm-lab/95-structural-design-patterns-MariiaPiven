using System;

namespace JsonToXmlAdapterExample.Contracts
{
    // Target interface for converters
    public interface IConverter
    {
        // Convert input string (JSON) to output string (XML). Optional options configure conversion behavior.
        string Convert(string input, Options.ConverterOptions options = null);
    }
}

