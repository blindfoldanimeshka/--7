using System;
using System.Collections.Generic;

namespace yaystal.Models
{
    /// <summary>
    /// </summary>
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; } 
        public List<Address> Addresses { get; set; }
        public List<Order> OrderHistory { get; set; }
        public string Email { get; set; }
        public int BonusPoints { get; set; }

        public Client()
        {
            OrderHistory = new List<Order>();
            Addresses = new List<Address>();
        }

        public Client(int id, string name, string phoneNumber, Address address)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            Addresses = new List<Address>();
            if (address != null)
            {
                Addresses.Add(address);
            }
            OrderHistory = new List<Order>();
            Email = ""; //пуста
            BonusPoints = 0;
        }

        public void AddOrder(Order order)
        {
            OrderHistory.Add(order);
        }

        public void AddAddress(Address address)
        {
            if (address != null && !Addresses.Contains(address))
            {
                Addresses.Add(address);

                if (Addresses.Count == 1)
                {
                    address.IsMain = true;
                    Address = address;
                }
                else if (address.IsMain)
                {
                    SetMainAddress(address);
                }
            }
        }


        public void RemoveAddress(Address address)
        {
            if (address != null && Addresses.Contains(address))
            {
                bool wasMain = address.IsMain;
                Addresses.Remove(address);
                
                
                if (wasMain && Addresses.Count > 0)
                {
                    
                    Address newMainAddress = Addresses[0];
                    newMainAddress.IsMain = true;
                    Address = newMainAddress;
                }
                else if (Addresses.Count == 0)
                {
                    
                    Address = null;
                }
            }
        }


        public void SetMainAddress(Address address)
        {
            if (address != null && Addresses.Contains(address))
            {
                
                foreach (var addr in Addresses)
                {
                    addr.IsMain = false;
                }
                
                
                address.IsMain = true;
                
                
                Address = address;
            }
        }
    }
}
