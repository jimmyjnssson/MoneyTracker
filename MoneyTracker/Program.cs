using System.Globalization;

namespace MoneyTracker
{
    public class Program
    {
        private static List<Transaction> transactions = new List<Transaction>();
        private static string filePath = "transactions.txt";
        private static CultureInfo sweCurrency = new CultureInfo("sv-SE");

        public static void Main(string[] args)
        {
            // Load existing data from the file if the file exists
            LoadDataFromFile();

            while (true)
            {


                //Menu
                Console.Clear();
                DisplayAccountBalance();
                
                Console.WriteLine("1. Add Income");
                Console.WriteLine("2. Add Expense");
                Console.WriteLine("3. View Incomes");
                Console.WriteLine("4. View Expenses");
                Console.WriteLine("5. View All Transactions");
                Console.WriteLine("6. Edit Transaction");
                Console.WriteLine("7. Remove Transaction");
                Console.WriteLine("8. Delete Data File");
                Console.WriteLine("9. Save & Exit");
                Console.Write("Choose an option (1-9): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTransaction(TransactionType.Income);
                        break;
                    case "2":
                        AddTransaction(TransactionType.Expense);
                        break;
                    case "3":
                        ViewTransactions(TransactionType.Income);
                        break;
                    case "4":
                        ViewTransactions(TransactionType.Expense);
                        break;
                    case "5":
                        ViewAllTransactions();
                        break;
                    case "6":
                        EditTransaction();
                        break;
                    case "7":
                        RemoveTransaction();
                        break;
                    case "8":
                        DeleteFile();
                        break;
                    case "9":
                        return; // Exit the program without saving since saving happens on input
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private static void DisplayAccountBalance()
        {
            double balance = 0;
            foreach (var transaction in transactions)
            {
                if (transaction.Type == TransactionType.Income)
                {
                    balance += transaction.Amount;
                }
                else if (transaction.Type == TransactionType.Expense)
                {
                    balance -= transaction.Amount;
                }
            }
            Console.Write("Current Total Account Balance: ");
            Console.ForegroundColor = ConsoleColor.Green; 
            Console.WriteLine($"{balance.ToString("C", sweCurrency)}\n");
            Console.ResetColor(); 
        }

        private static void AddTransaction(TransactionType type)
        {
            Console.Write("Enter description to Expense/Income: ");
            string description = Console.ReadLine();
            Console.Write("Enter an amount: ");
            double amount;
            if (double.TryParse(Console.ReadLine(), out amount))
            {
                Console.Write("Enter month - between 1-12: ");
                int month;
                while (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12)
                {
                    Console.Write("Invalid month. Please enter a number between 1 and 12: ");
                }

                var transaction = new Transaction(type, description, amount, month); //Create transaction object
                transactions.Add(transaction); //Add to the list
                SaveDataToFile(); // Save data to file immediately after adding the transaction to the list
                Console.WriteLine($"{type} added successfully and data saved to file");
            }
            else
            {
                Console.WriteLine("Invalid amount entered. Try again");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void ViewTransactions(TransactionType type)
        {
            Console.WriteLine($"\n{type}s:");
            var sortedTransactions = transactions.Where(t => t.Type == type).OrderBy(t => t.Month).ToList();
            if (sortedTransactions.Count == 0)
            {
                Console.WriteLine($"No {type.ToString().ToLower()} data is available.");
            }
            else
            {
                PrintTableHeader();
                foreach (var transaction in sortedTransactions)
                {
                    PrintTransactionWithColor(transaction);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void ViewAllTransactions()
        {
            Console.WriteLine("\nAll Transactions:");
            var sortedTransactions = transactions.OrderBy(t => t.Month).ToList();
            if (sortedTransactions.Count == 0)
            {
                Console.WriteLine("No transactions available.");
            }
            else
            {
                PrintTableHeader();
                foreach (var transaction in sortedTransactions)
                {
                    PrintTransactionWithColor(transaction);
                }
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        //Prints the header for table
        private static void PrintTableHeader()
        {
            Console.WriteLine(
                $"{"Type".PadRight(10)}{"Description".PadRight(30)}{"Amount".PadLeft(15)}{"Month".PadLeft(10)}");
            Console.WriteLine(new string('-', 70)); // Create a line separator for header
        }

        //Method to print the transaction to look like a DB table and add color on the Enum type
        private static void PrintTransactionWithColor(Transaction transaction)
        {
            if (transaction.Type == TransactionType.Income)
            {
                Console.ForegroundColor = ConsoleColor.Green; // Set text color to green for income
            }
            else if (transaction.Type == TransactionType.Expense)
            {
                Console.ForegroundColor = ConsoleColor.Red; // Set text color to red for expense
            }

            // Print the transaction padded as a DB table
            Console.WriteLine(
                $"{transaction.Type.ToString().PadRight(10)}" +
                $"{transaction.Description.PadRight(30)}" +
                $"{transaction.Amount.ToString("C", sweCurrency).PadLeft(15)}" +
                $"{transaction.Month.ToString().PadLeft(10)}");


            Console.ResetColor();
        }

        private static void EditTransaction()
        {
            Console.WriteLine("\nSelect a transaction to edit by transaction index:");

            //Loops through the list and displays the transactions to be changed
            for (int i = 0; i < transactions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {transactions[i]}");
            }

            Console.Write("Enter the index of the transaction to edit: ");
            int index;
            if (int.TryParse(Console.ReadLine(), out index) && index > 0 && index <= transactions.Count)
            {
                var transaction = transactions[index - 1];

                Console.Write("Enter a new description (press Enter to keep the current value): ");
                string newDescription = Console.ReadLine();
                if (!string.IsNullOrEmpty(newDescription))
                {
                    transaction.Description = newDescription;
                }


                Console.Write("Enter a new amount (press Enter to keep the current value): ");
                double newAmount;
                if (double.TryParse(Console.ReadLine(), out newAmount))
                {
                    transaction.Amount = newAmount;
                }

                Console.Write("Enter a new month - between 1-12 (press Enter to keep the current value): ");
                int newMonth;
                if (int.TryParse(Console.ReadLine(), out newMonth) && newMonth >= 1 && newMonth <= 12)
                {
                    transaction.Month = newMonth;
                }

                SaveDataToFile(); // Save data to file after the edit
                Console.WriteLine("Transaction updated and data saved to file");
            }
            else
            {
                Console.WriteLine("Invalid index. No transaction found to edit.");
            }

            Console.WriteLine("Press any key to continue your session");
            Console.ReadKey();
        }

        private static void RemoveTransaction()
        {
            Console.WriteLine("\nSelect a transaction to remove by index:");
            for (int i = 0; i < transactions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {transactions[i]}");
            }

            Console.Write("Enter the index of the transaction to remove: ");
            int index;
            if (int.TryParse(Console.ReadLine(), out index) && index > 0 && index <= transactions.Count)
            {
                transactions.RemoveAt(index - 1);
                SaveDataToFile(); // Save data to file after removal
                Console.WriteLine("Transaction removed and data saved!");
            }
            else
            {
                Console.WriteLine("Invalid index. No transaction found to remove.");
            }

            Console.WriteLine("Press any key to continue to continue your session");
            Console.ReadKey();
        }

        private static void SaveDataToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    foreach (var transaction in transactions)
                    {
                        writer.WriteLine(transaction.WriteToFile());
                    }
                }
                Console.WriteLine("Data saved successfully!");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving data to file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access to file denied: {ex.Message}");
            }
        }

        private static void LoadDataFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string[] fileLines = File.ReadAllLines(filePath);
                    foreach (var fileLine in fileLines)
                    {
                        var transaction = Transaction.ReadFromFile(fileLine);
                        if (transaction != null)
                        {
                            transactions.Add(transaction);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading data from file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access to file denied: {ex.Message}");
            }
        }

        private static void DeleteFile()
        {
            Console.WriteLine("Are you sure you want to delete the data file?");
            Console.Write("Type 'yes' to confirm: ");
            string confirmation = Console.ReadLine().ToLower();

            if (confirmation == "yes")
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        transactions.Clear(); // Clear the transactions list as the file has been deleted and we don't want the values in our list anymore
                        Console.WriteLine("File deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("No file found to delete.");
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error deleting the file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("File deletion canceled.");
            }

            Console.WriteLine("Press any key to continue continue your session");
            Console.ReadKey();
        }
    }
}
