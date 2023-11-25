using Client.ViewModels;
using DALTest.Entities;
using System.Security.Cryptography;
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
using System.Net.Security;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
                        Data = new string[] { username, HashPassword(password) } 
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
                            NetworkData sendLog = new NetworkData
                            {
                                MessageType = "NewClient",
                                Data = user 
                            };
                            string sendLogJson = JsonConvert.SerializeObject(sendLog);
                            byte[] sendLogBuffer = Encoding.UTF8.GetBytes(sendLogJson);
                            stream.Write(sendLogBuffer, 0, sendLogBuffer.Length);


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
