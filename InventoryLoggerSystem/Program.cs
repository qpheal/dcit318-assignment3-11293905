using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable record for inventory items
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic inventory logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item) => _log.Add(item);

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
            Console.WriteLine($"✅ Data saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("⚠ No saved file found.");
                return;
            }

            string json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            if (items != null) _log = items;
            Console.WriteLine($"✅ Data loaded from {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading file: {ex.Message}");
        }
    }
}

// Inventory App
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Rice Bag", 30, DateTime.Now.AddDays(-2)));
        _logger.Add(new InventoryItem(3, "Desk Chair", 10, DateTime.Now.AddDays(-7)));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"{item.Name} (ID: {item.Id}, Qty: {item.Quantity}, Added: {item.DateAdded:dd-MMM-yyyy})");
        }
    }
}

class Program
{
    static void Main()
    {
        string filePath = "inventory.json";
        var app = new InventoryApp(filePath);

        // First run: seed and save
        app.SeedSampleData();
        app.SaveData();

        Console.WriteLine("\n--- Simulating New Session ---\n");

        // Clear and reload
        var newApp = new InventoryApp(filePath);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}

