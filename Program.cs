using System.Collections;
using System.Runtime.CompilerServices;

TransList transList = new TransList();
bool userQuit = false;

printMenu();

while (!userQuit)
{
    string userInput = Console.ReadLine().Trim().ToLower() ?? "None";

    if (byte.TryParse(userInput, out byte userChoice))
    {
        switch (userChoice)
        {
            case 1:
                Console.WriteLine("Choose transaction list -- 1: All, 2: Expense(s), 3: Income(s)");
                byte categoryChoice = 0;
                userInput = Console.ReadLine().Trim().ToLower() ?? "None";
                if (byte.TryParse(userInput, out categoryChoice))
                {
                    switch (categoryChoice)
                    {
                        case 1: categoryChoice = 1; break;
                        case 2: categoryChoice = 2; break;
                        case 3: categoryChoice = 3; break;
                        default: 
                            Console.WriteLine("Invalid input of category. Defaulting to All. "); 
                            categoryChoice = 1; 
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input of category. Defaulting to All. ");
                    categoryChoice = 1;
                }

                Console.WriteLine("Display list order by -- 1: Month, 2: Amount, 3: Title");
                byte orderByChoice = 0;
                userInput = Console.ReadLine().Trim().ToLower() ?? "None";
                if (byte.TryParse(userInput, out orderByChoice))
                {
                    switch (orderByChoice)
                    {
                        case 1: orderByChoice = 1; break;
                        case 2: orderByChoice = 2; break;
                        case 3: orderByChoice = 3; break;
                        default:
                            Console.WriteLine("Invalid input of order by. Defaulting to Month. ");
                            orderByChoice = 1;
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input of order by. Defaulting to Month. ");
                    orderByChoice = 1;
                }

                Console.WriteLine("Display list order -- 1: Ascending 2: Descending");
                byte orderChoice = 0;
                bool orderDescending = false;
                userInput = Console.ReadLine().Trim().ToLower() ?? "None";
                if (byte.TryParse(userInput, out orderChoice))
                {
                    switch (orderChoice)
                    {
                        case 1: orderDescending = false; break;
                        case 2: orderDescending = true; break;
                        default:
                            Console.WriteLine("Invalid input of order. Defaulting to Ascending. ");
                            orderDescending = false;
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input of order. Defaulting to Ascending. ");
                    orderDescending = false;
                }

                transList.Display(categoryChoice, orderByChoice, orderDescending);
                Console.WriteLine("Press enter to return to menu...");
                Console.ReadLine();
                printMenu();
                break;
            case 2:
                transList.AddItem();
                printMenu();
                break;

            case 3:
                transList.EditItem();
                printMenu();
                break;

            case 4:
                userQuit = true;
                break;

            default:
                Console.WriteLine("Invalid input");
                break;
        }

    }
    else Console.WriteLine("Invalid input");
}


//Functions
void printMenu()
{
    Console.Clear();
    Console.WriteLine("Welcome to TrackMoney");
    Console.WriteLine($"You have currently {transList.totalMoney} Kr on your account.");
    Console.WriteLine("Pick an option:");

    Console.WriteLine("(1) Show Items (All/Expense(s)/income(s))");
    Console.WriteLine("(2) Add New Expense/Income)");
    Console.WriteLine("(3) Edit Item (Edit, Remove)");
    Console.WriteLine("(4) Save and Quit");
}


//Classes
public class TransList
{
    //Variables
    public decimal totalMoney = 0.00m;
    private List<Transaction> userList = new List<Transaction>();
    
    //Initial
    public TransList()
    {
    }

    //Methods
    public void AddItem()
    {
        while (true)
        {
            ColoredString("To enter a new transaction - follow the steps | To quit - enter: \"Q\" ", ConsoleColor.Blue);

            ColoredString("To enter a Title: ", ConsoleColor.Yellow);

            string inputTitle = Console.ReadLine() ?? "Unknown";

            if (inputTitle.Trim().ToLower() == "q") break;

            ColoredString("To enter an Amount: ", ConsoleColor.Yellow);
            string? inputAmount = Console.ReadLine();

            ColoredString("Enter a Month: ", ConsoleColor.Yellow);
            string? inputMonth = Console.ReadLine();

            //Error handling of the price
            if (decimal.TryParse(inputAmount, out decimal roundAmount))
            {
                if (byte.TryParse(inputMonth, out byte transMonth))
                {
                    if (transMonth >= 1 && transMonth <= 12)
                    {
                        roundAmount = Math.Round(roundAmount, 2); //Round to 2 decimals
                        Transaction newTrans = new Transaction(inputTitle, roundAmount, transMonth);
                        userList.Add(newTrans);
                    }
                    else Console.WriteLine("Invalid month. Please enter a value between 1 and 12.");
                }
                else Console.WriteLine("Invalid input of Month. Please try again. ");
            }
            else Console.WriteLine("Invalid input of Amount. Please try again. ");
        }

        CalculateTotal();
    }

    public void Display(byte category = 1, byte orderBy = 1, bool orderDescending = false, bool showIndex = false)
    {

        //Category - 1: All, 2: Expense(s), 3: Income(s)
        var sortedList = userList;
        if (category == 2) sortedList = userList.Where(t => t.amount < 0).ToList();
        else if (category == 3) sortedList = userList.Where(t => t.amount >= 0).ToList();
        

        //Order - true: Ascending, false: Descending
        //OrderBy - 1: Month, 2: Amount, 3: Title
        if (orderDescending == false)
        {
            if (orderBy == 1) sortedList = sortedList.OrderBy(t => t.month).ToList();
            else if (orderBy == 2) sortedList = sortedList.OrderBy(t => t.amount).ToList();
            else if (orderBy == 3) sortedList = sortedList.OrderBy(t => t.title).ToList();
            else Console.WriteLine("Invalid input of OrderBy. ");
        }
        else 
        {
            if (orderBy == 1) sortedList = sortedList.OrderByDescending(t => t.month).ToList();
            else if (orderBy == 2) sortedList = sortedList.OrderByDescending(t => t.amount).ToList();
            else if (orderBy == 3) sortedList = sortedList.OrderByDescending(t => t.title).ToList();
            else Console.WriteLine("Invalid input of OrderBy. ");
        }

        //Print list
        Console.WriteLine(new string('-', 20 * 3));

        if (showIndex == false)
        {
            ColoredString("Title".PadRight(20) + "Amount".PadRight(20) + "Month".PadRight(20), ConsoleColor.Green);
            sortedList.ForEach(t => Console.WriteLine(t.title.PadRight(20) + t.amount.ToString().PadRight(20) + t.month.ToString().PadRight(20)));
        }
        else
        {
            int count = 1;

            ColoredString("Index".PadRight(10) + "Title".PadRight(20) + "Amount".PadRight(20) + "Month".PadRight(20), ConsoleColor.Green);

            foreach (var t in sortedList)
            {
                Console.WriteLine(count.ToString().PadRight(10) + t.title.PadRight(20) + t.amount.ToString().PadRight(20) + t.month.ToString().PadRight(20));
                count++;
            }
        }
        Console.WriteLine(new string('-', 20 * 3));
    }


    public void EditItem()
    {
        while (true)
        {
            //Default display, but with index
            Display(1, 1, false, true);

            var sortedList = userList.OrderBy(t => t.month).ToList();

            Console.WriteLine("Edit or remove item in the list-- 1: Edit, 2: Remove, 3: Return to menu");
            string userInput = Console.ReadLine().Trim().ToLower() ?? "None";

            if (int.TryParse(userInput, out int userChoice))
            {
                if (userChoice == 1)
                {
                    ColoredString("Edit - Enter item Index: ", ConsoleColor.Yellow);
                    userInput = Console.ReadLine().Trim().ToLower() ?? "None";
                    if (int.TryParse(userInput, out userChoice))
                    {
                        if (userChoice - 1 < sortedList.Count)
                        {
                            Transaction t = sortedList[userChoice - 1];
                            Console.WriteLine(userInput.ToString().PadRight(10) + t.title.PadRight(20) + t.amount.ToString().PadRight(20) + t.month.ToString().PadRight(20));

                            ColoredString("To enter a Title: ", ConsoleColor.Yellow);
                            string inputTitle = Console.ReadLine() ?? "Unknown";

                            ColoredString("To enter an Amount: ", ConsoleColor.Yellow);
                            string? inputAmount = Console.ReadLine();

                            ColoredString("Enter a Month: ", ConsoleColor.Yellow);
                            string? inputMonth = Console.ReadLine();

                            //Error handling of the price
                            if (decimal.TryParse(inputAmount, out decimal roundAmount))
                            {
                                if (byte.TryParse(inputMonth, out byte transMonth))
                                {
                                    if (transMonth >= 1 && transMonth <= 12)
                                    {
                                        roundAmount = Math.Round(roundAmount, 2); //Round to 2 decimals
                                        sortedList[userChoice - 1] = new Transaction(inputTitle, roundAmount, transMonth);
                                        userList = sortedList; //Update userList after removing
                                        CalculateTotal(); //Recalculate total after edit
                                    }
                                    else Console.WriteLine("Invalid month. Please enter a value between 1 and 12.");
                                }
                                else Console.WriteLine("Invalid input of Month. Please try again. ");
                            }
                            else Console.WriteLine("Invalid input of Amount. Please try again. ");
                        }
                        else ColoredString("Invalid index. Please try again. ", ConsoleColor.Red);
                    }
                    else ColoredString("Invalid index. Please try again. ", ConsoleColor.Red);
                }
                else if (userChoice == 2)
                {
                    ColoredString("Remove - Enter item Index: ", ConsoleColor.Yellow);
                    userInput = Console.ReadLine().Trim().ToLower() ?? "None";
                    if (int.TryParse(userInput, out userChoice))
                    {
                        if (userChoice - 1 < sortedList.Count)
                        {
                            sortedList.RemoveAt(userChoice - 1);
                            userList = sortedList; //Update userList after removing
                            CalculateTotal(); //Recalculate total after edit
                        }
                        else ColoredString("Invalid index. Please try again. ", ConsoleColor.Red);
                    }
                    else ColoredString("Invalid index. Please try again. ", ConsoleColor.Red);
                }
                else if (userChoice == 3) break;
                else ColoredString("Invalid input. Please try again. ", ConsoleColor.Red);
            }
        }
    }

    private void ColoredString(string text, ConsoleColor consoleColor)
    {
        Console.ForegroundColor = consoleColor;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    //Calculate total amount
    private void CalculateTotal()
    {
        totalMoney = userList.Sum(t => t.amount);
    }

}

public class Transaction
{
    //Variables
    public string title { get; set; }
    public decimal amount { get; set; }
    public byte month { get; set; }

    //Initial
    public Transaction(string title, decimal amount, byte month)
    {
        this.title = title;
        this.amount = amount;
        this.month = month;
    }
}
