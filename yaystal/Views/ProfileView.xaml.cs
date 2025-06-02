using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для ProfileView.xaml
    /// </summary>
    public partial class ProfileView : Page
    {
        private DataStorage _dataStorage;
        private Client _currentClient;
        private ObservableCollection<Address> _addresses;

        public ProfileView()
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
            
            // Загрузка адресов клиента
            if (_currentClient.Addresses != null && _currentClient.Addresses.Count > 0)
            {
                _addresses = new ObservableCollection<Address>(_currentClient.Addresses);
                lvAddresses.ItemsSource = _addresses;
            }
            else
            {
                _addresses = new ObservableCollection<Address>();
                lvAddresses.ItemsSource = _addresses;
            }
            
            // Отображение информации о клиенте
            DisplayClientInfo();
        }

        // Отображение информации о клиенте
        private void DisplayClientInfo()
        {
            if (_currentClient != null)
            {
                txtName.Text = _currentClient.Name;
                txtPhone.Text = _currentClient.PhoneNumber;
                txtEmail.Text = !string.IsNullOrEmpty(_currentClient.Email) ? _currentClient.Email : "client@example.com";
                txtBonuses.Text = _currentClient.BonusPoints.ToString();
            }
        }

        // Обработчики событий
        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Здесь будет открываться диалоговое окно для редактирования профиля
                MessageBox.Show("Функция редактирования профиля будет доступна в следующей версии.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание нового адреса
                int newId = _dataStorage.Addresses != null && _dataStorage.Addresses.Count > 0 ? 
                    _dataStorage.Addresses.Max(a => a.Id) + 1 : 1;
                
                // Если это первый адрес, устанавливаем его как основной
                bool isFirstAddress = _addresses.Count == 0;
                
                // Создаем новый адрес с указанием флага isMain
                Address newAddress = new Address(newId, "Улица Новая", "10", "101", isFirstAddress);
                
                // Добавление адреса в коллекцию и клиенту
                _addresses.Add(newAddress);
                _currentClient.AddAddress(newAddress);
                
                // Добавление адреса в хранилище данных
                if (_dataStorage.Addresses != null)
                {
                    _dataStorage.Addresses.Add(newAddress);
                }
                
                MessageBox.Show("Адрес успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSetMainAddress_Click(object sender, RoutedEventArgs e)
        {
            Address selectedAddress = lvAddresses.SelectedItem as Address;
            if (selectedAddress == null)
            {
                MessageBox.Show("Выберите адрес, который хотите сделать основным.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Сбрасываем флаг IsMain у всех адресов
                foreach (var address in _addresses)
                {
                    address.IsMain = false;
                }
                
                // Устанавливаем флаг IsMain у выбранного адреса
                selectedAddress.IsMain = true;
                
                // Устанавливаем выбранный адрес как основной у клиента
                _currentClient.SetMainAddress(selectedAddress);
                
                // Обновляем отображение списка
                lvAddresses.Items.Refresh();
                
                MessageBox.Show("Адрес успешно установлен как основной!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при установке основного адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveAddress_Click(object sender, RoutedEventArgs e)
        {
            Address selectedAddress = lvAddresses.SelectedItem as Address;
            if (selectedAddress == null)
            {
                MessageBox.Show("Выберите адрес для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Удаление адреса из коллекции
                _addresses.Remove(selectedAddress);
                
                // Удаление адреса у клиента
                _currentClient.RemoveAddress(selectedAddress);
                
                // Удаление адреса из хранилища данных
                if (_dataStorage.Addresses != null)
                {
                    _dataStorage.Addresses.Remove(selectedAddress);
                }
                
                MessageBox.Show("Адрес успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
