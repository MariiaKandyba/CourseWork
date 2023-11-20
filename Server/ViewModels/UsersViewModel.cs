using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using NetworkDataDll;
using Repository;
using Server.Views;
using Server.Views.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.ViewModels
{
    public class UsersViewModel : ObservableObject
    {
        private readonly IGenericRepository<User> _userRepository;

        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private User _selectedUser;

        private readonly IGenericRepository<UserTest> _userTestRepository;
        private UserTest _selectedUserTest;
        public UserTest SelectedUserTest
        {
            get { return _selectedUserTest; }
            set { SetProperty(ref _selectedUserTest, value); }
        }
        private ObservableCollection<UserTest> _userTests = new();
        public ObservableCollection<UserTest> UserTests // Властивість для користувачів
        {
            get { return _userTests; }
            set { SetProperty(ref _userTests, value); }
        }

        RepositoryFilter _repoWork;

        public UsersViewModel(IGenericRepository<User> userRepository, IGenericRepository<UserTest> userTestRepository, RepositoryFilter repoWork)
        {
            _userRepository = userRepository;
            _userTestRepository = userTestRepository;
            _repoWork = repoWork;
            LoadActualList();

            AddUserCommand = new RelayCommand<object>(OnAddUserClick);
            EditUserCommand = new RelayCommand<object>(OnEditUserClick);
            DeleteUserCommand = new RelayCommand(OnDeleteUserClick);
            RestoreUserCommand = new RelayCommand(OnRestoreUserClick);
            UpdateCommand = new RelayCommand(OnUpdateUserClick);
            SeeResultsCommand = new RelayCommand(OnResultsClick);
        }

        private void OnResultsClick()
        {
            List<TestResults> testResults = _repoWork.GetTestResults(SelectedUser.Id, true);
            UsersResultsWindow window = new(testResults);
            window.Show();
        }

        private void OnUpdateUserClick()
        {
            LoadActualList();
        }

        private void OnRestoreUserClick()
        {
            RestoreUserWindow window = new(_userRepository.FindAll(x => x.IsArchived).ToList());
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                foreach (var selectedItem in window.SelectedUsers)
                {
                    var userToUpdate = _userRepository.FindById(selectedItem.Id);
                    if (userToUpdate != null && userToUpdate.IsArchived)
                    {
                        userToUpdate.IsArchived = false;
                        _userRepository.Update(userToUpdate);
                    }
                }
                LoadActualList();

            }



        }

        private void LoadActualList()
        {
            Users = new ObservableCollection<User>(_userRepository.FindAll(x => !x.IsArchived));

        }
        private void OnDeleteUserClick()
        {
            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SelectedUser.IsArchived = true;
                _userRepository.Update(SelectedUser);
                LoadActualList();

            }

        }

        private void OnEditUserClick(object? obj)
        {
            CreateEditUser window = new(SelectedUser);
            
            window.ShowDialog();
            if (window.DialogResult ?? false)
            {
                _userRepository.Update(window.User);
                LoadActualList();
            }
        }

        private void OnAddUserClick(object? obj)
        {
            CreateEditUser window = new();
            window.ShowDialog();
            if (window.DialogResult ?? false)
            {
                _userRepository.Add(window.User);
                LoadActualList();
            }

        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
        public IRelayCommand<object> AddUserCommand { get; }
        public IRelayCommand<object> EditUserCommand { get; }
        public IRelayCommand DeleteUserCommand { get; }
        public IRelayCommand RestoreUserCommand { get; }
        public IRelayCommand UpdateCommand { get; }
        public IRelayCommand SeeResultsCommand { get; }



    }
    public class RepositoryFilter
    {
        private readonly IGenericRepository<Test> _testRepository;
        private readonly IGenericRepository<UserTest> _userTestRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
        private readonly IGenericRepository<UserAnswer> _userAnswerRepository;

        public RepositoryFilter(
            IGenericRepository<Test> testRepository,
            IGenericRepository<UserTest> userTestRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Answer> answerRepository,
            IGenericRepository<UserAnswer> userAnswerRepository)
        {
            _testRepository = testRepository;
            _userTestRepository = userTestRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userAnswerRepository = userAnswerRepository;
        }

        public List<TestResults> GetTestResults(int id, bool isTaken)
        {
            var takenTests = _userTestRepository
                        .FindAll(x => x.UserId == id && x.IsTaken == isTaken)
                        .Select(ut => new
                        {
                            ut.Id,
                            ut.TestId,
                            ut.PointsGrade,
                            ut.IsPassed,
                            ut.TakenDate,
                            ut.IsTaken
                        })
                        .ToList();
            List<Test> taken = _testRepository.FindAll(test => takenTests.Select(x => x.TestId).Contains(test.Id) && !test.IsArchived).ToList();


            var questions = _questionRepository.FindAll(x => taken.Select(x => x.Id).Contains(x.TestId));

            var userAnswers = _userAnswerRepository
                .FindAll(x => takenTests
                .Select(test => test.Id)
                .Contains(x.UserTestId))
                .ToList();
            var answers = _answerRepository.FindAll(a => questions.Select(a => a.Id).Contains(a.QuestionId));
            List<AnswerModel> answerModels = answers
                .Select(value => new AnswerModel
                {
                    Id = value.Id,
                    QuestionId = value.QuestionId,
                    AnswerText = value.AnswerText,
                    IsChecked = isTaken && userAnswers.Any(ua => ua.AnswerId == value.Id)

                }).ToList();



            List<Answer> correctAnswers = answers.Where(x => x.IsRight).ToList();


            List<QuestionModel> selectedData = questions.Select(value => new QuestionModel
            {
                QuestionText = value.QuestionText,
                Id = value.Id,
                TestId = value.TestId,
                Img = value.Img,
                Answers = answerModels.Where(x => x.QuestionId == value.Id).ToList(),
            }).ToList();



            List<TestResults> takenToSend = taken
                .Select(value => new TestResults
                {
                    Id = value.Id,
                    Title = value.Title,
                    Author = value.Author,
                    Description = value.Description,
                    Info = value.Info,
                    PassPercent = value.PassPercent,
                    LoadedDate = value.LoadedDate,
                    TotalPossiblePoints = value.Questions.Sum(x => x.Points),
                    PointsGrade = isTaken ? takenTests.FirstOrDefault(x => x.TestId == value.Id)?.PointsGrade ?? 0 : 0,
                    IsPassed = isTaken ? takenTests.FirstOrDefault(x => x.TestId == value.Id)?.IsPassed ?? false : false,
                    TakenDate = isTaken ? takenTests.FirstOrDefault(x => x.TestId == value.Id)?.TakenDate ?? DateTime.MinValue : DateTime.MinValue,
                    IsTaken = isTaken,
                    Questions = selectedData.Where(x => x.TestId == value.Id).ToList(),

                }).ToList();

            if (isTaken)
            {
                foreach (var item in takenToSend)
                {
                    item.ScoredPercent = item.PointsGrade / (double)item.TotalPossiblePoints * 100;
                }
            }
            return takenToSend;
        }

        public double CalculateGrade(List<UserAnswer> userAnswers, int testId)
        {
            var testQuestion = _questionRepository.FindAll(x => x.TestId == testId);
            var UsersAnswersIds = userAnswers.Select(x => x.AnswerId);

            var userPoints = UsersAnswersIds.Sum(answerId => testQuestion.FirstOrDefault(question => question.Answers.Any(answer => answer.Id == answerId && answer.IsRight))?.Points ?? 0);
            return userPoints;

        }

        public bool IsPassed(int testId, double points)
        {
            var test = _testRepository.FindById(testId);
            double PassPercent = test.PassPercent;

            var TotalPossiblePoints = test.Questions.Sum(x => x.Points);

            var ScoredPercent = points / TotalPossiblePoints * 100;

            if (ScoredPercent < PassPercent) return false;
            else return true;


        }

        public TestResults GetTestResultsToShow(TestResults gottenTest, UserTest userTest, List<UserAnswer> userAnswer)
        {
            var test = _testRepository.FindById(userTest.TestId);
            double PassPercent = test.PassPercent;

            var TotalPossiblePoints = test.Questions.Sum(x => x.Points);

            var ScoredPercent = userTest.PointsGrade / TotalPossiblePoints * 100;


            gottenTest.PointsGrade = userTest.PointsGrade;
            gottenTest.IsPassed = userTest.IsPassed;
            gottenTest.ScoredPercent = ScoredPercent;
            gottenTest.TakenDate = userTest.TakenDate;
            gottenTest.IsTaken = userTest.IsTaken;
            gottenTest.TotalPossiblePoints = TotalPossiblePoints;

            foreach (var item in gottenTest.Questions)
            {
                foreach (var answer in item.Answers)
                {
                    var correspondingUserAnswer = userAnswer.FirstOrDefault(ua => ua.AnswerId == answer.Id);

                    if (correspondingUserAnswer != null)
                    {
                        answer.IsChecked = correspondingUserAnswer.IsChecked;
                    }
                }
            }
            return gottenTest;

        }

    }
}
