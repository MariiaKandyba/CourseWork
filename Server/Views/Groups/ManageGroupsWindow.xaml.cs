using DALTest.Entities;
using Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Server.Views
{
    /// <summary>
    /// Interaction logic for ManageGroupsWindow.xaml
    /// </summary>
    public partial class ManageGroupsWindow : Window
    {
        public ObservableCollection<User> UsersInGroup { get; set; } 
        public ObservableCollection<User> OtherUsers { get; set; } 
        public ManageGroupsWindow(List<User> usersInGroup, List<User> otherUsers)
        {
            InitializeComponent();

            this.UsersInGroup = new ObservableCollection<User>( usersInGroup);
            this.OtherUsers = new ObservableCollection<User>(otherUsers);
            inGroup.ItemsSource = usersInGroup;
            others.ItemsSource = otherUsers;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (others.SelectedItem is User selectedUser)
            {
                UsersInGroup.Add(selectedUser);
                OtherUsers.Remove(selectedUser);
                inGroup.ItemsSource = UsersInGroup;
                others.ItemsSource = OtherUsers;
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (inGroup.SelectedItem is User selectedUser)
            {
                OtherUsers.Add(selectedUser);
                UsersInGroup.Remove(selectedUser);
                inGroup.ItemsSource = UsersInGroup;
                others.ItemsSource = OtherUsers;
            }
        }

        private void groupButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
