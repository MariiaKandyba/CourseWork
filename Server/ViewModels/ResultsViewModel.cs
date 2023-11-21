using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetworkDataDll;
using Server.Views.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ViewModels
{

    public class ResultsViewModel : ObservableObject
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


        public ResultsViewModel(List<TestResults> tests)
        {
            Tests = new ObservableCollection<TestResults>(tests);
            GetInfoCommand = new RelayCommand(OnGetInfoClick);
        }

        private void OnGetInfoClick()
        {
            DetailedResultWindow window = new(SelectedTest);
            window.Show();

        }

        public IRelayCommand GetInfoCommand { get; }

    }
}
