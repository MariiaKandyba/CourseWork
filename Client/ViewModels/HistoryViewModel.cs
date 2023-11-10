using Client.Models;
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
        private Test _selectedTest;
        public Test SelectedTest
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

            try
            {
                using (tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(serverIpAddress, serverPort);
                    using NetworkStream stream = tcpClient.GetStream();
                    NetworkData request = new()
                    {
                        MessageType = "TaskenQuestions",
                        Data = SelectedTest.Id
                    };
                    string requestJson = JsonConvert.SerializeObject(request);
                    byte[] requestBuffer = Encoding.UTF8.GetBytes(requestJson);
                    stream.Write(requestBuffer, 0, requestBuffer.Length);



                    byte[] responseBuffer = new byte[2500];
                    int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                    string responseJson = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);


                    NetworkData response = JsonConvert.DeserializeObject<NetworkData>(responseJson);

                    if (response.MessageType == "QuestionsResponse" && response.Data != null)
                    {
                        var questionAnswerPairs = JsonConvert.DeserializeObject<List<TestResult>>(response.Data.ToString());
                                             //var resultString = ""; // Рядок для накопичення всіх даних

                        //foreach (var pair in questionAnswerPairs)
                        //{
                        //    var questionData = pair["Question"];
                        //    var answerData = pair["Answers"];
                        //    var userAnswerData = pair["UserAnswers"];
                        //    var question = new Question
                        //    {
                        //        Id = questionData.Value<int>("Id"),
                        //        QuestionText = questionData.Value<string>("QuestionText"),
                        //        Points = questionData.Value<int>("Points"),
                        //        Img = questionData.Value<string>("Img"),
                        //        Answers = answerData
                        //            .Select(answer => new Answer
                        //            {
                        //                Id = answer.Value<int>("Id"),
                        //                AnswerText = answer.Value<string>("AnswerText")
                        //            })
                        //            .ToList()
                        //    };

                        //    // Додаємо дані питання та відповідей до рядка
                        //    resultString += $"Питання: {question.QuestionText}\n";
                        //    foreach (var answer in question.Answers)
                        //    {
                        //        resultString += $"- {answer.AnswerText}\n";
                        //        // Перевіряємо, чи відповідь користувача попадається в циклі
                        //        if (userAnswerData.Any(userAnswer => userAnswer.Value<int>("AnswerId") == answer.Id))
                        //        {
                        //            resultString += "  (ви відповіли)\n";
                        //        }
                        //    }

                        //    resultString += "\n"; // Порожній рядок для розділення питань
                        //}

                        //// Результат містить всі питання та відповіді в потрібному форматі
                        //MessageBox.Show(resultString); // Виведення результату на консоль
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public IRelayCommand GetInfoCommand { get; }

    }
}
