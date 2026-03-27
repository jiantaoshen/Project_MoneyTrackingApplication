using Project_MoneyTrackingApplication.Domain.Entities;

namespace Project_MoneyTrackingApplication.Application.Interfaces
{
    public interface IItemService
    {
        void AddItem(string title, decimal amount, string month, string type);
        void EditItem(int index, string title, decimal? amount, string month, string type);
        void RemoveItem(int index);
        List<Item> GetAllItems();
        List<Item> FilterByType(string type);
        List<Item> SortItems(string sortBy, bool ascending);
        void Save();
        void Load();
    }
}
