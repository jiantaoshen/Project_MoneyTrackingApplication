using Project_MoneyTrackingApplication.Application.Interfaces;
using Project_MoneyTrackingApplication.Application.Services;
using Project_MoneyTrackingApplication.Domain.Entities;
using Project_MoneyTrackingApplication.Infrastructure.Interface;
using Project_MoneyTrackingApplication.Infrastructure.Use_Cases;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

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
            total += item.Amount;
        }

        return total;
    }

    private void ShowItemsMenu()
    {
        Console.WriteLine("\nShow Items:");
        Console.WriteLine("(1) Only Expenses");
        Console.WriteLine("(2) Only Incomes");
        Console.WriteLine("(Default) All items");
        Console.Write("Choose an option: ");

        string choice = Console.ReadLine();

        List<Item> items = choice switch
        {
            "1" => _service.FilterByType("expense"),
            "2" => _service.FilterByType("income"),
            _ => _service.GetAllItems(),
        };

        // Sorting submenu
        Console.WriteLine("\nSort items by:");
        Console.WriteLine("(1) Title");
        Console.WriteLine("(2) Amount");
        Console.WriteLine("(Default) Month");
        Console.Write("Choose an option: ");
        string sortChoice = Console.ReadLine();
        
        Console.WriteLine("\n(1) Ascending");
        Console.WriteLine("(Default) Descending");
        Console.Write("Choose an option: ");

        bool ascending = Console.ReadLine() == "1";

        items = sortChoice switch
        {
            "1" => _service.SortItems("title", ascending, items),
            "2" => _service.SortItems("amount", ascending, items),
            _ => _service.SortItems("month", ascending, items),
        };

        Console.WriteLine("\n");
        DisplayItems(items);
        Pause();
    }

    private void AddItem()
    {
        string title = Prompt("Title") ?? "Unknown";
        string amount = Prompt("Amount");
        decimal amountValue = 0.0m;

        if (amount != null && amount != "") decimal.TryParse(amount, out amountValue);

        string month = Prompt("Month");
        byte inputMonth = 0;

        if (month != null && amount != "") byte.TryParse(month, out inputMonth);
        
        string type = string.Empty;
        if (amountValue > 0)
        {
            type = "income";
        }
        else if (amountValue < 0)
        {
            type = "expense";
        }
        else
        {
            Console.WriteLine("Amount cannot be zero. Please enter a valid amount.");
            Pause();
            return;
        }

        _service.AddItem(title, amountValue, inputMonth);
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
        int index = -1;

        if (int.TryParse(Prompt("Enter item number to edit"), out index)) index -= 1;
        else index = -1;

        if (index < 0 || index >= _service.GetAllItems().Count)
        {
            Console.WriteLine("Invalid item number.");
            return;
        }
        else
        {
            string title = Prompt("New title (leave blank to keep)");
            string inputAmount = Prompt("New amount (leave blank to keep)");

            decimal? amount = null;

            if (inputAmount != "")
            {
                if (decimal.TryParse(inputAmount, out decimal amountValue))
                {
                    amount = amountValue;
                    if (amount == 0)
                    {
                        Console.WriteLine("Amount cannot be zero.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid amount.");
                    return;
                }
            }
            
            string inputMonth = Prompt("New month (leave blank to keep)");
            byte? month = null;

            if (inputMonth != "")
            {
                if (byte.TryParse(inputMonth, out byte monthValue))
                {
                    month = monthValue;
                    if (month < 0 || month > 12)
                    {
                        Console.WriteLine("Month must be between 1 and 12.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid month.");
                    return;
                }
            }

            _service.EditItem(index, title, amount, month);
            Console.WriteLine("Item updated successfully!");
        }
    }

    private void RemoveItem()
    {
        string inputIndex = Prompt("Enter item number to remove");
        int index = -1;

        if (inputIndex != "")
        {
            if (int.TryParse(inputIndex, out index)) index = int.Parse(inputIndex) - 1;
            else index = -1;
        }
        

        if(index < 0 || index >= _service.GetAllItems().Count)
        {
            Console.WriteLine("Invalid item number.");
        }
        else
        {
            _service.RemoveItem(index);
            Console.WriteLine("Item removed successfully!");
        }
    }

    private void DisplayItems(List<Item> items)
    {
        Console.WriteLine($"{"Index",-15} {"Title",-15} {"Amount",-15:c} {"Month",-15}");
        Console.WriteLine(new string('-', 65));

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

            Console.WriteLine($"{($"{i + 1}."),-15} {item}");
        }

        Console.ResetColor();
        ShowNetBalance(items);
    }

    private void ShowNetBalance(List<Item> items)
    {
        decimal totalIncome = 0;
        decimal totalExpenses = 0;

        foreach (var item in items)
        {
            if (item.Type == "income") totalIncome += item.Amount;
            else if (item.Type == "expense") totalExpenses += item.Amount;
        }

        decimal net = totalIncome + totalExpenses;

        Console.WriteLine("\nSummary:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Total Income: {totalIncome:C}");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Total Expenses: {totalExpenses:C}");
        Console.ResetColor();
        Console.WriteLine($"Total: {net:C}");
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