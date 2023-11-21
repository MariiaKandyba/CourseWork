using DALTest.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using TestServices;

namespace Server.Views
{
    public partial class CreateEditUser : Window
    {
        public User User { get; set; }

        public CreateEditUser(User user = null!)
        {
            InitializeComponent();
            if (user != null)
            {
                User = user;
                FirstNameTextBox.Text = user.FirstName;
                LastNameTextBox.Text = user.LastName;
                LoginTextBox.Text = user.Login;
                PasswordBox.Password = user.Password;
                DescriptionTextBox.Text = user.Description;
                AdminCheckBox.IsChecked = user.IsAdmin;
            }
            else
            {
                User = new User();
                User.RegisterDate = DateTime.Now;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                User.FirstName = FirstNameTextBox.Text;
                User.LastName = LastNameTextBox.Text;
                User.Login = LoginTextBox.Text;
                User.Password = PasswordBox.Password;
                User.Description = DescriptionTextBox.Text;
                User.IsAdmin = AdminCheckBox.IsChecked ?? false;
                User.IsArchived = false;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                return false;
            }
            return true;
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordTextBox.Text = PasswordBox.Password;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Password = PasswordTextBox.Text;
        }
    }
}
