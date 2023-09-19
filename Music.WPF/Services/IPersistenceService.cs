namespace Music.WPF.Services
{
    public interface IPersistenceService
    {
        public void Parse();
        public void Save();
    }
}