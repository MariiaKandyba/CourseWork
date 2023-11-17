using Client.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using Microsoft.VisualBasic.ApplicationServices;
using NetworkDataDll;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels
{
    public class AssigmentViewModel : ObservableObject
    {

        private TcpClient tcpClient;
        private string serverIpAddress = "127.0.0.1";
        private int serverPort = 12345;
        private ObservableCollection<TestResults> _tests = new();
        public ObservableCollection<TestResults> Tests
        {
            get { return _tests; }
            set { SetProperty(ref _tests, value); }
        }
        private TestResults _selectedTest;
        public TestResults SelectedTest
        {
            get { return _selectedTest; }
            set { SetProperty(ref _selectedTest, value); }
        }

        int userId;
        public AssigmentViewModel(List<TestResults> tests, int userId)
        {
            _tests = new ObservableCollection<TestResults>(tests);
            this.userId = userId;
            StartTestCommand = new RelayCommand(OnStartTestClick);
        }

        private async void OnStartTestClick()
        {


            AssignmentTest passedTest = new(SelectedTest, userId);
            passedTest.ShowDialog();

          
           



        }
        private void OnConfirmAssignmentCommandClick()
        {


        }


        private void OnAssignToUsersClick()
        {



        }






        public IRelayCommand StartTestCommand { get; }
        public IRelayCommand AssignToUsersCommand { get; }
        public IRelayCommand ConfirmAssignmentCommand { get; }



    }
}

