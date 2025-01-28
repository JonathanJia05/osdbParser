namespace OsdbReader
{
    //represents a single beatmap in a .osdb file
    public class Beatmap
    {
        public int MapId { get; set; }
        public int MapSetId { get; set; }

        //if collection is not minimal, these fields are set
        public string ArtistRoman { get; set; }
        public string TitleRoman { get; set; }
        public string DiffName { get; set; }

        public string Md5 { get; set; }
        public string UserComment { get; set; }

        //playmode stored as a single byte
        public byte PlayMode { get; set; }

        //star rating w/o mods (if present in the .osdb file)
        public double StarsNomod { get; set; }
    }
}
