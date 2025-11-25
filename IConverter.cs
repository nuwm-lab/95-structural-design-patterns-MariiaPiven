using System;

namespace LabWork.Adapters
{
    public interface IConverter
    {
        // Конвертує вхідні дані у потрібний формат і повертає рядок результату
        string Convert(string input);
    }
}
