using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для CourierView.xaml
    /// </summary>
    public partial class CourierView : Page
    {
        private DataStorage _dataStorage;
        private ObservableCollection<Order> _activeOrders;
        private ObservableCollection<Order> _takenOrders;
        private Order _selectedOrder;
        private System.Windows.Threading.DispatcherTimer _routeTimer;
        private int _routeTimeMinutes;
        private bool _routeStarted;
        private List<Ellipse> _orderPoints;
        private List<Line> _routeLines;

        // Константы для отображения карты
        private const double RESTAURANT_SIZE = 20;
        private const double ORDER_SIZE = 15;
        private const double RESTAURANT_X = 200;
        private const double RESTAURANT_Y = 200;

        public CourierView()
        {
            InitializeComponent();
            
            _dataStorage = DataStorage.GetInstance();
            _activeOrders = new ObservableCollection<Order>(_dataStorage.GetDeliveryOrders());
            _takenOrders = new ObservableCollection<Order>();
            _orderPoints = new List<Ellipse>();
            _routeLines = new List<Line>();
            
            // Привязка данных
            lvActiveOrders.ItemsSource = _activeOrders;
            lvTakenOrders.ItemsSource = _takenOrders;

            // Инициализация таймера для отслеживания времени маршрута
            _routeTimer = new System.Windows.Threading.DispatcherTimer();
            _routeTimer.Tick += RouteTimer_Tick;
            _routeTimer.Interval = TimeSpan.FromSeconds(1);
            _routeTimeMinutes = 0;
            _routeStarted = false;

            // Отрисовка карты
            DrawMap();
        }

        // Отрисовка карты
        private void DrawMap()
        {
            mapCanvas.Children.Clear();
            _orderPoints.Clear();
            _routeLines.Clear();

            // Отрисовка ресторана (квадрат в центре)
            Rectangle restaurant = new Rectangle
            {
                Width = RESTAURANT_SIZE,
                Height = RESTAURANT_SIZE,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(restaurant, RESTAURANT_X - RESTAURANT_SIZE / 2);
            Canvas.SetTop(restaurant, RESTAURANT_Y - RESTAURANT_SIZE / 2);
            mapCanvas.Children.Add(restaurant);

            // Отрисовка активных заказов (полые круги)
            foreach (var order in _activeOrders)
            {
                DrawOrderPoint(order, false, false);
            }

            // Отрисовка взятых заказов (закрашенные круги)
            foreach (var order in _takenOrders)
            {
                DrawOrderPoint(order, true, false);
            }

            // Отрисовка выбранного заказа (если есть)
            if (_selectedOrder != null && _activeOrders.Contains(_selectedOrder))
            {
                DrawOrderPoint(_selectedOrder, false, true);
            }

            // Отрисовка маршрута (если есть взятые заказы)
            if (_takenOrders.Count > 0)
            {
                DrawRoute();
            }
        }

        // Отрисовка точки заказа
        private void DrawOrderPoint(Order order, bool isTaken, bool isSelected)
        {
            if (order.DeliveryAddress == null)
                return;

            // Преобразуем координаты адреса в координаты на канвасе
            double x = order.DeliveryAddress.X * 4;
            double y = order.DeliveryAddress.Y * 4;

            Ellipse point = new Ellipse
            {
                Width = ORDER_SIZE,
                Height = ORDER_SIZE,
                Stroke = isSelected ? Brushes.Blue : Brushes.Black,
                StrokeThickness = isSelected ? 3 : 1,
                Fill = isTaken ? Brushes.Green : Brushes.Transparent
            };
            Canvas.SetLeft(point, x - ORDER_SIZE / 2);
            Canvas.SetTop(point, y - ORDER_SIZE / 2);
            mapCanvas.Children.Add(point);
            _orderPoints.Add(point);

            // Добавляем текст с ID заказа
            TextBlock text = new TextBlock
            {
                Text = order.Id.ToString(),
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(text, x - 5);
            Canvas.SetTop(text, y - 20);
            mapCanvas.Children.Add(text);
        }

        // Отрисовка маршрута
        private void DrawRoute()
        {
            if (_takenOrders.Count == 0)
                return;

            // Начинаем маршрут от ресторана
            double lastX = RESTAURANT_X;
            double lastY = RESTAURANT_Y;

            foreach (var order in _takenOrders)
            {
                if (order.DeliveryAddress == null)
                    continue;

                // Преобразуем координаты адреса в координаты на канвасе
                double x = order.DeliveryAddress.X * 4;
                double y = order.DeliveryAddress.Y * 4;

                // Рисуем линию от предыдущей точки к текущей
                Line line = new Line
                {
                    X1 = lastX,
                    Y1 = lastY,
                    X2 = x,
                    Y2 = y,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2
                };
                mapCanvas.Children.Add(line);
                _routeLines.Add(line);

                // Обновляем последнюю точку
                lastX = x;
                lastY = y;
            }

            // Рисуем линию от последней точки обратно к ресторану
            Line returnLine = new Line
            {
                X1 = lastX,
                Y1 = lastY,
                X2 = RESTAURANT_X,
                Y2 = RESTAURANT_Y,
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };
            mapCanvas.Children.Add(returnLine);
            _routeLines.Add(returnLine);

            // Рассчитываем время маршрута
            CalculateRouteTime();
        }

        // Расчет времени маршрута
        private void CalculateRouteTime()
        {
            // Простая формула для расчета времени маршрута:
            // Время = сумма расстояний между точками + 10 минут * количество заказов
            double totalDistance = 0;
            
            // Начинаем от ресторана
            double lastX = RESTAURANT_X;
            double lastY = RESTAURANT_Y;

            foreach (var order in _takenOrders)
            {
                if (order.DeliveryAddress == null)
                    continue;

                // Преобразуем координаты адреса в координаты на канвасе
                double x = order.DeliveryAddress.X * 4;
                double y = order.DeliveryAddress.Y * 4;

                // Рассчитываем расстояние
                double distance = Math.Sqrt(Math.Pow(x - lastX, 2) + Math.Pow(y - lastY, 2));
                totalDistance += distance;

                // Обновляем последнюю точку
                lastX = x;
                lastY = y;
            }

            // Добавляем расстояние от последней точки до ресторана
            totalDistance += Math.Sqrt(Math.Pow(RESTAURANT_X - lastX, 2) + Math.Pow(RESTAURANT_Y - lastY, 2));

            // Переводим расстояние в минуты (условно: 10 пикселей = 1 минута)
            int distanceMinutes = (int)(totalDistance / 10);
            
            // Добавляем время на обслуживание клиентов
            int serviceMinutes = _takenOrders.Count * 10;
            
            // Общее время маршрута
            _routeTimeMinutes = distanceMinutes + serviceMinutes;
            
            // Обновляем отображение
            UpdateRouteTimeDisplay();
        }

        // Обновление отображения времени маршрута
        private void UpdateRouteTimeDisplay()
        {
            txtRouteTime.Text = $"{_routeTimeMinutes} мин";
        }

        // Обработчик тика таймера маршрута
        private void RouteTimer_Tick(object sender, EventArgs e)
        {
            if (_routeStarted)
            {
                _routeTimeMinutes--;
                UpdateRouteTimeDisplay();
            }
        }

        // Обработчик выбора активного заказа
        private void lvActiveOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvActiveOrders.SelectedItem is Order selectedOrder)
            {
                _selectedOrder = selectedOrder;
                btnTakeOrder.IsEnabled = true;
                
                // Перерисовываем карту, чтобы выделить выбранный заказ
                DrawMap();
            }
            else
            {
                btnTakeOrder.IsEnabled = false;
            }
        }

        // Обработчик нажатия кнопки "Взять заказ"
        private void btnTakeOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder != null && _activeOrders.Contains(_selectedOrder))
            {
                // Проверяем, готов ли заказ
                if (_selectedOrder.Status != OrderStatus.Ready)
                {
                    MessageBox.Show("Заказ еще не готов к доставке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Назначаем курьера
                _selectedOrder.AssignCourier(_dataStorage.GetEmployeesByType(EmployeeType.Courier).FirstOrDefault() as Courier);
                
                // Перемещаем заказ из активных во взятые
                _activeOrders.Remove(_selectedOrder);
                _takenOrders.Add(_selectedOrder);
                
                // Сбрасываем выбранный заказ
                _selectedOrder = null;
                btnTakeOrder.IsEnabled = false;
                
                // Перерисовываем карту
                DrawMap();
                
                // Проверяем, можно ли начать маршрут
                CheckRouteStartAvailability();
            }
        }

        // Проверка доступности кнопки "Начать маршрут"
        private void CheckRouteStartAvailability()
        {
            // Маршрут можно начать, если есть хотя бы один взятый заказ
            // и все заказы готовы к доставке
            bool canStart = _takenOrders.Count > 0 && _takenOrders.All(o => o.Status == OrderStatus.InDelivery);
            btnStartRoute.IsEnabled = canStart && !_routeStarted;
            btnPayOrder.IsEnabled = _routeStarted && _takenOrders.Count > 0;
        }

        // Обработчик нажатия кнопки "Начать маршрут"
        private void btnStartRoute_Click(object sender, RoutedEventArgs e)
        {
            if (_takenOrders.Count == 0)
            {
                MessageBox.Show("Нет взятых заказов", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Начинаем отсчет времени
            _routeStarted = true;
            _routeTimer.Start();
            
            // Обновляем доступность кнопок
            btnStartRoute.IsEnabled = false;
            btnPayOrder.IsEnabled = true;
            btnTakeOrder.IsEnabled = false;
            lvActiveOrders.IsEnabled = false;
        }

        // Обработчик нажатия кнопки "Оплатить заказ"
        private void btnPayOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_takenOrders.Count == 0)
            {
                MessageBox.Show("Нет взятых заказов", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Берем первый заказ из списка
            Order order = _takenOrders[0];
            
            // Отмечаем заказ как выполненный
            order.Complete();
            
            // Удаляем заказ из списка взятых
            _takenOrders.Remove(order);
            
            // Если это был последний заказ, останавливаем таймер
            if (_takenOrders.Count == 0)
            {
                _routeTimer.Stop();
                _routeStarted = false;
                lvActiveOrders.IsEnabled = true;
                
                // Обновляем метрику эффективности курьера
                // В реальном приложении здесь была бы логика обновления метрики
                
                // Обновляем список активных заказов
                _activeOrders.Clear();
                foreach (var activeOrder in _dataStorage.GetDeliveryOrders())
                {
                    _activeOrders.Add(activeOrder);
                }
            }
            
            // Перерисовываем карту
            DrawMap();
            
            // Проверяем доступность кнопок
            CheckRouteStartAvailability();
        }
    }
}
