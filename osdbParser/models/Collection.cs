using System.Collections.Generic;

namespace OsdbReader
{
    //represents a single collection in a .osdb file
    public class Collection
    {
        public string Name { get; set; }
        public int OnlineId { get; set; }

        //beatmaps with detailed metadata
        public List<Beatmap> Beatmaps { get; } = new List<Beatmap>();

        //beatmaps with only md5 hash
        public List<string> HashOnlyBeatmaps { get; } = new List<string>();

        //number of beatmaps in this collection
        public int NumberOfBeatmaps => Beatmaps.Count + HashOnlyBeatmaps.Count;
    }
}
