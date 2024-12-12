using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker
{
    public enum TransactionType
    {
        Income,
        Expense
    }

    public class Transaction
    {
        public TransactionType Type { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int Month { get; set; }

        public Transaction(TransactionType type, string description, double amount, int month)
        {
            Type = type;
            Description = description;
            Amount = amount;
            Month = month;
        }

        public override string ToString()
        {
            return $"{Type}: {Description}, Amount: {Amount.ToString("C", new System.Globalization.CultureInfo("sv-SE"))}, Month: {Month}";
        }

        public string WriteToFile()
        {
            return $"{Type},{Description},{Amount},{Month}";
        }

        public static Transaction ReadFromFile(string fileLine)
        {
            var partsFromFile = fileLine.Split(',');
            if (partsFromFile.Length == 4 && Enum.TryParse(partsFromFile[0], out TransactionType type) && double.TryParse(partsFromFile[2], out double amount) && int.TryParse(partsFromFile[3], out int month))
            {
                return new Transaction(type, partsFromFile[1], amount, month);
            }
            return null;
        }
    }
}
