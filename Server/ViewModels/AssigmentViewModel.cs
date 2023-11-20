using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Repository;
using Server.Views;
using Server.Views.Tests;
using Server.Views.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TestServices;
using Answer = DALTest.Entities.Answer;
using Question = DALTest.Entities.Question;
using Test = DALTest.Entities.Test;

namespace Server.ViewModels
{

    public class AssigmentViewModel : ObservableObject
    {
        #region AdditionalProps
        private Visibility _groupsVisibility = Visibility.Collapsed;

        public Visibility GroupsVisibility
        {
            get { return _groupsVisibility; }
            set { SetProperty(ref _groupsVisibility, value); }

        }

        private Visibility _usersVisibility = Visibility.Collapsed;

        public Visibility UsersVisibility
        {
            get { return _usersVisibility; }
            set { SetProperty(ref _usersVisibility, value); }
        }

        #endregion

        #region TestProps
        private readonly IGenericRepository<Test> _testRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
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

        #region GroupProps
        private readonly IGenericRepository<Group> _groupRepository;
        private ObservableCollection<Group> _groups = new();
        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get { return _selectedGroup; }
            set { SetProperty(ref _selectedGroup, value); }
        }

        public ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set { SetProperty(ref _groups, value); }
        }
        #endregion

        #region UserProps
        private readonly IGenericRepository<User> _userRepository;
        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }
        private ObservableCollection<User> _users = new();
        public ObservableCollection<User> Users // Властивість для користувачів
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
        #endregion


        #region UserTestProps
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
        #endregion

        public AssigmentViewModel(IGenericRepository<UserTest> userTestRepository, 
            IGenericRepository<Test> testRepository,  IGenericRepository<Group> groupRepository, 
            IGenericRepository<User> userRepository, IGenericRepository<Question> questionRepository, IGenericRepository<Answer> answerRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _testRepository = testRepository;
            _userTestRepository = userTestRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;


            LoadData();

            AssignToGroupCommand = new RelayCommand(OnAssignToGroupClick);
            AssignToUsersCommand = new RelayCommand(OnAssignToUsersClick);
            ConfirmAssignmentCommand = new RelayCommand(OnConfirmAssignmentCommandClick);
            InfoAssignmentCommand = new RelayCommand(OnInfoAssignmentClick);
            UploadTestCommand = new RelayCommand(OnUploadTestClick);
            PreviewTestCommand = new RelayCommand(OnPreviewClcik);
            DeleteTestCommand = new RelayCommand(OnDeleteTestClcik);
            UpdateTestCommand = new RelayCommand(LoadData);
            RestoreTestCommand = new RelayCommand(OnRestoreTestClick);
        }

        private void OnRestoreTestClick()
        {
            RestoreTest window = new(_testRepository.FindAll(x => x.IsArchived).ToList());
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                foreach (var selectedItem in window.SelectedTests)
                {
                    var testToUpdate = _testRepository.FindById(selectedItem.Id);
                    if (testToUpdate != null && testToUpdate.IsArchived)
                    {
                        testToUpdate.IsArchived = false;
                        _testRepository.Update(testToUpdate);
                    }
                }
                LoadData();
            }
        }

        private void LoadData()
        {
            Groups = new ObservableCollection<Group>(_groupRepository.GetAll());
            Users = new ObservableCollection<User>(_userRepository.FindAll(x => !x.IsArchived));
            Tests = new ObservableCollection<Test>(_testRepository.FindAll(x => !x.IsArchived));
            UserTests = new ObservableCollection<UserTest>(_userTestRepository.GetAll());

        }
        private void OnDeleteTestClcik()
        {
            SelectedTest.IsArchived = true;
            _testRepository.Update(SelectedTest);
            LoadData();
        }

        private void OnPreviewClcik()
        {
            TestPreview test = new(SelectedTest);
            test.Show();
        }

        private async void OnUploadTestClick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Select an XML file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FileService fileService = new();
                string xmlContent = await fileService.LoadFileAsync(openFileDialog.FileName);
                SerializationService service = new();
                TestServices.Test test = service.DeserializeObjectFromXml<TestServices.Test>(xmlContent);
                var testDb = new Test
                {
                    Title = test.Title,
                    Author = test.Author,
                    Description = test.Description,
                    Info = test.Info,
                    PassPercent = Convert.ToInt32(test.PassPercent),
                    IsArchived = false,
                    LoadedDate = DateTime.Now,

                };

                _testRepository.Add(testDb);


                foreach (var question in test.Questions.Question)
                {
                    var newQuestion = new Question
                    {
                        QuestionText = question.QuestionText,
                        Img = question.Img,
                        Points = Convert.ToInt32(question.Points),
                        Test = testDb
                    };

                    _questionRepository.Add(newQuestion);

                    foreach (var answer in question.Answers.Answer)
                    {
                        var newAnswer = new Answer
                        {
                            AnswerText = answer.TextAnswer,
                            IsRight = Convert.ToBoolean(answer.IsRight),
                            Question = newQuestion
                        };
                        _answerRepository.Add(newAnswer);
                    }

                }
                LoadData();
               
            }
           



        }


        private void OnInfoAssignmentClick()
        {
            LoadData();

            AssignedUsersAndGroups assigned = new(GetAssignedGroups().ToList(), GetAssignedUsers().ToList()) ;
            assigned.Show();
        }

        private void OnConfirmAssignmentCommandClick()
        {
            if(SelectedGroup != null && GroupsVisibility == Visibility.Visible)
            {
                foreach (var user in SelectedGroup.Users.Where(x => !AssignedUsersId().Contains(x.Id)))
                {
                    UserTest userTest = new() { IsPassed = false, IsTaken = false, PointsGrade = 0, TestId = SelectedTest.Id, UserId = user.Id, TakenDate = DateTime.Now };
                    _userTestRepository.Add(userTest);
                    Users.Remove(user);
                    Groups.Remove(SelectedGroup);

                }
            }

            if (SelectedUser != null && UsersVisibility == Visibility.Visible)
            {
                UserTest userTest = new() { IsPassed = false, IsTaken = false, PointsGrade = 0, TestId = SelectedTest.Id, UserId = SelectedUser.Id, TakenDate = DateTime.Now };
                _userTestRepository.Add(userTest);
                Users.Remove(SelectedUser);
                Users = new ObservableCollection<User>(Users.Where(user => !AssignedUsersId().Contains(user.Id)));
            }


        }

        private void OnAssignToGroupClick()
        {
            if (SelectedTest != null)
            {
                GroupsVisibility = Visibility.Visible;
                UsersVisibility = Visibility.Collapsed;
                Groups = GetUnassignedGroups();
            }

        }
        private void OnAssignToUsersClick()
        {
            if(SelectedTest != null)
            {
                GroupsVisibility = Visibility.Collapsed;
                UsersVisibility = Visibility.Visible;
                Users = GetUnassignedUsers() ;
            }


        }

        private List<int> AssignedUsersId() 
            => UserTests.Where(x => x.TestId == SelectedTest.Id)
                 .Select(x => x.UserId)
                 .ToList();








        public IRelayCommand AssignToGroupCommand { get; }
        public IRelayCommand AssignToUsersCommand { get; }
        public IRelayCommand ConfirmAssignmentCommand { get; }
        public IRelayCommand InfoAssignmentCommand { get; }
        public IRelayCommand UploadTestCommand { get; }
        public IRelayCommand PreviewTestCommand { get; }
        public IRelayCommand DeleteTestCommand { get; }
        public IRelayCommand UpdateTestCommand { get; }
        public IRelayCommand RestoreTestCommand { get; }




        private ObservableCollection<Group> GetUnassignedGroups()
        {
            return new ObservableCollection<Group>(_groupRepository.GetAll().Where(group => group.Users.Any(user => !AssignedUsersId().Contains(user.Id))));
        }

        private ObservableCollection<Group> GetAssignedGroups()
        {
            return new ObservableCollection<Group>(_groupRepository.GetAll().Where(group => group.Users.All(user => AssignedUsersId().Contains(user.Id))));
        }
        private ObservableCollection<User> GetUnassignedUsers()
        {
            return new ObservableCollection<User>(_userRepository.GetAll().Where(user => !AssignedUsersId().Contains(user.Id)));
        }

        private ObservableCollection<User> GetAssignedUsers()
        {
            return new ObservableCollection<User>(_userRepository.GetAll().Where(user => AssignedUsersId().Contains(user.Id)));
        }
    }




}
