using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest;
using DALTest.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetworkDataDll;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestServices;
using static System.Net.Mime.MediaTypeNames;
using Answer = DALTest.Entities.Answer;
using Question = DALTest.Entities.Question;
using Test = DALTest.Entities.Test;

namespace Server.ViewModels
{
    public class ServerViewModel : ObservableObject
    {
        private TcpListener tcpListener;

        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Test> _testRepository;
        private readonly IGenericRepository<UserTest> _userTestRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
        private readonly IGenericRepository<UserAnswer> _userAnswerRepository;


        User user;
        GenericUnitOfWork _unitOfWork;


        public ObservableCollection<string> ConnectedClients { get; } = new ObservableCollection<string>();

        public ServerViewModel()
        {
            StartServerCommand = new RelayCommand(OnStartServerClick);
            StopServerCommand = new RelayCommand(OnStopServerClick);

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            string conStr = builder.Build().GetConnectionString("DefaultConnection")!;

            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            var options = optionsBuilder.UseLazyLoadingProxies().UseSqlServer(conStr).Options;
            _unitOfWork = new GenericUnitOfWork(new Context(options));
            _userRepository = _unitOfWork.Repository<User>();
            _testRepository = _unitOfWork.Repository<Test>();
            _userTestRepository = _unitOfWork.Repository<UserTest>();
            _questionRepository = _unitOfWork.Repository<Question>();
            _answerRepository = _unitOfWork.Repository<Answer>();
            _userAnswerRepository = _unitOfWork.Repository<UserAnswer>();

        }

    //private async void OnStartServerClick()
    //{
    //    int port = 12345; // Ваш порт
    //    try
    //    {
    //        tcpListener = new TcpListener(IPAddress.Any, port);
    //        tcpListener.Start();

    //        try
    //        {
    //            TcpClient client = await tcpListener.AcceptTcpClientAsync();
    //            string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
    //            ConnectedClients.Add(clientIpAddress);
    //            client.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //    }
    //    catch (Exception)
    //    {
    //    }


    //}

    //private async void OnStartServerClick()
    //{
    //    int port = 12345; // Ваш порт
    //    try
    //    {
    //        tcpListener = new TcpListener(IPAddress.Any, port);
    //        tcpListener.Start();

    //        try
    //        {
    //            TcpClient client = await tcpListener.AcceptTcpClientAsync();
    //            string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

    //            NetworkStream stream = client.GetStream();
    //            byte[] buffer = new byte[1024];
    //            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
    //            string loginData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

    //            if (VerifyLoginData(loginData))
    //            {
    //                string response = "Login successful";
    //                byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
    //                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
    //            }
    //            else
    //            {
    //                string response = "Login failed";
    //                byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
    //                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
    //            }

    //            client.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //    }
    //    catch (Exception)
    //    {
    //    }
    //}

        private async void OnStartServerClick()
            {
                int port = 12345; // Ваш порт
                try
                {
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    tcpListener.Start();

                    while (true) 
                    {
                        //string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                        HandleClient(await tcpListener.AcceptTcpClientAsync()); 

                    }
                } catch (Exception) { }
        }
        User currentUser;
        private async void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                NetworkData request = JsonConvert.DeserializeObject<NetworkData>(requestJson);

