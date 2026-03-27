using Project_MoneyTrackingApplication.Application.Interfaces;
using Project_MoneyTrackingApplication.Domain.Entities;
using Project_MoneyTrackingApplication.Infrastructure.Interface;

namespace Project_MoneyTrackingApplication.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;

        public ItemService(IItemRepository repository)
        {
            _repository = repository;
            _repository.Load();
        }

        public void AddItem(string title, decimal amount, string month, string type)
        {
            _repository.Add(new Item(title, amount, month, type));
        }

        public void EditItem(int index, string title, decimal? amount, string month, string type)
        {
            var items = _repository.GetAll();
            if (index < 0 || index >= items.Count) throw new IndexOutOfRangeException();

            var item = items[index];
            if (!string.IsNullOrEmpty(title)) item.Title = title;
            if (amount.HasValue) item.Amount = amount.Value;
            if (!string.IsNullOrEmpty(month)) item.Month = month;
            if (!string.IsNullOrEmpty(type)) item.Type = type.ToLower();
        }

        public void RemoveItem(int index)
        {
            var items = _repository.GetAll();
            if (index < 0 || index >= items.Count) throw new IndexOutOfRangeException();
            _repository.Remove(items[index]);
        }

        public List<Item> GetAllItems() => _repository.GetAll();

        public List<Item> FilterByType(string type) =>
            _repository.GetAll().Where(i => i.Type == type.ToLower()).ToList();

        public List<Item> SortItems(string sortBy, bool ascending)
        {
            var items = _repository.GetAll();
            return sortBy.ToLower() switch
            {
                "title" => ascending ? items.OrderBy(i => i.Title).ToList() : items.OrderByDescending(i => i.Title).ToList(),
                "amount" => ascending ? items.OrderBy(i => i.Amount).ToList() : items.OrderByDescending(i => i.Amount).ToList(),
                "month" => ascending ? items.OrderBy(i => i.Month).ToList() : items.OrderByDescending(i => i.Month).ToList(),
                _ => items
            };
        }

        public void Save() => _repository.Save();
        public void Load() => _repository.Load();
    }
}
