using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для OrderHistoryView.xaml
    /// </summary>
    public partial class OrderHistoryView : Page
    {
        private DataStorage _dataStorage;
        private Client _currentClient;
        private ObservableCollection<Order> _clientOrders;

        public OrderHistoryView()
        {
            InitializeComponent();
            
            // Инициализация данных
            _dataStorage = DataStorage.GetInstance();
            
            // Получение текущего клиента (для примера берем первого)
            _currentClient = _dataStorage.Clients.Count > 0 ? _dataStorage.Clients[0] : null;
            
            if (_currentClient == null)
            {
                MessageBox.Show("Не удалось загрузить данные клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Установка начальных дат для фильтра
            dpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Now;
            
            // Загрузка заказов клиента
            LoadClientOrders();
        }

        // Загрузка заказов клиента
        private void LoadClientOrders()
        {
            try
            {
                // Фильтрация заказов по клиенту
                var filteredOrders = _dataStorage.Orders.Where(o => o.Client != null && o.Client.Id == _currentClient.Id);
                
                // Применение фильтра по датам
                if (dpStartDate.SelectedDate.HasValue)
                {
                    DateTime startDate = dpStartDate.SelectedDate.Value.Date;
                    filteredOrders = filteredOrders.Where(o => o.CreationTime.Date >= startDate);
                }
                
                if (dpEndDate.SelectedDate.HasValue)
                {
                    DateTime endDate = dpEndDate.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);
                    filteredOrders = filteredOrders.Where(o => o.CreationTime <= endDate);
                }
                
                // Применение фильтра по типу заказа
                if (cmbOrderType.SelectedIndex > 0)
                {
                    OrderType selectedType = OrderType.Pickup; // По умолчанию
                    
                    switch (cmbOrderType.SelectedIndex)
                    {
                        case 1: // Самовывоз
                            selectedType = OrderType.Pickup;
                            break;
                        case 2: // Доставка
                            selectedType = OrderType.Delivery;
                            break;
                    }
                    
                    filteredOrders = filteredOrders.Where(o => o.Type == selectedType);
                }
                
                // Создание коллекции для отображения
                _clientOrders = new ObservableCollection<Order>(filteredOrders.OrderByDescending(o => o.CreationTime));
                lvOrders.ItemsSource = _clientOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Отображение деталей заказа
        private void DisplayOrderDetails(Order order)
        {
            if (order == null)
            {
                lvOrderItems.ItemsSource = null;
                txtDeliveryAddress.Text = "-";
                txtDeliveryTime.Text = "-";
                txtTotalPrice.Text = "0 ₽";
                return;
            }
            
            // Отображение позиций заказа
            lvOrderItems.ItemsSource = order.Items;
            
            // Отображение адреса доставки
            if (order.Type == OrderType.Delivery && order.DeliveryAddress != null)
            {
                txtDeliveryAddress.Text = $"{order.DeliveryAddress.Street}, {order.DeliveryAddress.HouseNumber}, кв. {order.DeliveryAddress.ApartmentNumber}";
            }
            else
            {
                txtDeliveryAddress.Text = "-";
            }
            
            // Отображение времени доставки
            if (order.Type == OrderType.Delivery && order.DeliveryStartTime.HasValue)
            {
                txtDeliveryTime.Text = order.DeliveryStartTime.Value.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                txtDeliveryTime.Text = "-";
            }
            
            // Отображение общей стоимости
            txtTotalPrice.Text = $"{order.GetTotalPrice()} ₽";
        }

        // Обработчики событий
        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadClientOrders();
        }

        private void lvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Order selectedOrder = lvOrders.SelectedItem as Order;
            DisplayOrderDetails(selectedOrder);
        }
    }
}
