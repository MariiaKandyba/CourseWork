using Client.ViewModels;
using DALTest.Entities;
using DALTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Views;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IGenericRepository<User> _userRepository;
        IGenericRepository<UserTest> _userTestRepository;
        GenericUnitOfWork _unitOfWork;

        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //string username = UsernameTextBox.Text;
            //string password = PasswordBox.Password;
             string username = "mary_smith";
            string password = "pass123";
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string conStr = config.GetConnectionString("DefaultConnection")!;

            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            var options = optionsBuilder.UseLazyLoadingProxies().UseSqlServer(conStr).Options;
            _unitOfWork = new GenericUnitOfWork(new Context(options));
            _userRepository = _unitOfWork.Repository<User>();
            _userTestRepository = _unitOfWork.Repository<UserTest>();



            if (YourAuthenticationLogic(username, password))
            {
                User user = _userRepository.GetAll().FirstOrDefault(x => x.Login == username && x.Password == password);
                Account account = new Account(_unitOfWork, user);
                account.Show();

                Close();
            }
            else
            {
                ErrorMessageText.Text = "Невірний логін або пароль. Спробуйте ще раз.";
            }
        }

        private bool YourAuthenticationLogic(string username, string password)
        {
            User user = _userRepository.GetAll().FirstOrDefault(x => x.Login == username && x.Password == password);
            return user == null? false : true;
        }

    }
}
