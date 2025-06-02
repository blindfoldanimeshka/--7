using System;
using System.Collections.Generic;
using System.Linq;

namespace yaystal.Models
{
    /// <summary>
    /// </summary>
    public class DataStorage
    {
        private static DataStorage _instance;
        
        public List<Dish> Dishes { get; private set; }
        public List<Client> Clients { get; private set; }
        public List<Employee> Employees { get; private set; }
        public List<Order> Orders { get; private set; }
        public List<Address> Addresses { get; private set; }

        public double RestaurantX { get; private set; }
        public double RestaurantY { get; private set; }

        private int _nextDishId = 1;
        private int _nextClientId = 1;
        private int _nextEmployeeId = 1;
        private int _nextOrderId = 1;
        private int _nextAddressId = 1;
        private int _nextOrderItemId = 1;

        private DataStorage()
        {
            Dishes = new List<Dish>();
            Clients = new List<Client>();
            Employees = new List<Employee>();
            Orders = new List<Order>();
            Addresses = new List<Address>();

//центрую ресторан на мапе
            RestaurantX = 50;
            RestaurantY = 50;

            InitializeTestData();
        }

        public static DataStorage GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataStorage();
            }
            return _instance;
        }

        private void InitializeTestData()
        {
            AddDish(new Dish(_nextDishId++, "Пицца Маргарита", "Тесто, томатный соус, моцарелла, базилик", 
                "Классическая итальянская пицца", 450, 399, "/Images/pizza_margherita.jpg", 15));
            
            AddDish(new Dish(_nextDishId++, "Паста Карбонара", "Спагетти, бекон, яйцо, сыр пармезан", 
                "Традиционная итальянская паста", 350, 349, "/Images/pasta_carbonara.jpg", 12));
            
            AddDish(new Dish(_nextDishId++, "Цезарь с курицей", "Салат романо, куриное филе, гренки, соус цезарь, пармезан", 
                "Популярный салат", 250, 299, "/Images/caesar_salad.jpg", 10));

            AddEmployee(new Administrator(_nextEmployeeId++, "Иван Петров"));
            AddEmployee(new Cook(_nextEmployeeId++, "Мария Иванова"));
            AddEmployee(new Courier(_nextEmployeeId++, "Алексей Сидоров"));

            Address address = new Address(_nextAddressId++, "Ленина", "10", "5");
            AddAddress(address);

            Client client = new Client(_nextClientId++, "Петр Смирнов", "+79001234567", address);
            AddClient(client);

            Order order = new Order(_nextOrderId++, OrderType.Delivery, client, address);
            order.AddDish(Dishes[0]);
            order.Status = OrderStatus.InProgress;
            AddOrder(order);
        }

        public void AddDish(Dish dish)
        {
            if (dish.Id == 0)
            {
                dish.Id = _nextDishId++;
            }
            Dishes.Add(dish);
        }

        public void AddClient(Client client)
        {
            if (client.Id == 0)
            {
                client.Id = _nextClientId++;
            }
            Clients.Add(client);
        }

        public void AddEmployee(Employee employee)
        {
            if (employee.Id == 0)
            {
                employee.Id = _nextEmployeeId++;
            }
            Employees.Add(employee);
        }

        public void AddOrder(Order order)
        {
            if (order.Id == 0)
            {
                order.Id = _nextOrderId++;
            }
            Orders.Add(order);
            if (order.Client != null)
            {
                order.Client.AddOrder(order);
            }
        }

        public void AddAddress(Address address)
        {
            if (address.Id == 0)
            {
                address.Id = _nextAddressId++;
            }
            
            var existingAddress = Addresses.FirstOrDefault(a => 
                a.Street == address.Street && a.HouseNumber == address.HouseNumber);
            
            if (existingAddress != null)
            {
                address.X = existingAddress.X;
                address.Y = existingAddress.Y;
            }
            
            Addresses.Add(address);
        }

        public Dish GetDishById(int id)
        {
            return Dishes.FirstOrDefault(d => d.Id == id);
        }

        public Client GetClientById(int id)
        {
            return Clients.FirstOrDefault(c => c.Id == id);
        }

        public Employee GetEmployeeById(int id)
        {
            return Employees.FirstOrDefault(e => e.Id == id);
        }

        public Order GetOrderById(int id)
        {
            return Orders.FirstOrDefault(o => o.Id == id);
        }

        public Address GetAddressById(int id)
        {
            return Addresses.FirstOrDefault(a => a.Id == id);
        }

        public List<Order> GetActiveOrders()
        {
            return Orders.Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled).ToList();
        }

        public List<Order> GetDeliveryOrders()
        {
            return Orders.Where(o => o.Type == OrderType.Delivery && o.Status == OrderStatus.Ready).ToList();
        }

        public List<Employee> GetEmployeesByType(EmployeeType type)
        {
            return Employees.Where(e => e.Type == type).ToList();
        }
    }
}