                if (request.MessageType == "Login")
                {
                    string username = ((JArray)request.Data)[0].ToString();
                    string password = ((JArray)request.Data)[1].ToString();
                    NetworkData response = new()
                    {
                        MessageType = "LoginResponse",
                        Data = VerifyLoginData(username, password) ? currentUser : null
                    };

                    await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)));
                }

                if (request.MessageType == "TestList")
                {

                    var takenTests= _userTestRepository
                        .FindAll(x => x.UserId == currentUser.Id && x.IsTaken)
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




                    List<Test> taken = _testRepository.FindAll(test => takenTests.Select(x => x.TestId).Contains(test.Id)).ToList();


                    var questions = _questionRepository.FindAll(x => taken.Select(x =>x.Id).Contains(x.TestId));

                    var userAnswers = _userAnswerRepository
                        .FindAll(x => takenTests
                        .Select(test => test.Id)
                        .Contains(x.UserTestId))
                        .ToList();
                    var answers = _answerRepository.FindAll(a => questions.Select(a => a.Id).Contains(a.QuestionId));
                    List<AnswerModel> answerModels = answers.Select(value => new AnswerModel
                    {
                        Id = value.Id,
                        QuestionId = value.QuestionId,
                        AnswerText = value.AnswerText,
                        IsChecked = userAnswers.Any(ua => ua.AnswerId == value.Id)

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

                   

                    List<TestResults> takenToSend = taken.Select(value => new TestResults
                    {
                        Id = value.Id,
                        Title = value.Title,
                        Author = value.Author,
                        Description = value.Description,
                        Info = value.Info,
                        PassPercent = value.PassPercent,
                        LoadedDate = value.LoadedDate,
                        TotalPossiblePoints = value.Questions.Sum(x => x.Points),
                        PointsGrade = takenTests.FirstOrDefault(x => x.TestId == value.Id)?.PointsGrade ?? 0,
                        IsPassed = takenTests.FirstOrDefault(x => x.TestId == value.Id)?.IsPassed ?? false,
                        TakenDate = takenTests.FirstOrDefault(x => x.TestId == value.Id)?.TakenDate ?? DateTime.MinValue,
                        IsTaken = takenTests.FirstOrDefault(x => x.TestId == value.Id)?.IsTaken ?? false,
                        Questions = selectedData.Where(x => x.TestId == value.Id).ToList(),

                    }).ToList();

                    foreach (var item in takenToSend)
                    {
                        item.ScoredPercent = (double)item.PointsGrade / (double)item.TotalPossiblePoints * 100;
                    }
                    NetworkData response = new()
                    {
                        MessageType = "TestListResponse",
                        Data = takenToSend
                    };

                    string ch = JsonConvert.SerializeObject(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(ch));


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




                    //List<Test> taken = _testRepository.FindAll(test => takenTests.Contains(test.Id)).ToList();

                    //List<Test> takenToSend = taken.Select(value => new Test
                    //{
                    //    Id = value.Id,
                    //    Title = value.Title,
                    //    Author = value.Author,
                    //    Description = value.Description,
                    //    Info = value.Info,
                    //    PassPercent = value.PassPercent,
                    //    IsArchived = value.IsArchived,
                    //    LoadedDate = value.LoadedDate
                    //}).ToList();



                    //NetworkData response = new()
                    //{
                    //    MessageType = "TestListResponse",
                    //    Data = new List<List<Test>> { takenToSend }
                    //};


                    //await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)));
                }

                
            }
            catch (Exception) { }
        }

        private List<Test> GetAssignedTestList() 
        {
            var assignedtestIds = _userTestRepository.FindAll(x => x.UserId == currentUser.Id && !x.IsTaken)
                                           .Select(ut => ut.TestId)
                                           .ToList();
            List<Test> assigned = _testRepository.FindAll(test => assignedtestIds.Contains(test.Id)).ToList();
            return assigned.Select(value => new Test
            {
                Id = value.Id,
                Title = value.Title,
                Author = value.Author,
                Description = value.Description,
                Info = value.Info,
                PassPercent = value.PassPercent,
                IsArchived = value.IsArchived,
                LoadedDate = value.LoadedDate,
            }).ToList();
        }

        private bool VerifyLoginData(string login, string password)
        {
            currentUser = _userRepository.GetAll().FirstOrDefault(x => x.Login == login && x.Password == password);
            return currentUser != null;
        }
        
        private void OnStopServerClick()
        {
            tcpListener?.Stop();
        }


        public IRelayCommand StartServerCommand { get; }
        public IRelayCommand StopServerCommand { get; }

    }
}
