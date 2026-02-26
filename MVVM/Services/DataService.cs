using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MVVM.Models;

namespace MVVM.Services;

public class DataService
{
    private readonly string _filePath = "orders.json";

    public List<Order> LoadOrders()
    {
        if (!File.Exists(_filePath))
            return new List<Order>();

        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
    }

    public void SaveOrders(List<Order> orders)
    {
        string json = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public int GetNextId(List<Order> orders)
    {
        return orders.Any() ? orders.Max(o => o.Id) + 1 : 1;
    }
}