using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

public class CsvRecord
{
    [Name("authors")]
    public string Authors { get; set; }

    [Name("first usage countries")]
    public string FirstUsageCountries { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.Write("Укажите путь к файлу:\n");
            string filePath = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Выберите действие:\n1) Вывести на экран\n2) Cохранить в txt\n");
            int flag = 0;
            //string filePath = """C:\Users\igusa\Downloads\data-20230901-structure-20180828.csv""";
            Dictionary<string, HashSet<string>> authorsByCountry = ExtractAuthorsByCountry(filePath);
            var t = Console.ReadKey();
            if (t.Key == ConsoleKey.D1)
            {
                flag = 1;
            }
            else if (t.Key == ConsoleKey.D2)
            {
                flag = 2;
            }
            Console.Clear();
            switch (flag)
            {
                case 1:
                    foreach (var kvp in authorsByCountry)
                    {
                        Console.WriteLine($"Страна: {kvp.Key}");
                        foreach (var author in kvp.Value)
                        {
                            Console.WriteLine($"Автор: {author}");
                        }
                        Console.WriteLine();
                    }
                    break;
                case 2:
                    string userDownloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Загрузки";

                    if (!Directory.Exists(userDownloadsFolder))
                    {
                        Directory.CreateDirectory(userDownloadsFolder);
                    }
                    string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Загрузки", "результат.txt");
                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        foreach (var kvp in authorsByCountry)
                        {
                            foreach (var author in kvp.Value)
                            {
                                string line = $"{author}; {kvp.Key}";
                                writer.WriteLine(line);
                            }
                        }
                    }
                    Console.WriteLine($"Результат сохранен в файле '{outputPath}'.");
                    break;
                default:
                    Console.WriteLine("Выбран не правильный параметр");
                    break;
            }
        } catch (Exception ex) 
        { 
            Console.WriteLine(ex.Message); 
        }
     }
    static Dictionary<string, HashSet<string>> ExtractAuthorsByCountry(string filePath)
    {
        List<CsvRecord> records;
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            records = csv.GetRecords<CsvRecord>().ToList();
        }
        Dictionary<string, HashSet<string>> authorsByCountry = new Dictionary<string, HashSet<string>>();
        foreach (var record in records)
        {
            string[] authors = record.Authors.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string[] countries = record.FirstUsageCountries.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < authors.Length; i++)
            {
                string author = authors[i].Trim();
                string country = countries.Length > i ? countries[i].Trim() : "Неизвестно";
                if (!authorsByCountry.ContainsKey(country))
                {
                    authorsByCountry[country] = new HashSet<string>();
                }
                authorsByCountry[country].Add(author);
            }
        }
        return authorsByCountry;
    }
}