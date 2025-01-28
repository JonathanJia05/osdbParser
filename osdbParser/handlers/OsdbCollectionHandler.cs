using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OsdbReader
{
    //class to read .osdb files
    public class OsdbCollectionHandler
    {
        private static readonly Dictionary<string, int> Versions = new()
        {
            { "o!dm", 1 },   { "o!dm2", 2 },   { "o!dm3", 3 },
            { "o!dm4", 4 },  { "o!dm5", 5 },   { "o!dm6", 6 },
            { "o!dm7", 7 },  { "o!dm8", 8 },
            { "o!dm7min", 1007 },
            { "o!dm8min", 1008 }
        };

        //reads a .osdb file from disk and returns a list of collections
        public List<Collection> ReadOsdb(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fileStream, Encoding.UTF8);

            //read the version string
            string versionString = reader.ReadString();
            if (!Versions.TryGetValue(versionString, out int fileVersion))
                throw new Exception($"Unknown .osdb version: {versionString}");

            //if version >= 7, then the remainder is a GZip stream
            if (fileVersion >= 7)
            {
                using var compressedStream = new MemoryStream();
                fileStream.CopyTo(compressedStream);
                compressedStream.Position = 0;

                //decompress the GZip data
                using var gzip = new GZipStream(compressedStream, CompressionMode.Decompress);
                using var decompressedStream = new MemoryStream();
                gzip.CopyTo(decompressedStream);
                decompressedStream.Position = 0;

                using var decompressedReader = new BinaryReader(decompressedStream, Encoding.UTF8);
                return ReadCollections(decompressedReader, versionString);
            }
            else
            {
                //if version < 7, it has no GZip compression
                return ReadCollections(reader, versionString);
            }
        }

        //parse collections from an uncompressed BinaryReader.
        private List<Collection> ReadCollections(BinaryReader reader, string versionString)
        {
            var collections = new List<Collection>();

            double fileDateValue = reader.ReadDouble();
            DateTime fileDate = DateTime.FromOADate(fileDateValue);

            string lastEditor = reader.ReadString();

            int collectionCount = reader.ReadInt32();
            for (int i = 0; i < collectionCount; i++)
            {
                var collection = new Collection
                {
                    Name = reader.ReadString(),
                    OnlineId = reader.ReadInt32()
                };

                int beatmapCount = reader.ReadInt32();
                for (int j = 0; j < beatmapCount; j++)
                {
                    var beatmap = new Beatmap();

                    beatmap.MapId = reader.ReadInt32();
                    beatmap.MapSetId = reader.ReadInt32();

                    bool isMinimal = versionString.EndsWith("min");
                    if (!isMinimal)
                    {
                        beatmap.ArtistRoman = reader.ReadString();
                        beatmap.TitleRoman = reader.ReadString();
                        beatmap.DiffName = reader.ReadString();
                    }

                    beatmap.Md5 = reader.ReadString();

                    //if the file version >= 4, read user comment
                    if (Versions[versionString] >= 4)
                    {
                        beatmap.UserComment = reader.ReadString();
                    }

                    //ff the nfile version >= 8 or >=5 for full, read play mode
                    if (Versions[versionString] >= 8 ||
                       (Versions[versionString] >= 5 && !isMinimal))
                    {
                        beatmap.PlayMode = reader.ReadByte();
                    }

                    //if the file version >= 8 or >=6 for full, read stars
                    if (Versions[versionString] >= 8 ||
                       (Versions[versionString] >= 6 && !isMinimal))
                    {
                        beatmap.StarsNomod = reader.ReadDouble();
                    }

                    collection.Beatmaps.Add(beatmap);
                }

                //if the file version >= 3, get the number of hash only maps
                if (Versions[versionString] >= 3)
                {
                    int hashCount = reader.ReadInt32();
                    for (int k = 0; k < hashCount; k++)
                    {
                        collection.HashOnlyBeatmaps.Add(reader.ReadString());
                    }
                }

                collections.Add(collection);
            }

            //check if footer is "By Piotrekol"
            string footer = reader.ReadString();
            if (footer != "By Piotrekol")
            {
                throw new Exception("File footer invalid, must be 'By Piotrekol'");
            }

            return collections;
        }
    }
}
