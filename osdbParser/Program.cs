using System.Text.Json;

namespace OsdbReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = args.Length > 0 ? args[0] : "";
            string outputPath = args.Length > 1 ? args[1] : "";

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("No input file path provided");
                Console.WriteLine("Usage: dotnet run --project osdbParser -- '<input.osdb>' '<output.json>'");
                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = "output.json";
                Console.WriteLine($"No output path provided, default to: {outputPath}");
            }

            try
            {
                var handler = new OsdbCollectionHandler();
                var collections = handler.ReadOsdb(filePath);

                Console.WriteLine($"Parsed {collections.Count} collections:");
                foreach (var collection in collections)
                {
                    Console.WriteLine($"- Collection: {collection.Name}, Beatmaps: {collection.Beatmaps.Count}");
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(collections, jsonOptions);

                File.WriteAllText(outputPath, json);
                Console.WriteLine($"Exported collections to JSON: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading .osdb file: {ex.Message}");
            }
        }
    }
}
