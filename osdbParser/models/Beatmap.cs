namespace OsdbReader
{
    public class Beatmap
    {
        public int MapId { get; set; }
        public int MapSetId { get; set; }
        public string ArtistRoman { get; set; } = "";
        public string TitleRoman { get; set; } = "";
        public string DiffName { get; set; } = "";
        public string Md5 { get; set; } = "";
        public string UserComment { get; set; } = "";
        public byte PlayMode { get; set; }
        public double StarsNomod { get; set; }
    }
}
