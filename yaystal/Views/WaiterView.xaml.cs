using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для WaiterView.xaml
    /// </summary>
    public partial class WaiterView : Page
    {
        private DataStorage _dataStorage;
        private ObservableCollection<OrderItem> _orderItems;
        private ObservableCollection<Order> _currentOrders;

        public WaiterView()
        {
            InitializeComponent();
            
            // Инициализация данных
            _dataStorage = DataStorage.GetInstance();
            _orderItems = new ObservableCollection<OrderItem>();
            _currentOrders = new ObservableCollection<Order>();
            
            // Привязка данных к элементам управления
            lvDishes.ItemsSource = _dataStorage.Dishes;
            lvOrderItems.ItemsSource = _orderItems;
            
            // Загрузка текущих заказов
            LoadCurrentOrders();
            
            // Запуск таймера для обновления заказов
            StartOrdersUpdateTimer();
        }

        // Загрузка текущих заказов
        private void LoadCurrentOrders()
        {
            _currentOrders.Clear();
            
            // Фильтрация заказов, где официант - текущий пользователь и статус не "Завершен" или "Отменен"
            foreach (var order in _dataStorage.Orders)
            {
                if (order.Status != OrderStatus.Completed && order.Status != OrderStatus.Cancelled)
                {
                    _currentOrders.Add(order);
                }
            }
            
            lvOrders.ItemsSource = _currentOrders;
        }

        // Запуск таймера для обновления заказов
        private void StartOrdersUpdateTimer()
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (sender, e) => LoadCurrentOrders();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();
        }

        // Обновление общей стоимости заказа
        private void UpdateTotalPrice()
        {
            decimal totalPrice = _orderItems.Sum(item => item.Dish.Price);
            txtTotalPrice.Text = $"{totalPrice} ₽";
        }

        // Обработчики событий
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                lvDishes.ItemsSource = _dataStorage.Dishes;
            }
            else
            {
                lvDishes.ItemsSource = _dataStorage.Dishes.Where(d => 
                    d.Name.ToLower().Contains(searchText) || 
                    d.Description.ToLower().Contains(searchText));
            }
        }

        private void lvDishes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Dish selectedDish = lvDishes.SelectedItem as Dish;
            if (selectedDish != null)
            {
                OrderItem newItem = new OrderItem(_orderItems.Count + 1, selectedDish);
                _orderItems.Add(newItem);
                UpdateTotalPrice();
            }
        }

        private void btnClearCart_Click(object sender, RoutedEventArgs e)
        {
            _orderItems.Clear();
            UpdateTotalPrice();
        }

        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста. Добавьте блюда перед оформлением заказа.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Создание нового заказа
                int newOrderId = _dataStorage.Orders.Count + 1;
                Client client = _dataStorage.Clients.FirstOrDefault(); // Для примера берем первого клиента
                
                if (client == null)
                {
                    MessageBox.Show("Не найден клиент для оформления заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Используем Pickup вместо DineIn, так как в модели нет значения DineIn
                Order newOrder = new Order(newOrderId, OrderType.Pickup, client);
                
                // Добавление блюд в заказ
                foreach (OrderItem item in _orderItems)
                {
                    newOrder.AddDish(item.Dish);
                }
                
                // Сохранение заказа
                _dataStorage.AddOrder(newOrder);
                
                // Очистка корзины
                _orderItems.Clear();
                UpdateTotalPrice();
                
                // Обновление списка заказов
                LoadCurrentOrders();
                
                MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Order selectedOrder = lvOrders.SelectedItem as Order;
            if (selectedOrder != null)
            {
                lvOrderDetails.ItemsSource = selectedOrder.Items;
            }
            else
            {
                lvOrderDetails.ItemsSource = null;
            }
        }

        private void btnPayOrder_Click(object sender, RoutedEventArgs e)
        {
            Order selectedOrder = lvOrders.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для оплаты.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Проверка, что все блюда готовы
                if (!selectedOrder.AreAllItemsReady())
                {
                    MessageBox.Show("Не все блюда в заказе готовы. Дождитесь приготовления всех блюд.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Изменение статуса заказа на "Оплачен"
                selectedOrder.Status = OrderStatus.Completed;
                selectedOrder.CompletionTime = DateTime.Now;
                
                // Обновление списка заказов
                LoadCurrentOrders();
                
                MessageBox.Show("Заказ успешно оплачен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оплате заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
