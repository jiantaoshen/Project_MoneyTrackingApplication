using Project_MoneyTrackingApplication.Application.Interfaces;
using Project_MoneyTrackingApplication.Application.Services;
using Project_MoneyTrackingApplication.Domain.Entities;
using Project_MoneyTrackingApplication.Infrastructure.Interface;
using Project_MoneyTrackingApplication.Infrastructure.Use_Cases;

class Program
{
    static void Main()
    {
        IItemRepository repository = new ItemRepository();
        IItemService service = new ItemService(repository);
        ConsoleUI ui = new ConsoleUI(service);

        ui.Run();
    }
}

public class ConsoleUI
{
    private readonly IItemService _service;

    public ConsoleUI(IItemService service)
    {
        _service = service;
    }

    public void Run()
    {
        while (true)
        {
            PrintMenu();
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        ShowItemsMenu();
                        break;
                    case "2":
                        AddItem();
                        break;
                    case "3":
                        EditRemoveItemMenu();
                        break;
                    case "4":
                        _service.Save();
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        Pause();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Pause();
            }
        }
    }

    private void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome to TrackMoney");

        decimal totalMoney = CalculateTotalMoney();
        Console.WriteLine($"You have currently {totalMoney:C} on your account.\n");

        Console.WriteLine("Pick an option:");
        Console.WriteLine("(1) Show Items (All/Expense(s)/Income(s))");
        Console.WriteLine("(2) Add New Expense/Income");
        Console.WriteLine("(3) Edit Item (Edit, Remove)");
        Console.WriteLine("(4) Save and Quit");
    }

    private decimal CalculateTotalMoney()
    {
        decimal total = 0;
        foreach (var item in _service.GetAllItems())
        {
            total += item.Type == "income" ? item.Amount : -item.Amount;
        }
        return total;
    }

    private void ShowItemsMenu()
    {
        Console.WriteLine("\nShow Items:");
        Console.WriteLine("(1) All items");
        Console.WriteLine("(2) Only Expenses");
        Console.WriteLine("(3) Only Incomes");
        Console.Write("Choose an option: ");
        string choice = Console.ReadLine();

        System.Collections.Generic.List<Item> items = choice switch
        {
            "1" => _service.GetAllItems(),
            "2" => _service.FilterByType("expense"),
            "3" => _service.FilterByType("income"),
            _ => null
        };

        if (items == null)
        {
            Console.WriteLine("Invalid choice!");
            Pause();
            return;
        }

        // Sorting submenu
        Console.WriteLine("\nSort items by:");
        Console.WriteLine("(1) Title");
        Console.WriteLine("(2) Amount");
        Console.WriteLine("(3) Month");
        Console.WriteLine("(4) No sorting");
        Console.Write("Choose an option: ");
        string sortChoice = Console.ReadLine();

        bool ascending = true;
        if (sortChoice != "4")
        {
            Console.Write("Ascending or Descending? (A/D): ");
            string order = Console.ReadLine();
            ascending = order.ToUpper() == "A";
        }

        items = sortChoice switch
        {
            "1" => _service.SortItems("title", ascending),
            "2" => _service.SortItems("amount", ascending),
            "3" => _service.SortItems("month", ascending),
            _ => items
        };

        DisplayItems(items);
        Pause();
    }

    private void AddItem()
    {
        string title = Prompt("Title");
        decimal amount = decimal.Parse(Prompt("Amount"));
        string month = Prompt("Month");
        string type = Prompt("Type (income/expense)");
        _service.AddItem(title, amount, month, type);
        Console.WriteLine("Item added successfully!");
        Pause();
    }

    private void EditRemoveItemMenu()
    {
        DisplayItems(_service.GetAllItems());
        Console.WriteLine("\nChoose an action:");
        Console.WriteLine("(1) Edit an item");
        Console.WriteLine("(2) Remove an item");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                EditItem();
                break;
            case "2":
                RemoveItem();
                break;
            default:
                Console.WriteLine("Invalid choice!");
                break;
        }

        Pause();
    }

    private void EditItem()
    {
        int index = int.Parse(Prompt("Enter item number to edit")) - 1;
        string title = Prompt("New title (leave blank to keep)");
        string amt = Prompt("New amount (leave blank to keep)");
        decimal? amount = string.IsNullOrEmpty(amt) ? null : decimal.Parse(amt);
        string month = Prompt("New month (leave blank to keep)");
        string type = Prompt("New type (income/expense, leave blank to keep)");

        _service.EditItem(index, title, amount, month, type);
        Console.WriteLine("Item updated successfully!");
    }

    private void RemoveItem()
    {
        int index = int.Parse(Prompt("Enter item number to remove")) - 1;
        _service.RemoveItem(index);
        Console.WriteLine("Item removed successfully!");
    }

    private void DisplayItems(System.Collections.Generic.List<Item> items)
    {
        if (items.Count == 0)
        {
            Console.WriteLine("No items found.");
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item.Type == "income")
                Console.ForegroundColor = ConsoleColor.Green;
            else if (item.Type == "expense")
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ResetColor();

            Console.WriteLine($"{i + 1}. {item}");
        }

        Console.ResetColor();
        ShowNetBalance(items);
    }

    private void ShowNetBalance(System.Collections.Generic.List<Item> items)
    {
        decimal totalIncome = 0;
        decimal totalExpenses = 0;

        foreach (var item in items)
        {
            if (item.Type == "income") totalIncome += item.Amount;
            else if (item.Type == "expense") totalExpenses += item.Amount;
        }

        decimal net = totalIncome - totalExpenses;

        Console.WriteLine("\nSummary:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Total Income: {totalIncome:C}");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Total Expenses: {totalExpenses:C}");
        Console.ResetColor();
        Console.WriteLine($"Net Balance: {net:C}");
    }

    private string Prompt(string message)
    {
        Console.Write($"{message}: ");
        return Console.ReadLine();
    }

    private void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}