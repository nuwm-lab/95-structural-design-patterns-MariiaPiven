using System;
using LabWork.Adapters;

namespace LabWork
{
    // Даний проект є шаблоном для виконання лабораторних робіт
    // з курсу "Об'єктно-орієнтоване програмування та патерни проектування"
    // Необхідно змінювати і дописувати код лише в цьому проекті
    // Відео-інструкції щодо роботи з github можна переглянути 
    // за посиланням https://www.youtube.com/@ViktorZhukovskyy/videos 
    class Program
    {
        static void Main(string[] args)
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
            }
        }
    }
}
