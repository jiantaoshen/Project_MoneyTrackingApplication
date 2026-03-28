namespace Project_MoneyTrackingApplication.Domain.Entities
{
    [Serializable]
    public class Item
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public byte Month { get; set; }
        public string Type { get; set; } // "income" or "expense"

        public Item(string title, decimal amount, byte month, string type)
        {
            Title = title;
            Amount = amount;
            Month = month;
            Type = type;
        }

        public override string ToString() => $"{Title, -15} {Amount, -15:C} {Month, -15}";
    }
}
