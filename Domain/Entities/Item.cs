namespace Project_MoneyTrackingApplication.Domain.Entities
{
    [Serializable]
    public class Item
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Month { get; set; }
        public string Type { get; set; } // "income" or "expense"

        public Item(string title, decimal amount, string month, string type)
        {
            Title = title;
            Amount = amount;
            Month = month;
            Type = type.ToLower();
        }

        public override string ToString() => $"{Title} | {Amount:C} | {Month} | {Type}";
    }
}
