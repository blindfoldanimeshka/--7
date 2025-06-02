using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для OrderView.xaml
    /// </summary>
    public partial class OrderView : Page
    {
        private DataStorage _dataStorage;
        private ObservableCollection<OrderItem> _orderItems;
        private decimal _deliveryPrice = 150; // Стоимость доставки

        public OrderView()
        {
            InitializeComponent();
            
            _dataStorage = DataStorage.GetInstance();
            _orderItems = new ObservableCollection<OrderItem>();
            
            // Загрузка списка блюд
            lvDishes.ItemsSource = _dataStorage.Dishes;
            lvOrderItems.ItemsSource = _orderItems;
            
            // Установка начальных значений
            UpdatePrices();
        }

        // Обработчик двойного клика по блюду в меню
        private void lvDishes_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvDishes.SelectedItem is Dish selectedDish)
            {
                AddDishToOrder(selectedDish);
            }
        }

        // Добавление блюда в заказ
        private void AddDishToOrder(Dish dish)
        {
            int newId = _orderItems.Count > 0 ? _orderItems.Max(i => i.Id) + 1 : 1;
            _orderItems.Add(new OrderItem(newId, dish));
            UpdatePrices();
        }

        // Удаление блюда из заказа
        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvOrderItems.SelectedItem is OrderItem selectedItem)
            {
                _orderItems.Remove(selectedItem);
                UpdatePrices();
            }
        }

        // Обновление цен
        private void UpdatePrices()
        {
            try
            {
                // Проверка на null для предотвращения NullReferenceException
                if (_orderItems == null || rbDelivery == null || txtOrderPrice == null || txtDeliveryPrice == null || txtTotalPrice == null)
                    return;
                    
                decimal orderPrice = _orderItems.Sum(item => item.Dish.Price);
                decimal deliveryPrice = rbDelivery.IsChecked == true ? _deliveryPrice : 0;
                decimal totalPrice = orderPrice + deliveryPrice;

                txtOrderPrice.Text = $"{orderPrice} ₽";
                txtDeliveryPrice.Text = $"{deliveryPrice} ₽";
                txtTotalPrice.Text = $"{totalPrice} ₽";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении цен: {ex.Message}");
            }
        }

        // Обработчики изменения типа заказа
        private void rbDelivery_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на null для предотвращения NullReferenceException
                if (spDeliveryAddress != null)
                {
                    spDeliveryAddress.Visibility = Visibility.Visible;
                    UpdatePrices();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при изменении типа заказа: {ex.Message}");
            }
        }

        private void rbPickup_Checked(object sender, RoutedEventArgs e)
        {
            // Проверка на null для предотвращения NullReferenceException
            if (spDeliveryAddress != null && spAddressFields != null)
            {
                spDeliveryAddress.Visibility = Visibility.Collapsed;
                spAddressFields.Visibility = Visibility.Collapsed;
                UpdatePrices();
            }
        }

        // Обработчики изменения адреса доставки
        private void rbSelfAddress_Checked(object sender, RoutedEventArgs e)
        {
            // Проверка на null для предотвращения NullReferenceException
            if (spAddressFields != null)
            {
                spAddressFields.Visibility = Visibility.Collapsed;
            }
        }

        private void rbOtherAddress_Checked(object sender, RoutedEventArgs e)
        {
            // Проверка на null для предотвращения NullReferenceException
            if (spAddressFields != null)
            {
                spAddressFields.Visibility = Visibility.Visible;
            }
        }

        // Очистка заказа
        private void btnClearOrder_Click(object sender, RoutedEventArgs e)
        {
            _orderItems.Clear();
            UpdatePrices();
        }

        // Оформление заказа
        private void btnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одно блюдо в заказ", "Пустой заказ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // В реальном приложении здесь была бы логика работы с клиентом
                // Сейчас используем тестового клиента из хранилища данных
                Client client = _dataStorage.Clients.FirstOrDefault();
                if (client == null)
                {
                    MessageBox.Show("Ошибка: клиент не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Определяем тип заказа
                OrderType orderType = rbDelivery.IsChecked == true ? OrderType.Delivery : OrderType.Pickup;

                // Создаем адрес доставки, если выбрана доставка другому
                Address deliveryAddress = null;
                if (orderType == OrderType.Delivery && rbOtherAddress.IsChecked == true)
                {
                    if (string.IsNullOrWhiteSpace(txtStreet.Text) || string.IsNullOrWhiteSpace(txtHouse.Text))
                    {
                        MessageBox.Show("Введите адрес доставки", "Неполный адрес", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    deliveryAddress = new Address(0, txtStreet.Text, txtHouse.Text, txtApartment.Text);
                    _dataStorage.AddAddress(deliveryAddress);
                }
                else if (orderType == OrderType.Delivery)
                {
                    // Ищем основной адрес клиента или берем первый, если основной не задан
                    if (client.Addresses != null && client.Addresses.Count > 0)
                    {
                        deliveryAddress = client.Addresses.FirstOrDefault(a => a.IsMain) ?? client.Addresses.First();
                    }
                    else
                    {
                        MessageBox.Show("У клиента нет адресов доставки. Пожалуйста, добавьте адрес в профиле.", "Нет адреса", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Создаем заказ
                Order order = new Order(0, orderType, client, deliveryAddress);
                
                // Добавляем блюда в заказ
                foreach (var item in _orderItems)
                {
                    order.AddDish(item.Dish);
                }

                // Сохраняем заказ
                _dataStorage.AddOrder(order);

                MessageBox.Show($"Заказ №{order.Id} успешно оформлен!", "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Очищаем форму
                _orderItems.Clear();
                UpdatePrices();
                rbPickup.IsChecked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
