namespace Music.APIs
{
    public sealed record MusicFileMetadataModel
    {
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string[] AlbumArtists { get; set; }
        public string[] Performers { get; set; }
        public string[] Genres { get; set; }
        public uint Year { get; set; }
        public bool HasPictures { get; set; } = false;
        public bool TagsCleared { get; set; } = false;
        public bool TagsCompleted { get; set; } = false;
    }
}
