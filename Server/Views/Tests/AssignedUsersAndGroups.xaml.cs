using DALTest.Entities;
using Microsoft.VisualBasic.ApplicationServices;
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
using User = DALTest.Entities.User;

namespace Server.Views
{
    /// <summary>
    /// Interaction logic for AssignedUsersAndGroups.xaml
    /// </summary>
    public partial class AssignedUsersAndGroups : Window
    {
        public ObservableCollection<Group> Groups { get; set; }
        public ObservableCollection<User> Users { get; set; } = new();
        public AssignedUsersAndGroups(List<Group> groups, List<User> users)
        {
            InitializeComponent();
            Groups = new ObservableCollection<Group>(groups);
            Users = new ObservableCollection<User>(users);
            Groupsgrid.ItemsSource = Groups;
            UsersGrid.ItemsSource = Users;
        }
    }
}
