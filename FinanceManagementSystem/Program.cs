using System;
using System.Collections.Generic;

// Record for financial transactions
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface for transaction processing
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Concrete processors
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] {transaction.Category} - Amount: {transaction.Amount:C}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] {transaction.Category} - Amount: {transaction.Amount:C}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] {transaction.Category} - Amount: {transaction.Amount:C}");
    }
}

// Base account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// Sealed savings account
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds for this transaction.");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction successful. New balance: {Balance:C}");
        }
    }
}

// Main application
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        // Create account with given example balance
        var account = new SavingsAccount("ACCT-001", 1000m);

        // Create transactions
        var t1 = new Transaction(1, DateTime.Now, 120m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 450m, "Entertainment");

        // Processors
        ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
        ITransactionProcessor bankTransfer = new BankTransferProcessor();
        ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

        // Process and apply
        mobileMoney.Process(t1);
        account.ApplyTransaction(t1);
        _transactions.Add(t1);

        bankTransfer.Process(t2);
        account.ApplyTransaction(t2);
        _transactions.Add(t2);

        cryptoWallet.Process(t3);
        account.ApplyTransaction(t3);
        _transactions.Add(t3);
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
