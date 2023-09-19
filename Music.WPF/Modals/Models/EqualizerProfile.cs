namespace Music.WPF.Modals.Models
{
    public sealed record EqualizerProfile
    {
        public string Name { get; set; }
        public float Band1 { get; set; }
        public float Band2 { get; set; }
        public float Band3 { get; set; }
        public float Band4 { get; set; }
        public float Band5 { get; set; }
        public float Band6 { get; set; }
        public float Band7 { get; set; }
        public float Band8 { get; set; }
        public float Band9 { get; set; }
        public float Band10 { get; set; }

        public EqualizerProfile(string name, float band1, float band2, float band3, float band4, float band5, float band6, float band7, float band8, float band9, float band10)
        {
            Name = name;
            Band1 = band1;
            Band2 = band2;
            Band3 = band3;
            Band4 = band4;
            Band5 = band5;
            Band6 = band6;
            Band7 = band7;
            Band8 = band8;
            Band9 = band9;
            Band10 = band10;
        }
    }
}
