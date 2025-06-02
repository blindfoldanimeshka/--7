using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using yaystal.Models;

namespace yaystal
{
    /// <summary>
    /// </summary>
    public partial class LoginWindow : Window
    {
        private DataStorage _dataStorage;
        private Random _random = new Random();
        private string _verificationCode;

        public LoginWindow()
        {
            InitializeComponent();
            _dataStorage = DataStorage.GetInstance();
            
            rbEmployee.Checked += RbUserType_Checked;
            rbClient.Checked += RbUserType_Checked;
            btnSendCode.Click += BtnSendCode_Click;
            btnLogin.Click += BtnLogin_Click;
            txtClientPhone.TextChanged += TxtClientPhone_TextChanged;
            txtClientCode.PreviewTextInput += TxtClientCode_PreviewTextInput;
        }
        
        private void RbUserType_Checked(object sender, RoutedEventArgs e)
        {
            if (rbEmployee != null && rbClient != null && panelEmployee != null && panelClient != null)
            {
                if (rbEmployee.IsChecked == true)
                {
                    panelEmployee.Visibility = Visibility.Visible;
                    panelClient.Visibility = Visibility.Collapsed;
                }
                else
                {
                    panelEmployee.Visibility = Visibility.Collapsed;
                    panelClient.Visibility = Visibility.Visible;
                }
            }
        }
        
        private void TxtClientPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            string phone = txtClientPhone.Text;
            
            phone = Regex.Replace(phone, @"[^\d]", "");

            if (phone.StartsWith("8") || phone.StartsWith("7"))
            {
                phone = phone.Substring(1);
            }
            
            if (phone.Length > 10)
            {
                phone = phone.Substring(0, 10);
            }
            
            if (phone != txtClientPhone.Text)
            {
                txtClientPhone.Text = phone;
                txtClientPhone.CaretIndex = phone.Length; 
            }
        }

        private void TxtClientCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            return Regex.IsMatch(text, "[0-9]");
        }
        
        private void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {
            _verificationCode = _random.Next(1000, 10000).ToString();
            
            MessageBox.Show($"Ваш код подтверждения: {_verificationCode}", "Код подтверждения", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (rbEmployee.IsChecked == true)
            {
                LoginEmployee();
            }
            else
            {
                LoginClient();
            }
        }
        
        private void LoginEmployee()
        {
            string login = txtEmployeeLogin.Text;
            string password = txtEmployeePassword.Password;
            
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            Employee currentEmployee = null;
            
            if (login == "admin" && password == "admin")
            {
                currentEmployee = _dataStorage.Employees.OfType<Administrator>().FirstOrDefault();
                if (currentEmployee == null)
                {
                    currentEmployee = new Administrator(_dataStorage.Employees.Count + 1, "Администратор");
                    _dataStorage.AddEmployee(currentEmployee);
                }
            }
            else if (login == "cook" && password == "cook")
            {
                currentEmployee = _dataStorage.Employees.OfType<Cook>().FirstOrDefault();
                if (currentEmployee == null)
                {
                    currentEmployee = new Cook(_dataStorage.Employees.Count + 1, "Повар");
                    _dataStorage.AddEmployee(currentEmployee);
                }
            }
            else if (login == "courier" && password == "courier")
            {
                currentEmployee = _dataStorage.Employees.OfType<Courier>().FirstOrDefault();
                if (currentEmployee == null)
                {
                    currentEmployee = new Courier(_dataStorage.Employees.Count + 1, "Курьер");
                    _dataStorage.AddEmployee(currentEmployee);
                }
            }
            
            if (currentEmployee != null)
            {
                MainWindow mainWindow = new MainWindow(currentEmployee, MainWindow.UserType.Employee);
                mainWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoginClient()
        {
            string phone = txtClientPhone.Text;
            string code = txtClientCode.Text;
            
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Введите номер телефона", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Введите код подтверждения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // ПРОВЕРКА
            if (code != _verificationCode)
            {
                MessageBox.Show("Неверный код подтверждения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            Client currentClient = null;
            
            foreach (var client in _dataStorage.Clients)
            {
                if (client.PhoneNumber != null && client.PhoneNumber.EndsWith(phone))
                {
                    currentClient = client;
                    break;
                }
            }
            
            if (currentClient == null)
            {
                int newId = _dataStorage.Clients.Count > 0 ? _dataStorage.Clients.Max(c => c.Id) + 1 : 1;
                currentClient = new Client(newId, "Новый клиент", $"+7{phone}", null);
                _dataStorage.AddClient(currentClient);
            }
            
            MainWindow mainWindow = new MainWindow(currentClient, MainWindow.UserType.Client);
            mainWindow.Show();
            
            this.Close();
        }
    }
}
