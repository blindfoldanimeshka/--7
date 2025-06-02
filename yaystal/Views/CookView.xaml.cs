using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для CookView.xaml
    /// </summary>
    public partial class CookView : Page
    {
        private DataStorage _dataStorage;
        private ObservableCollection<Order> _activeOrders;
        private ObservableCollection<OrderItem> _notReadyItems;
        private ObservableCollection<OrderItem> _readyItems;
        private Order _selectedOrder;
        private System.Windows.Threading.DispatcherTimer _timer;
        private TimeSpan _elapsedTime;

        public CookView()
        {
            InitializeComponent();
            
            _dataStorage = DataStorage.GetInstance();
            _activeOrders = new ObservableCollection<Order>(_dataStorage.GetActiveOrders());
            _notReadyItems = new ObservableCollection<OrderItem>();
            _readyItems = new ObservableCollection<OrderItem>();
            
            // Привязка данных
            lvActiveOrders.ItemsSource = _activeOrders;
            lvNotReadyItems.ItemsSource = _notReadyItems;
            lvReadyItems.ItemsSource = _readyItems;

            // Инициализация таймера для отслеживания времени приготовления
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _elapsedTime = TimeSpan.Zero;
            _timer.Start();
        }

        // Обработчик тика таймера
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_selectedOrder != null)
            {
                _selectedOrder.CookingTimer = _selectedOrder.CookingTimer.Add(TimeSpan.FromSeconds(1));
                _elapsedTime = _selectedOrder.CookingTimer;
            }
        }

        // Обработчик выбора заказа
        private void lvActiveOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvActiveOrders.SelectedItem is Order selectedOrder)
            {
                _selectedOrder = selectedOrder;
                
                // Обновляем списки готовых и неготовых блюд
                _notReadyItems.Clear();
                _readyItems.Clear();
                
                foreach (var item in selectedOrder.GetNotReadyItems())
                {
                    _notReadyItems.Add(item);
                }
                
                foreach (var item in selectedOrder.GetReadyItems())
                {
                    _readyItems.Add(item);
                }
            }
        }

        // Обработчик нажатия кнопки "Готово"
        private void btnMarkAsReady_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (lvNotReadyItems.SelectedItem is OrderItem selectedItem)
            {
                // Отмечаем блюдо как готовое
                _selectedOrder.MarkItemAsReady(selectedItem.Id, _elapsedTime);
                
                // Обновляем эффективность повара
                // В реальном приложении здесь была бы логика обновления метрики эффективности
                // на основе разницы между ожидаемым и фактическим временем приготовления
                
                // Обновляем списки
                _notReadyItems.Remove(selectedItem);
                _readyItems.Add(selectedItem);
                
                // Если все блюда готовы, обновляем статус заказа
                if (_selectedOrder.AreAllItemsReady())
                {
                    _selectedOrder.Status = OrderStatus.Ready;
                    
                    // Обновляем список активных заказов, если заказ больше не активен
                    if (_selectedOrder.Status != OrderStatus.InProgress)
                    {
                        // Обновляем список активных заказов
                        _activeOrders.Clear();
                        foreach (var order in _dataStorage.GetActiveOrders())
                        {
                            _activeOrders.Add(order);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите блюдо из списка неготовых блюд", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
