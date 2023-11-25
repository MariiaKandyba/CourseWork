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
using User = DALTest.Entities.User;

namespace Client.ViewModels
{
    public class AssigmentViewModel : ObservableObject
    {

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

        User _user;
        public AssigmentViewModel(List<TestResults> tests, User user)
        {
            _tests = new ObservableCollection<TestResults>(tests);
            _user = user;
            StartTestCommand = new RelayCommand(OnStartTestClick);
            UpdateCommand = new RelayCommand(OnUpadateClick);
        }

        private async void OnUpadateClick()
        {
            try
            {
                List<List<TestResults>> tests = await UpdateHelper.GetTests(_user);
                if (tests != null)
                    Tests = new ObservableCollection<TestResults>(tests[0]);
            }
            catch (Exception)
            {
            }
          
        }
        private async void OnStartTestClick()
        {
            if(SelectedTest != null)
            {
                AssignmentTest passedTest = new(SelectedTest, _user.Id);
                passedTest.ShowDialog();
            }
            

        }



        public IRelayCommand StartTestCommand { get; }
        public IRelayCommand UpdateCommand { get; }

    }
}

