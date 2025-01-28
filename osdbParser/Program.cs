using System;
using System.Collections.Generic;

namespace OsdbReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "/Users/cheekysquid/Documents/code projects/IOS/2025.osdb";

            try
            {
                var handler = new OsdbCollectionHandler();
                var collections = handler.ReadOsdb(filePath);

                Console.WriteLine($"Parsed {collections.Count} collections:");
                foreach (var collection in collections)
                {
                    Console.WriteLine($"- Collection: {collection.Name}, Beatmaps: {collection.Beatmaps.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading .osdb file: {ex.Message}");
            }
        }
    }
}
