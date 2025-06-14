using System;
using System.IO;

public static class SeederMaker
{
    public static void Make(string name)
    {
        var folder = "Data/Seeders";
        var filePath = $"{folder}/{name}.cs";

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (File.Exists(filePath))
        {
            Console.WriteLine("Seeder already exists.");
            return;
        }

        string text = $@"
public static class {name}
{{
    public static void Seed(AppDbContext context)
    {{
        // Add your seed data here
    }}
}}";

        File.WriteAllText(filePath, text);
        Console.WriteLine($"Seeder {name} created!");
    }
}
