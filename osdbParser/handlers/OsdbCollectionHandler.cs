using SharpCompress.Archives;
using SharpCompress.Archives.GZip;

namespace OsdbReader
{
    public class OsdbCollectionHandler
    {
        private readonly Dictionary<string, int> _versions = new()
        {
            {"o!dm", 1},
            {"o!dm2", 2},
            {"o!dm3", 3},
            {"o!dm4", 4},
            {"o!dm5", 5},
            {"o!dm6", 6},
            {"o!dm7", 7},
            {"o!dm8", 8},
            {"o!dm7min", 1007},
            {"o!dm8min", 1008},
        };

        public List<Collection> ReadOsdb(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            using var reader = new BinaryReader(memoryStream);

            //1. Read outer version string
            string outerVersion = reader.ReadString();
            //Console.WriteLine($"Outer version string: {outerVersion}");

            if (!_versions.TryGetValue(outerVersion, out int outerVersionNumber))
            {
                throw new Exception($"Unknown outer version: {outerVersion}");
            }

            //2. Handle GZip if version >= 7
            if (outerVersionNumber >= 7)
            {
                using var archive = GZipArchive.Open(memoryStream);
                var entry = archive.Entries.First();
                using var extractedStream = new MemoryStream();
                entry.WriteTo(extractedStream);
                extractedStream.Position = 0;
                using var innerReader = new BinaryReader(extractedStream);

                return ParseOsdbInner(innerReader);
            }
            else
            {
                return ParseOsdbInner(reader);
            }
        }

        private List<Collection> ParseOsdbInner(BinaryReader reader)
        {
            //3. Read inner version string
            string innerVersion = reader.ReadString();
            //Console.WriteLine($"Inner version string: {innerVersion}");

            if (!_versions.TryGetValue(innerVersion, out int fileVersion))
            {
                throw new Exception($"Unknown inner version: {innerVersion}");
            }

            //4. Read file metadata
            double dateRaw = reader.ReadDouble();
            DateTime fileDate = DateTime.FromOADate(dateRaw);
            //Console.WriteLine($"File date: {fileDate}");

            string editor = reader.ReadString();
            //Console.WriteLine($"Editor: {editor}");

            int numberOfCollections = reader.ReadInt32();
            //Console.WriteLine($"Number of collections: {numberOfCollections}");

            var collections = new List<Collection>();

            for (int i = 0; i < numberOfCollections; i++)
            {
                var collection = new Collection
                {
                    Name = reader.ReadString()
                };

                if (fileVersion >= 7)
                {
                    collection.OnlineId = reader.ReadInt32();
                }

                int numberOfBeatmaps = reader.ReadInt32();
                //Console.WriteLine($"Collection {i + 1}: {collection.Name}, Beatmaps: {numberOfBeatmaps}");

                for (int j = 0; j < numberOfBeatmaps; j++)
                {
                    var beatmap = new Beatmap
                    {
                        MapId = reader.ReadInt32(),
                    };

                    if (fileVersion >= 2)
                    {
                        beatmap.MapSetId = reader.ReadInt32();
                    }

                    if (!innerVersion.EndsWith("min"))
                    {
                        beatmap.ArtistRoman = reader.ReadString();
                        beatmap.TitleRoman = reader.ReadString();
                        beatmap.DiffName = reader.ReadString();
                    }

                    beatmap.Md5 = reader.ReadString();

                    if (fileVersion >= 4)
                    {
                        beatmap.UserComment = reader.ReadString();
                    }

                    if (fileVersion >= 8 || (fileVersion >= 5 && !innerVersion.EndsWith("min")))
                    {
                        beatmap.PlayMode = reader.ReadByte();
                    }

                    if (fileVersion >= 8 || (fileVersion >= 6 && !innerVersion.EndsWith("min")))
                    {
                        beatmap.StarsNomod = reader.ReadDouble();
                    }

                    collection.Beatmaps.Add(beatmap);
                }

                if (fileVersion >= 3)
                {
                    int numberOfHashes = reader.ReadInt32();
                    for (int k = 0; k < numberOfHashes; k++)
                    {
                        collection.HashOnlyBeatmaps.Add(reader.ReadString());
                    }
                }

                collections.Add(collection);
            }

            string footer = reader.ReadString();
            if (footer != "By Piotrekol")
            {
                throw new Exception("Footer missing or invalid.");
            }

            Console.WriteLine("File sucessfully parsed!");
            return collections;
        }
    }
}
