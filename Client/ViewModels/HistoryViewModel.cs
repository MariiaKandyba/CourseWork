using Client.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using Microsoft.VisualBasic.ApplicationServices;
using NetworkDataDll;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using User = DALTest.Entities.User;


namespace Client.ViewModels
{
    public class HistoryViewModel : ObservableObject
    {
        private User _user;
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


        public HistoryViewModel(List<TestResults> tests, User user)
        {
            _user = user;
            _tests = new ObservableCollection<TestResults>(tests);
            GetInfoCommand = new RelayCommand(OnGetInfoClick);
            UpdateCommand = new RelayCommand(OnUptadeClick);
        }

        private async void OnUptadeClick()
        {
            try
            {
                List<List<TestResults>> tests = await UpdateHelper.GetTests(_user);
                if(tests != null)
                    Tests = new ObservableCollection<TestResults>(tests[1]);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private async void OnGetInfoClick()
        {
            if(SelectedTest != null)
            {
                PassedTestInfo passedTestInfo = new(SelectedTest);
                passedTestInfo.ShowDialog();
            }
           
        }

        
        public IRelayCommand GetInfoCommand { get; }
        public IRelayCommand UpdateCommand { get; }

    }
}
