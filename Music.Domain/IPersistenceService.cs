using Music.Domain.DataModels;

namespace Music.Domain
{
    public interface IPersistenceService
    {
        public Root? Parse();
        public void Save();
    }
}