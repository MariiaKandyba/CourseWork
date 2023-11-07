using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.ViewModels
{
    public class ServerViewModel : ObservableObject
    {
        private TcpListener tcpListener;

        public ObservableCollection<string> ConnectedClients { get; } = new ObservableCollection<string>();

        public ServerViewModel()
        {
            StartServerCommand = new RelayCommand(OnStartServerClick);
            StopServerCommand = new RelayCommand(OnStopServerClick);
        }

        private async void OnStartServerClick()
        {
            int port = 12345; // Ваш порт
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();

                try
                {
                    TcpClient client = await tcpListener.AcceptTcpClientAsync();
                    string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    ConnectedClients.Add(clientIpAddress);
                    client.Close();
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception)
            {
            }
           

        }

        private void OnStopServerClick()
        {
            tcpListener?.Stop();
        }


        public IRelayCommand StartServerCommand { get; }
        public IRelayCommand StopServerCommand { get; }

    }
}
