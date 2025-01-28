namespace OsdbReader
{
    public class Collection
    {
        public string Name { get; set; } = "";
        public int OnlineId { get; set; }
        public List<Beatmap> Beatmaps { get; set; } = new();
        public List<string> HashOnlyBeatmaps { get; set; } = new();
    }
}
