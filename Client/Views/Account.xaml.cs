using Client.ViewModels;
using DALTest.Entities;
using Microsoft.Extensions.Options;
using NetworkDataDll;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
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
using Test = DALTest.Entities.Test;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        AssigmentViewModel _assigmentViewModel;
        HistoryViewModel _historyViewModel;
        private TcpClient tcpClient;
        private string serverIpAddress = "127.0.0.1";
        private int serverPort = 12345;
        private readonly User _user;

        public Account(User user)
        {
            InitializeComponent();
            _user = user;
            string userInfo =
                 $"{_user.FirstName} {_user.LastName}\n" +
                 $"Логін: {_user.Login}\n" +
                 $"Опис: {_user.Description}\n" +
                 $"Дата реєстрації: {_user.RegisterDate}";

            infoLbl.Content = userInfo;

            GetTests();

        }
        public async void GetTests()
        {
            try
            {
                using (tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(serverIpAddress, serverPort);
                    using NetworkStream stream = tcpClient.GetStream();
                    NetworkData request = new() 
                    {
                        MessageType = "TestList",
                        Data = _user.Id
                    };
                    string requestJson = JsonConvert.SerializeObject(request);
                    byte[] requestBuffer = Encoding.UTF8.GetBytes(requestJson);
                    stream.Write(requestBuffer, 0, requestBuffer.Length);



                    byte[] responseBuffer = new byte[50000192];
                    int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                    string responseJson = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);


                    NetworkData response = JsonConvert.DeserializeObject<NetworkData>(responseJson);

                    if (response.MessageType == "TestListResponse" && (response.Data != null))
                    {

                        List<List<TestResults>> tests = JsonConvert.DeserializeObject<List<List<TestResults>>>(response.Data.ToString());
                        _assigmentViewModel = new AssigmentViewModel(tests[0], _user.Id);
                        _historyViewModel = new HistoryViewModel(tests[1]);
                        assignedTestTab.DataContext = _assigmentViewModel;
                        HistoryTab.DataContext = _historyViewModel;
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new();
            main.Show();
            Close();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                using (tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(serverIpAddress, serverPort);
                    using NetworkStream stream = tcpClient.GetStream();
                    NetworkData request = new()
                    {
                        MessageType = "ClientDisconnect",
                        Data = _user
                    };
                    string requestJson = JsonConvert.SerializeObject(request);
                    byte[] requestBuffer = Encoding.UTF8.GetBytes(requestJson);
                    await stream.WriteAsync(requestBuffer);



                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
