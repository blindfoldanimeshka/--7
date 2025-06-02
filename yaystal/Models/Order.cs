using System;
using System.Collections.Generic;
using System.Linq;

namespace yaystal.Models
{
    /// <summary>
    /// </summary>
    public enum OrderType
    {
        Delivery,
        Pickup
    }

    /// <summary>
    /// </summary>
    public enum OrderStatus
    {
        Created,
        InProgress,
        Ready,
        InDelivery,
        Completed,
        Cancelled
    }

    /// <summary>
    /// </summary>
    public class OrderItem
    {
        public int Id { get; set; }
        public Dish Dish { get; set; }
        public bool IsReady { get; set; }

        public OrderItem()
        {
            IsReady = false;
        }

        public OrderItem(int id, Dish dish)
        {
            Id = id;
            Dish = dish;
            IsReady = false;
        }
    }

    /// <summary>
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public Client Client { get; set; }
        public Address DeliveryAddress { get; set; }
        public List<OrderItem> Items { get; set; }
        public Courier AssignedCourier { get; set; }
        public DateTime? DeliveryStartTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public TimeSpan CookingTimer { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
            CreationTime = DateTime.Now;
            Status = OrderStatus.Created;
            CookingTimer = TimeSpan.Zero;
        }

        public Order(int id, OrderType type, Client client, Address deliveryAddress = null)
        {
            Id = id;
            CreationTime = DateTime.Now;
            Type = type;
            Status = OrderStatus.Created;
            Client = client;
            DeliveryAddress = deliveryAddress ?? client?.Address;
            Items = new List<OrderItem>();
            CookingTimer = TimeSpan.Zero;
        }

        public void AddDish(Dish dish)
        {
            int newId = Items.Count > 0 ? Items.Max(i => i.Id) + 1 : 1;
            Items.Add(new OrderItem(newId, dish));
        }

        public decimal GetTotalPrice()
        {
            decimal total = Items.Sum(item => item.Dish.Price);
            
            if (Type == OrderType.Delivery)
            {
                total += 150;
            }
            
            return total;
        }

        public bool AreAllItemsReady()
        {
            return Items.All(item => item.IsReady);
        }

        public List<OrderItem> GetReadyItems()
        {
            return Items.Where(item => item.IsReady).ToList();
        }

        public List<OrderItem> GetNotReadyItems()
        {
            return Items.Where(item => !item.IsReady).ToList();
        }

        public void MarkItemAsReady(int itemId, TimeSpan elapsedTime)
        {
            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.IsReady = true;
                
                if (AreAllItemsReady())
                {
                    Status = OrderStatus.Ready;
                }
            }
        }

        public void AssignCourier(Courier courier)
        {
            AssignedCourier = courier;
            if (Status == OrderStatus.Ready)
            {
                Status = OrderStatus.InDelivery;
            }
        }

        public void Complete()
        {
            Status = OrderStatus.Completed;
            CompletionTime = DateTime.Now;
        }

        public void Cancel()
        {
            Status = OrderStatus.Cancelled;
        }
    }
}
