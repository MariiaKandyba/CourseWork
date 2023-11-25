using DALTest;
using DALTest.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
using Server.Helpers;
using Server.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace Server.Views
{
    /// <summary>
    /// Interaction logic for MainInterfaceWindow.xaml
    /// </summary>
    public partial class MainInterfaceWindow : Window
    {

        public bool IsValid;
        private readonly UsersViewModel _usersViewModel;
        private readonly GroupViewModel _groupViewModel;
        private readonly AssigmentViewModel _assigmentViewModel;
        private readonly ServerViewModel _serverViewModel;
        public MainInterfaceWindow(string login, string password)
        {
           RepositoryHelper repositoryHelper = new ();
            IsValid = repositoryHelper.IsVerifed(login, password);

            InitializeComponent();
            _usersViewModel = new UsersViewModel(repositoryHelper.UserRepository, repositoryHelper.RepositoryFilter) ;
            _groupViewModel = new GroupViewModel(repositoryHelper.GroupRepository, repositoryHelper.UserRepository);
            _assigmentViewModel = new AssigmentViewModel(
                repositoryHelper.UserTestRepository, 
                repositoryHelper.TestRepository, 
                repositoryHelper.GroupRepository,
                repositoryHelper.UserRepository, 
                repositoryHelper.QuestionRepository, 
                repositoryHelper.AnswerRepository);
            _serverViewModel = new ServerViewModel();

            usersTab.DataContext = _usersViewModel;
            groupsTab.DataContext = _groupViewModel;
            assignedTestTab.DataContext = _assigmentViewModel;
            serversTab.DataContext = _serverViewModel;
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();

            Close();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }
    }
}
