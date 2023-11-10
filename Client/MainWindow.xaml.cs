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
using System.Net.Sockets;
using Newtonsoft.Json;
using NetworkDataDll;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //public partial class MainWindow : Window
    //{
    //    IGenericRepository<User> _userRepository;
    //    User user;
    //    GenericUnitOfWork _unitOfWork;

    //    public MainWindow()
    //    {
    //        InitializeComponent();

    //    }

    //    private void LoginButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        string username = UsernameTextBox.Text;
    //        string password = PasswordBox.Password;
    //        var builder = new ConfigurationBuilder();
    //        builder.SetBasePath(Directory.GetCurrentDirectory());
    //        builder.AddJsonFile("appsettings.json");
    //        string conStr = builder.Build().GetConnectionString("DefaultConnection")!;

    //        var optionsBuilder = new DbContextOptionsBuilder<Context>();
    //        var options = optionsBuilder.UseLazyLoadingProxies().UseSqlServer(conStr).Options;
    //        _unitOfWork = new GenericUnitOfWork(new Context(options));
    //        _userRepository = _unitOfWork.Repository<User>();



    //        if (YourAuthenticationLogic(username, password))
    //        {
    //            Account account = new(_unitOfWork, user);
    //            account.Show();
    //            Close();
    //        }
    //        else
    //        {
    //            ErrorMessageText.Text = "Невірний логін або пароль. Спробуйте ще раз.";
    //        }
    //    }

    //    private bool YourAuthenticationLogic(string username, string password)
    //    {
    //        user = _userRepository.GetAll().FirstOrDefault(x => x.Login == username && x.Password == password);
    //        return user == null? false : true;
    //    }

    //}


    public partial class MainWindow : Window
    {
        private TcpClient tcpClient;
        private string serverIpAddress = "127.0.0.1";
        private int serverPort = 12345;
        public MainWindow()
        {
            InitializeComponent();

        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                using (tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(serverIpAddress, serverPort);
                    using NetworkStream stream = tcpClient.GetStream();
                    NetworkData request = new NetworkData
                    {
                        MessageType = "Login",
                        Data = new string[] { username, password } // Ваші значення логіну та пароля
                    };
                    string requestJson = JsonConvert.SerializeObject(request);
                    byte[] requestBuffer = Encoding.UTF8.GetBytes(requestJson);
                    stream.Write(requestBuffer, 0, requestBuffer.Length);



                    byte[] responseBuffer = new byte[1024];
                    int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                    string responseJson = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

                    NetworkData response = JsonConvert.DeserializeObject<NetworkData>(responseJson);

                    if (response.MessageType == "LoginResponse")
                    {
                        if (response.Data != null)
                        {
                            User user = JsonConvert.DeserializeObject<User>(response.Data.ToString());

                            Account window = new(user);
                            window.Show();
                            Close();
                        }
                        else
                        {
                            ErrorMessageText.Text = "Check your credentials and try again!";
                        }
                    }

                }
            }
            catch (Exception)
            {
                ErrorMessageText.Text = "Server is not available at the moment";
            }
        }


    }
}
