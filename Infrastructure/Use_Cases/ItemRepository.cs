using Project_MoneyTrackingApplication.Domain.Entities;
using Project_MoneyTrackingApplication.Infrastructure.Interface;
using System.Text.Json;

namespace Project_MoneyTrackingApplication.Infrastructure.Use_Cases
{
    public class ItemRepository : IItemRepository
    {
        private List<Item> _items = new();
        private readonly string _fileName = "data.json";

        public void Add(Item item) => _items.Add(item);
        public void Remove(Item item) => _items.Remove(item);
        public List<Item> GetAll() => _items;

        public void Save()
        {
            string json = JsonSerializer.Serialize(_items);
            File.WriteAllText(_fileName, json);
        }

        public void Load()
        {
            if (!File.Exists(_fileName)) return;
            string json = File.ReadAllText(_fileName);
            _items = JsonSerializer.Deserialize<List<Item>>(json) ?? new List<Item>();
        }
    }
}
