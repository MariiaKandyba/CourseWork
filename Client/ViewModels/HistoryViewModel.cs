using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class HistoryViewModel : ObservableObject
    {

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
        public ObservableCollection<UserTest> UserTests // Властивість для користувачів
        {
            get { return _userTests; }
            set { SetProperty(ref _userTests, value); }
        }
        #endregion


        public HistoryViewModel(User user, IGenericRepository<UserTest> userTestRepository, IGenericRepository<Test> testRepository)
        {
            _testRepository = testRepository;
            _userTestRepository = userTestRepository;

            UserTests = new ObservableCollection<UserTest>(_userTestRepository.FindAll(x => x.UserId == user.Id && x.IsTaken));
            List<int> testId = new();

            foreach (var item in UserTests)
                testId.Add(item.TestId);

            Tests = new ObservableCollection<Test>(_testRepository.FindAll(x => testId.Contains(x.Id)));

            AssignToGroupCommand = new RelayCommand(OnAssignToGroupClick);
            AssignToUsersCommand = new RelayCommand(OnAssignToUsersClick);
            ConfirmAssignmentCommand = new RelayCommand(OnConfirmAssignmentCommandClick);
        }


        private void OnConfirmAssignmentCommandClick()
        {


        }

        private void OnAssignToGroupClick()
        {

        }
        private void OnAssignToUsersClick()
        {



        }

        private List<int> AssignedUsersId()
            => UserTests.Where(x => x.TestId == SelectedTest.Id)
                 .Select(x => x.UserId)
                 .ToList();









        public IRelayCommand AssignToGroupCommand { get; }
        public IRelayCommand AssignToUsersCommand { get; }
        public IRelayCommand ConfirmAssignmentCommand { get; }



    }
}
