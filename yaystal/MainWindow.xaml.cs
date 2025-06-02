using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using yaystal.Models;
using yaystal.Views;

namespace yaystal
{
    /// <summary>
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button _currentActiveButton;
        
        private object _currentUser;
        
        public enum UserType
        {
            Client,
            Employee
        }
        
        private UserType _currentUserType;

        public MainWindow()
        {
            InitializeComponent();
            
            DataStorage.GetInstance();

            _currentUserType = UserType.Client;
            _currentUser = null;
            
            ConfigureUIForUserType();
            
            btnOrder_Click(btnOrder, null);
        }
        
        public MainWindow(object user, UserType userType)
        {
            InitializeComponent();
            
            DataStorage.GetInstance();
            
            _currentUser = user;
            _currentUserType = userType;
            
            ConfigureUIForUserType();
            
            btnOrder_Click(btnOrder, null);
        }
        
        private void ConfigureUIForUserType()
        {
            switch (_currentUserType)
            {
                case UserType.Client:
                    btnAdmin.Visibility = Visibility.Collapsed;
                    btnCourier.Visibility = Visibility.Collapsed;
                    btnCook.Visibility = Visibility.Collapsed;
                    break;
                    
                case UserType.Employee:
                    if (_currentUser is Administrator)
                    {
                        btnAdmin.Visibility = Visibility.Visible;
                        btnCourier.Visibility = Visibility.Visible;
                        btnCook.Visibility = Visibility.Visible;
                    }
                    else if (_currentUser is Cook)
                    {
                        btnAdmin.Visibility = Visibility.Collapsed;
                        btnCourier.Visibility = Visibility.Collapsed;
                        btnCook.Visibility = Visibility.Visible;
                    }
                    else if (_currentUser is Courier)
                    {
                        btnAdmin.Visibility = Visibility.Collapsed;
                        btnCourier.Visibility = Visibility.Visible;
                        btnCook.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnAdmin.Visibility = Visibility.Collapsed;
                        btnCourier.Visibility = Visibility.Collapsed;
                        btnCook.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        private void SetActiveButton(Button button)
        {
            if (_currentActiveButton != null)
            {
                _currentActiveButton.Style = (Style)FindResource("SidebarButtonStyle");
            }

            button.Style = (Style)FindResource("ActiveSidebarButtonStyle");
            _currentActiveButton = button;
        }

        private void btnCook_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnCook);
            MainFrame.Navigate(new CookView());
        }

        private void btnCourier_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnCourier);
            MainFrame.Navigate(new CourierView());
        }
        
        private void btnWaiter_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnWaiter);
            try
            {
                MainFrame.Navigate(new WaiterView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии представления официанта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnAdmin);
            try
            {
                MainFrame.Navigate(new AdminView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии панели администратора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnOrder);
            MainFrame.Navigate(new OrderView());
        }
        
        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnProfile);
            try
            {
                MainFrame.Navigate(new ProfileView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии профиля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void btnOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnOrderHistory);
            try
            {
                MainFrame.Navigate(new OrderHistoryView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии истории заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}