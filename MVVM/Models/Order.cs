using System;

namespace MVVM.Models;

public enum OrderStatus
{
    New,
    Processing,
    Delivering,
    Completed
}

public class Order
{
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string Address { get; set; }
    public string Items { get; set; } 
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public string Courier { get; set; } = "Не назначен";
    public DateTime? DeliveryTime { get; set; }

    public void NextStatus()
    {
        if (Status < OrderStatus.Completed)
            Status++;
    }

    public void PreviousStatus()
    {
        if (Status > OrderStatus.New)
            Status--;
    }

    public string GenerateReceipt()
    {
        return $"Чек №{Id}\nКлиент: {ClientName}\nТелефон: {Courier}\nАдрес: {Address}\nТовары: {Items}\nДата: {OrderDate}\nСтатус: {Status}\nВремя доставки: {(DeliveryTime.HasValue ? DeliveryTime.Value.ToString() : "не назначено")}";
    }
}