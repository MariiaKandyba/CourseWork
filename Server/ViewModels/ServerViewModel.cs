using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest;
using DALTest.Entities;
using iText.Layout.Element;
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

        private readonly RepositoryFilter _repositoryFilter;
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

            _repositoryFilter = new(_testRepository, _userTestRepository, _questionRepository, _answerRepository, _userAnswerRepository);

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
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[5024];
            int bytesRead = await stream.ReadAsync(buffer);

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
                List<List<TestResults>> tests = new()
                {
                    _repositoryFilter.GetTestResults(currentUser.Id, false),
                    _repositoryFilter.GetTestResults(currentUser.Id, true)
                };

                NetworkData response = new()
                {
                    MessageType = "TestListResponse",
                    Data = tests
                };

                string ch = JsonConvert.SerializeObject(response);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(ch));


                   
            }
            if (request.MessageType == "TestCompleted")
            {
                TestResults gottenTest = JsonConvert.DeserializeObject<TestResults>(JsonConvert.SerializeObject(request.Data));

                UserTest userTestId = _userTestRepository.FindAll(x => x.UserId == currentUser.Id && x.TestId == gottenTest.Id).FirstOrDefault();

                string aa = string.Empty;

                List<UserAnswer> actualAnswers = new();
                foreach (var item in gottenTest.Questions)
                {
                    foreach (var ans in item.Answers)
                    {
                        if (ans.IsChecked)
                        {
                            UserAnswer answer = new UserAnswer()
                            {
                                IsChecked = true,
                                AnswerId = ans.Id,
                                UserTestId = userTestId.Id,
                            };
                            actualAnswers.Add(answer);
                        }

                    }

                }
                foreach (var answer in actualAnswers)
                {
                    //aa += $"IsChecked: {answer.IsChecked}\n" +
                    //    $"UserTestId: {answer.UserTestId}" +
                    //    $"\nAnswerId: {answer.AnswerId}";
                }

                double grade = _repositoryFilter.CalculateGrade(actualAnswers, gottenTest.Id);
                userTestId.PointsGrade = (int)grade;
                userTestId.IsPassed = _repositoryFilter.IsPassed(gottenTest.Id, grade);
                userTestId.TakenDate = DateTime.Now;
                userTestId.IsTaken = true;
                userTestId.UserId = currentUser.Id;
                userTestId.TestId = gottenTest.Id;




                //string userTest = $"Id: {userTestId.Id}" +
                //    $"\n" +
                //    $"PointsGrade: {userTestId.PointsGrade}" +
                //    $"\n" +
                //    $"IsPassed: {userTestId.IsPassed}" +
                //    $"\n" +
                //    $"TakenDate: {userTestId.TakenDate}" +
                //    $"\n" +
                //    $"IsTaken: {userTestId.IsTaken}" +
                //    $"\n" +
                //    $"UserId: {userTestId.UserId}" +
                //    $"\n" +
                //    $"TestId: {userTestId.TestId}"
                //    ;


                _userTestRepository.Update(userTestId);
                foreach (var answer in actualAnswers)
                {
                    _userAnswerRepository.Add(answer);
                }


                NetworkData response = new()
                {
                    MessageType = "CurrentTestResults",
                    Data = _repositoryFilter.GetTestResultsToShow(gottenTest, userTestId, actualAnswers)
                };

                string ch = JsonConvert.SerializeObject(response);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(ch));
            }

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
