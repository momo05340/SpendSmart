using System;
using System.IO;

namespace SpendSmart.Commands
{
    public static class SeederMaker
    {
        public static void Make(string name)
        {
            var folder = Path.Combine("Data", "Seeders");
            var filePath = Path.Combine(folder, $"{name}.cs");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (File.Exists(filePath))
            {
                Console.WriteLine($"❌ Seeder '{name}' already exists.");
                return;
            }

            var content = $@"
using YourProjectName.Data;

public static class {name}
{{
    public static void Seed(AppDbContext context)
    {{
        // TODO: Add seed logic here
    }}
}}";

            File.WriteAllText(filePath, content);
            Console.WriteLine($"✅ Seeder '{name}' created at '{filePath}'.");
        }
    }
}
