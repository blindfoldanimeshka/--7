using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : Page
    {
        private DataStorage _dataStorage;
        private ObservableCollection<Employee> _employees;

        public AdminView()
        {
            InitializeComponent();
            
            try
            {
                // Получаем экземпляр хранилища данных
                _dataStorage = DataStorage.GetInstance();
                
                // Проверяем, что коллекция сотрудников инициализирована
                if (_dataStorage.Employees != null)
                {
                    _employees = new ObservableCollection<Employee>(_dataStorage.Employees);
                }
                else
                {
                    _employees = new ObservableCollection<Employee>();
                }
                
                // Привязка данных
                dgEmployees.ItemsSource = _employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _employees = new ObservableCollection<Employee>();
                dgEmployees.ItemsSource = _employees;
            }
        }

        // Обработчик изменения выбранного типа сотрудника
        private void cmbEmployeeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Проверяем, что комбобокс и коллекция сотрудников инициализированы
                if (cmbEmployeeType == null || _employees == null || _dataStorage == null || _dataStorage.Employees == null)
                    return;
                    
                if (cmbEmployeeType.SelectedIndex < 0)
                    return;

                // Фильтрация сотрудников по выбранному типу
                _employees.Clear();
                
                switch (cmbEmployeeType.SelectedIndex)
                {
                    case 0: // Все сотрудники
                        foreach (var employee in _dataStorage.Employees.OrderByDescending(emp => emp.EfficiencyMetric))
                        {
                            _employees.Add(employee);
                        }
                        break;
                    case 1: // Администраторы
                        var admins = _dataStorage.GetEmployeesByType(EmployeeType.Administrator);
                        if (admins != null)
                        {
                            foreach (var employee in admins.OrderByDescending(emp => emp.EfficiencyMetric))
                            {
                                _employees.Add(employee);
                            }
                        }
                        break;
                    case 2: // Повара
                        var cooks = _dataStorage.GetEmployeesByType(EmployeeType.Cook);
                        if (cooks != null)
                        {
                            foreach (var employee in cooks.OrderByDescending(emp => emp.EfficiencyMetric))
                            {
                                _employees.Add(employee);
                            }
                        }
                        break;
                    case 3: // Курьеры
                        var couriers = _dataStorage.GetEmployeesByType(EmployeeType.Courier);
                        if (couriers != null)
                        {
                            foreach (var employee in couriers.OrderByDescending(emp => emp.EfficiencyMetric))
                            {
                                _employees.Add(employee);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия кнопки "Общая статистика клиентов"
        private void btnClientStats_Click(object sender, RoutedEventArgs e)
        {
            // В реальном приложении здесь был бы переход на страницу статистики
            MessageBox.Show("Функционал статистики клиентов находится в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Обработчик нажатия кнопки "Регистрация нового сотрудника"
        private void btnRegisterEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, что хранилище данных инициализировано
                if (_dataStorage == null || _dataStorage.Employees == null)
                {
                    MessageBox.Show("Не удалось получить доступ к хранилищу данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Создаем нового сотрудника
                int newId = _dataStorage.Employees.Count > 0 ? _dataStorage.Employees.Max(e => e.Id) + 1 : 1;
                var newEmployee = new Cook(newId, "Новый повар");
                
                // Добавляем нового сотрудника в хранилище
                _dataStorage.AddEmployee(newEmployee);
                
                // Обновляем список сотрудников
                // Вместо передачи null, используем прямое обновление списка
                RefreshEmployeesList();
                
                MessageBox.Show($"Сотрудник {newEmployee.Name} успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Метод для обновления списка сотрудников
        private void RefreshEmployeesList()
        {
            try
            {
                if (_dataStorage == null || _dataStorage.Employees == null || _employees == null)
                    return;
                    
                _employees.Clear();
                
                // Добавляем всех сотрудников в список
                foreach (var employee in _dataStorage.Employees)
                {
                    _employees.Add(employee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
