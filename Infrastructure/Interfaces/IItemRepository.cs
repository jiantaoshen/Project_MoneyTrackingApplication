using Project_MoneyTrackingApplication.Domain.Entities;

namespace Project_MoneyTrackingApplication.Infrastructure.Interface
{
    public interface IItemRepository
    {
        void Add(Item item);
        void Remove(Item item);
        List<Item> GetAll();
        void Save();
        void Load();
    }
}
