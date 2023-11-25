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
                
                _assigmentViewModel = new AssigmentViewModel((await UpdateHelper.GetTests(_user))[0], _user);
                _historyViewModel = new HistoryViewModel((await UpdateHelper.GetTests(_user))[1], _user);
                assignedTestTab.DataContext = _assigmentViewModel;
                HistoryTab.DataContext = _historyViewModel;
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
                    try
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
                    catch (Exception)
                    {

                    }
                    



                }
            }
            catch (Exception)
            {
                throw;
            }

        }

       
    }
}
