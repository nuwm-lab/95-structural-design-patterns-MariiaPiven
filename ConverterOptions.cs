using System;

namespace JsonToXmlAdapterExample.Options
{
    public enum NullHandling
    {
        // Keep element but empty value
        EmptyElement,
        // Remove the element altogether
        OmitElement,
        // Write the literal text "null"
        LiteralNull
    }

    public enum BooleanHandling
    {
        // "True"/"False" (default .NET)
        TrueFalse,
        // "true"/"false"
        Lowercase,
        // "1"/"0"
        Numeric
    }

    public class ConverterOptions
    {
        // Name for the root XML element
        public string RootName { get; set; } = "Root";

        // Name for array items
        public string ArrayItemName { get; set; } = "Item";

        // How to treat JSON null values
        public NullHandling NullHandling { get; set; } = NullHandling.EmptyElement;

        // How to format booleans
        public BooleanHandling BooleanHandling { get; set; } = BooleanHandling.TrueFalse;
    }
}
