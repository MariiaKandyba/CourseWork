using Client.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest.Entities;
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


namespace Client.ViewModels
{
    //public class HistoryViewModel : ObservableObject
    //{

    //    #region TestProps
    //    private readonly IGenericRepository<Test> _testRepository;
    //    private ObservableCollection<Test> _tests = new();
    //    private Test _selectedTest;
    //    public Test SelectedTest
    //    {
    //        get { return _selectedTest; }
    //        set { SetProperty(ref _selectedTest, value); }
    //    }

    //    public ObservableCollection<Test> Tests
    //    {
    //        get { return _tests; }
    //        set { SetProperty(ref _tests, value); }
    //    }
    //    #endregion

    //    #region UserTestProps
    //    private readonly IGenericRepository<UserTest> _userTestRepository;
    //    private ObservableCollection<UserTest> _userTests = new();
    //    public ObservableCollection<UserTest> UserTests 
    //    {
    //        get { return _userTests; }
    //        set { SetProperty(ref _userTests, value); }
    //    }
    //    #endregion

    //    private readonly IGenericRepository<UserAnswer> _userAnswerRepository;

    //    User me;
    //    public HistoryViewModel(User user, GenericUnitOfWork _unitOfWork)
    //    {
    //        _testRepository = _unitOfWork.Repository<Test>();
    //        _userTestRepository = _unitOfWork.Repository<UserTest>();
    //        _userAnswerRepository = _unitOfWork.Repository<UserAnswer>();

    //        me = user;
    //        UserTests = new ObservableCollection<UserTest>(_userTestRepository.FindAll(x => x.UserId == user.Id && x.IsTaken));
    //        List<int> testId = new();

    //        foreach (var item in UserTests)
    //            testId.Add(item.TestId);

    //        Tests = new ObservableCollection<Test>(_testRepository.FindAll(x => testId.Contains(x.Id)));

    //        GetInfoCommand = new RelayCommand(OnGetInfoClick);
    //        AssignToUsersCommand = new RelayCommand(OnAssignToUsersClick);
    //        //ConfirmAssignmentCommand = new RelayCommand(OnConfirmAssignmentCommandClick);
    //    }


    //    private void OnGetInfoClick()
    //    {

    //        var testToCheck = UserTests.FirstOrDefault(x => x.TestId == SelectedTest.Id);
    //        TestResult testResult = new()
    //        {
    //            Test = SelectedTest,
    //            UserAnswers = (List<UserAnswer>)_userAnswerRepository.FindAll(x => x.UserTestId == testToCheck.Id),
    //        };
    //        //foreach (var item in SelectedTest.Questions)
    //        //{
    //        //    MessageBox.Show(item.QuestionText);
    //        //    foreach (var ritem in item.Answers)
    //        //    {
    //        //        MessageBox.Show(ritem.AnswerText);

    //        //    }

    //        //}
    //        //foreach (var item in _userAnswerRepository.FindAll(x => x.UserTestId == testToCheck.Id))
    //        //{
    //        //    MessageBox.Show(item.Answer.AnswerText);
    //        //}

    //        PassedTestInfo testInfo = new(testResult);
    //        testInfo.ShowDialog();

    //    }

    //    private void OnAssignToGroupClick()
    //    {

    //    }
    //    private void OnAssignToUsersClick()
    //    {



    //    }

    //    //private List<int> AssignedUsersId()
    //    //    => UserTests.Where(x => x.TestId == SelectedTest.Id)
    //    //         .Select(x => x.UserId)
    //    //         .ToList();









    //    public IRelayCommand GetInfoCommand { get; }
    //    public IRelayCommand AssignToUsersCommand { get; }
    //    public IRelayCommand ConfirmAssignmentCommand { get; }



    //}


    public class HistoryViewModel : ObservableObject
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


        public HistoryViewModel(List<TestResults> tests)
        {
            _tests = new ObservableCollection<TestResults>(tests);
            GetInfoCommand = new RelayCommand(OnGetInfoClick);


        }

        private async void OnGetInfoClick()
        {
            PassedTestInfo passedTestInfo = new(SelectedTest);
            passedTestInfo.ShowDialog();

            //string outputString = string.Join(Environment.NewLine, takenToSend.Select(testResult =>
            //{
            //    string testInfo = $"Title: {testResult.Title}\n" +
            //                      $"Possible points: {testResult.TotalPossiblePoints}\n" +
            //                      $"Your Grade: {testResult.PointsGrade}\n" +
            //                      $"Author: {testResult.Author}\n" +
            //                      $"Description: {testResult.Description}\n" +
            //                      $"Info: {testResult.Info}\n" +
            //                      $"Pass Percent: {testResult.PassPercent}\n" +
            //                      $"Loaded Date: {testResult.LoadedDate}\n" +
            //                      $"Is Passed: {testResult.IsPassed}\n" +
            //                      $"Taken Date: {testResult.TakenDate}\n" +
            //                      $"Is Taken: {testResult.IsTaken}\n" +
            //                      "-------------\n";

            //    string questionsInfo = string.Join(Environment.NewLine, testResult.Questions.Select(question =>
            //    {
            //        string questionInfo = $"  Question Text: {question.QuestionText}\n" +
            //                              $"  Img: {question.Img}\n";

            //        string answersInfo = string.Join(Environment.NewLine, question.Answers.Select(answer =>
            //            $"    Answer Text: {answer.AnswerText}\n" +
            //            $"    Is Checked: {answer.IsChecked}\n"));

            //        return $"{questionInfo} " +
            //        $" Answers:\n{answersInfo}\n";
            //    }));

            //    return $"{testInfo}Questions:\n{questionsInfo}\n";
            //}));
            //string check = outputString;
            //MessageBox.Show(outputString);

            //string outputString = string.Join(Environment.NewLine, takenToSend.Select(test =>
            //    $"Test ID: {test.Id}\n" +
            //    $"Title: {test.Title}\n" +
            //    $"Author: {test.Author}\n" +
            //    $"Description: {test.Description}\n" +
            //    $"Info: {test.Info}\n" +
            //    $"Pass Percent: {test.PassPercent}\n" +
            //    $"Loaded Date: {test.LoadedDate}\n" +
            //    $"Points Grade: {test.PointsGrade}\n" +
            //    $"Is Passed: {test.IsPassed}\n" +
            //    $"Taken Date: {test.TakenDate}\n" +
            //    $"Is Taken: {test.IsTaken}\n" +
            //    "-------------"));

            //MessageBox.Show(outputString);

        }

        public IRelayCommand GetInfoCommand { get; }

    }
}
