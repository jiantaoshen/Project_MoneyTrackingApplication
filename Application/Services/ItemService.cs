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

        public void AddItem(string title, decimal amount, byte month)
        {
            string type;

            if (amount > 0) type = "income";
            else if (amount < 0) type = "expense";
            else throw new Exception("Amount cannot be zero.");

            _repository.Add(new Item(title, amount, month, type));
        }

        public void EditItem(int index, string title, decimal? amount, byte? month)
        {
            var items = _repository.GetAll();
            if (index < 0 || index >= items.Count) throw new IndexOutOfRangeException();

            var item = items[index];
            if (!string.IsNullOrEmpty(title)) item.Title = title;
            if (amount.HasValue) item.Amount = amount.Value;
            if (month.HasValue) item.Month = month.Value;
          
            if (amount > 0) item.Type = "income";
            else if (amount < 0) item.Type = "expense";
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

        public List<Item> SortItems(string sortBy, bool ascending, List<Item>? items = null)
        {
            return sortBy.ToLower() switch
            {
                "title" => ascending ? items.OrderBy(i => i.Title).ToList() : items.OrderByDescending(i => i.Title).ToList(),
                "amount" => ascending ? items.OrderBy(i => i.Amount).ToList() : items.OrderByDescending(i => i.Amount).ToList(),
                "month" => ascending ? items.OrderBy(i => i.Month).ToList() : items.OrderByDescending(i => i.Month).ToList(),
                _ => items.ToList()
            };
        }

        public void Save() => _repository.Save();
        public void Load() => _repository.Load();
    }
}
