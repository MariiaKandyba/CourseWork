using DALTest.Entities;
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
using System.Windows.Shapes;
using TestServices;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Views
{
    /// <summary>
    /// Interaction logic for CreateEditUser.xaml
    /// </summary>
    public partial class CreateEditUser : Window
    {
        public User User{ get; set; }

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
