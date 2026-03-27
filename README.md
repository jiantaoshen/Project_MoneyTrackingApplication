# TrackMoney – Console Money Tracking Application
## Overview

TrackMoney is a simple console-based money tracking application built in C#. It allows users to manage their personal finances by recording income and expenses, tracking them by month, and saving data for future use.

The application uses a layered architecture with SOLID and DRY principles to ensure maintainability and clarity.

## Features
Add income or expense items with title, amount, and month
Edit or remove existing items
Display items: all, only incomes, or only expenses
Sort items by title, amount, or month (ascending/descending)
Color-coded display:
Green for incomes
Red for expenses
Show net balance and summary of total income/expenses
Persist data to data.json in the Infrastructure folder
Load saved data on program start

## Project Structure
TrackMoney/
│
├─ Domain/
│   └─ Item.cs             # Represents a financial item (income/expense)
│
├─ Infrastructure/
│   ├─ ItemRepository.cs   # Handles file storage (data.json)
│
├─ Application/
│   └─ ItemService.cs      # Business logic (add/edit/remove/filter/sort)
│
└─ Program.cs              # Entry point and user interface

data.json is automatically created inside the bin file when you debug the program with Visual Studio.

## Usage
Open the project in Visual Studio or your preferred C# IDE
Build the solution
Run the application

## Example Menu Interaction:

Welcome to TrackMoney
You have currently 0.00 on your account.

Pick an option:
(1) Show Items (All/Expense(s)/Income(s))
(2) Add New Expense/Income
(3) Edit Item (Edit, Remove)
(4) Save and Quit
Adding a New Item
Select option (2)
Enter:
Title: e.g., Salary
Amount: e.g., 5000
Month: e.g., March
Type: income or expense
Editing/Removing an Item
Select option (3)
Choose to edit or remove
Follow prompts to update fields or delete the item
Saving and Quitting
Select option (4)
All changes are saved to Infrastructure/data.json
Notes
Amount must always be positive; type determines if it’s added or subtracted from balance
The application is single-user and works entirely on local files
Sorting and filtering help organize your items quickly

## Future Enhancements
Error handling for invalid inputs
Month validation (e.g., only allow valid month names) OR use a numeric month representation
Income is only valid if the amount is positive, and expense is only valid if the amount is negative. This can be enforced in the UI when adding/editing items.
Security improvements (e.g., encrypting data.json)