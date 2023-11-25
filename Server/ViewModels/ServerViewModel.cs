using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest;
using DALTest.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.Logging;
using NetworkDataDll;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository;
using Server.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestServices;
using static System.Net.Mime.MediaTypeNames;
using Answer = DALTest.Entities.Answer;
using Application = System.Windows.Application;
using Question = DALTest.Entities.Question;
using Test = DALTest.Entities.Test;

namespace Server.ViewModels
{

    public class ServerViewModel : ObservableObject
    {
        
        public ObservableCollection<string> ConnectedClients { get; } = new ObservableCollection<string>();

        public void AddConnectedClient(ClientHandler clientHandler)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ConnectedClients.Add($"{clientHandler.currentUser.Id}:{clientHandler.currentUser.Login}");
            });
        }

        public void RemoveClientHandler(ClientHandler clientHandler)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ConnectedClients.Remove($"{clientHandler.currentUser.Id}:{clientHandler.currentUser.Login}");


            });
        }


        public ServerViewModel()
        {
            StartServerCommand = new AsyncRelayCommand(OnStartServerClick);
            StopServerCommand = new RelayCommand(OnStopServerClick);
        }


        private TcpListener _tcpListener;


        private async Task OnStartServerClick()
            {
                int port = 12345;
                try
                {
                _tcpListener = new TcpListener(IPAddress.Any, port);
                _tcpListener.Start();

                    while (true) 
                    {
                        TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                        ClientHandler clientHandler = new(client, this);
                        await Task.Run(async () => await clientHandler.HandleClient(client));
                    }
                } catch (Exception) { }
        }
        
        private void OnStopServerClick()
        {
            _tcpListener?.Stop();
            Application.Current.Dispatcher.Invoke(() =>
            {
                ConnectedClients.Clear();


            });
        }


        public IRelayCommand StartServerCommand { get; }
        public IRelayCommand StopServerCommand { get; }

    }
}
