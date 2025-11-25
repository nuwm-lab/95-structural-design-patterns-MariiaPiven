using System;

namespace LabWork.Adapters
{
    public enum ItemNamingStrategy
    {
        Item,
        RepeatPropertyName,
        Custom
    }

    public class ConverterOptions
    {
        // How to name elements for array items
        public ItemNamingStrategy ItemNaming { get; set; } = ItemNamingStrategy.RepeatPropertyName;
        // Used when ItemNaming == Custom
        public string CustomItemName { get; set; } = string.Empty;

        // Include an attribute 'type' with the JSON type (string, number, boolean, object, array, null)
        public bool IncludeTypeAttribute { get; set; } = false;

        // Include an attribute 'isNull'="true" for null values
        public bool IncludeNullAttribute { get; set; } = false;
    }
}
