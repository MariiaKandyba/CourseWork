using Client.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
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
        private string _serverState;
        public string ServerState
        {
            get { return _serverState; }
            set { SetProperty(ref _serverState, value); }
        }

        #region TestProps
        private readonly IGenericRepository<Test> _testRepository;
        private ObservableCollection<Test> _tests = new();
        private Test _selectedTest;
        public Test SelectedTest
        {
            get { return _selectedTest; }
            set { SetProperty(ref _selectedTest, value); }
        }

        public ObservableCollection<Test> Tests
        {
            get { return _tests; }
            set { SetProperty(ref _tests, value); }
        }
        #endregion

        #region UserTestProps
        private readonly IGenericRepository<UserTest> _userTestRepository;
        private ObservableCollection<UserTest> _userTests = new();
        public ObservableCollection<UserTest> UserTests 
        {
            get { return _userTests; }
            set { SetProperty(ref _userTests, value); }
        }
        #endregion

        IGenericRepository<Question> _questionRepository;
        public AssigmentViewModel(User user, GenericUnitOfWork _unitOfWork)
        {
            _testRepository = _unitOfWork.Repository<Test>();
            _userTestRepository = _unitOfWork.Repository<UserTest>();
            _questionRepository = _unitOfWork.Repository<Question>();
            UserTests = new ObservableCollection<UserTest>(_userTestRepository.FindAll(x => x.UserId == user.Id && !x.IsTaken));
            List<int> testId = new();

            foreach (var item in UserTests)
                testId.Add(item.TestId);

            Tests = new ObservableCollection<Test>(_testRepository.FindAll(x => testId.Contains(x.Id)));

            StartTestCommand = new RelayCommand(OnStartTestClick);
            AssignToUsersCommand = new RelayCommand(OnAssignToUsersClick);
            ConfirmAssignmentCommand = new RelayCommand(OnConfirmAssignmentCommandClick);
        }

        private void OnStartTestClick()
        {

            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(serverIpAddress, serverPort);

                ServerState = "Connected";

                UserTest test = UserTests.FirstOrDefault(x => x.TestId == SelectedTest.Id);
                TestView window = new(SelectedTest.Questions.ToList());
                window.ShowDialog();
                //string a = string.Empty;

                //foreach (var item in window.UserAnswers)
                //{
                //    //a += "UserTestId: " + test.Id + " ";
                //    //a += "IsChecked: " + item.IsChecked.ToString() + ' ';
                //    //a += "AnswerId: " + item.AnswerId + Environment.NewLine;
                //}
                //MessageBox.Show(a);


            }
            catch (Exception)
            {
                ServerState = "Test will be available at the assigned time.";
            }
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
