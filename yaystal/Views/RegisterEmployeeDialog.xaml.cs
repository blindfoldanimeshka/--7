using System;
using System.Windows;
using yaystal.Models;

namespace yaystal.Views
{
    /// <summary>
    /// Логика взаимодействия для RegisterEmployeeDialog.xaml
    /// </summary>
    public partial class RegisterEmployeeDialog : Window
    {
        public Employee NewEmployee { get; private set; }

        public RegisterEmployeeDialog()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeName.Text))
            {
                MessageBox.Show("Введите имя сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаем нового сотрудника в зависимости от выбранного типа
            switch (cmbEmployeeType.SelectedIndex)
            {
                case 0: // Администратор
                    NewEmployee = new Administrator(0, txtEmployeeName.Text);
                    break;
                case 1: // Повар
                    NewEmployee = new Cook(0, txtEmployeeName.Text);
                    break;
                case 2: // Курьер
                    NewEmployee = new Courier(0, txtEmployeeName.Text);
                    break;
                default:
                    NewEmployee = new Cook(0, txtEmployeeName.Text);
                    break;
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
