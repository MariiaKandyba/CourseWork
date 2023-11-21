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

namespace Server.Views.Users
{
    /// <summary>
    /// Interaction logic for RestoreUserWindow.xaml
    /// </summary>
    public partial class RestoreUserWindow : Window
    {

        public List<User> Users { get; set; }
        public List<User> SelectedUsers { get; set; } = new();
        public RestoreUserWindow(List<User> users)
        {
            InitializeComponent();
            Users = users; 
            grid.ItemsSource = Users;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (var item in grid.SelectedItems)
            {
                if (item is User user)
                    SelectedUsers.Add(user);
            }
            DialogResult = true;

        }
    }
}
