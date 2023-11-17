using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DALTest;
using DALTest.Entities;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.Logging;
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
using Application = System.Windows.Application;
using Question = DALTest.Entities.Question;
using Test = DALTest.Entities.Test;

namespace Server.ViewModels
{


    public class RepositoryHelper
    {
        public IGenericRepository<User> UserRepository { get; set; }
        public IGenericRepository<Test> TestRepository { get; set; }
        public IGenericRepository<UserTest> UserTestRepository { get; set; }
        public IGenericRepository<Question> QuestionRepository { get; set; }
        public IGenericRepository<Answer> AnswerRepository { get; set; }
        public IGenericRepository<UserAnswer> UserAnswerRepository { get; set; }
        GenericUnitOfWork _unitOfWork;
        private readonly RepositoryFilter _repositoryFilter;


        public RepositoryHelper()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            string conStr = builder.Build().GetConnectionString("DefaultConnection")!;

            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            var options = optionsBuilder.UseLazyLoadingProxies().UseSqlServer(conStr).Options;
            _unitOfWork = new GenericUnitOfWork(new Context(options));
            UserRepository = _unitOfWork.Repository<User>();
            TestRepository = _unitOfWork.Repository<Test>();
            UserTestRepository = _unitOfWork.Repository<UserTest>();
            QuestionRepository = _unitOfWork.Repository<Question>();
            AnswerRepository = _unitOfWork.Repository<Answer>();
            UserAnswerRepository = _unitOfWork.Repository<UserAnswer>();

            _repositoryFilter = new(TestRepository, UserTestRepository, QuestionRepository, AnswerRepository, UserAnswerRepository);

        }
        public List<List<TestResults>> GetAssignedAndUnassignedTestLists(int userId)
        {
            return new()
                {
                    _repositoryFilter.GetTestResults(userId, false),
                    _repositoryFilter.GetTestResults(userId, true)
                };

        }
        private List<Test> GetAssignedTestList(int userId)
        {
            var assignedtestIds = UserTestRepository.FindAll(x => x.UserId == userId && !x.IsTaken)
                                           .Select(ut => ut.TestId)
                                           .ToList();
            List<Test> assigned = TestRepository.FindAll(test => assignedtestIds.Contains(test.Id)).ToList();
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

        public TestResults GetResultsAfterTakingTest(int userId, TestResults gottenTest)
        {
            UserTest userTestId = UserTestRepository.FindAll(x => x.UserId == userId && x.TestId == gottenTest.Id).FirstOrDefault();

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


            double grade = _repositoryFilter.CalculateGrade(actualAnswers, gottenTest.Id);
            userTestId.PointsGrade = (int)grade;
            userTestId.IsPassed = _repositoryFilter.IsPassed(gottenTest.Id, grade);
            userTestId.TakenDate = DateTime.Now;
            userTestId.IsTaken = true;
            userTestId.UserId = userId;
            userTestId.TestId = gottenTest.Id;






            UserTestRepository.Update(userTestId);
            foreach (var answer in actualAnswers)
                UserAnswerRepository.Add(answer);

            return _repositoryFilter.GetTestResultsToShow(gottenTest, userTestId, actualAnswers);

        }

        public User GetCurrentUser(string login, string password)
        {
            return UserRepository.GetAll().FirstOrDefault(x => x.Login == login && x.Password == password);
        }
    }

    public class ClientHandler
    {
        private TcpClient _tcpClient;
        private readonly ServerViewModel _serverViewModel;
        public int UserId { get; private set; }

        private RepositoryHelper helper;

        public ClientHandler(TcpClient tcpClient, ServerViewModel serverViewModel)
        {
            _tcpClient = tcpClient;
            _serverViewModel = serverViewModel;
            helper = new RepositoryHelper();
        }

        User currentUser;
        public async Task HandleClient(TcpClient client)
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

                _serverViewModel.AddConnectedClient(UserId, currentUser.FirstName + " " + currentUser.LastName);

                await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)));
            }

            if (request.MessageType == "TestList")
            {
                List<List<TestResults>> test =  helper.GetAssignedAndUnassignedTestLists(Convert.ToInt32(request.Data));

                    NetworkData response = new()
                    {
                        MessageType = "TestListResponse",
                        Data = test
                    };

                    string ch = JsonConvert.SerializeObject(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(ch));
              



            }
            if (request.MessageType == "TestCompleted")
            {
                TestResults gottenTest = JsonConvert.DeserializeObject<TestResults>(JsonConvert.SerializeObject(request.Data));
                TestResults test = helper.GetResultsAfterTakingTest(gottenTest.UserId, gottenTest);



                NetworkData response = new()
                {
                    MessageType = "CurrentTestResults",
                    Data = test
                };

                string ch = JsonConvert.SerializeObject(response);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(ch));
            }

        }



        private bool VerifyLoginData(string login, string password)
        {
            UserId = helper.GetCurrentUser(login, password).Id;
            currentUser = helper.GetCurrentUser(login, password);
            return currentUser != null;
        }
    }


    public class ServerViewModel : ObservableObject
    {
        
        public ObservableCollection<string> ConnectedClients { get; } = new ObservableCollection<string>();

        public void AddConnectedClient(int userId, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ConnectedClients.Add($"{userId}: {username}");
            });
        }

        public void RemoveClientHandler(ClientHandler clientHandler)
        {
            // Видалити зі списку при відключенні клієнта або помилці
            Application.Current.Dispatcher.Invoke(() =>
            {
                ConnectedClients.Remove($"{clientHandler.UserId}: Username");
            });

            _clientHandlers.Remove(clientHandler);
        }



        RepositoryHelper helper;

        public ServerViewModel()
        {
            helper = new();
            StartServerCommand = new AsyncRelayCommand(OnStartServerClick);
            StopServerCommand = new RelayCommand(OnStopServerClick);
        }


        private TcpListener _tcpListener;
        private readonly List<ClientHandler> _clientHandlers = new List<ClientHandler>();


        private async Task OnStartServerClick()
            {
                int port = 12345;
                try
                {
                _tcpListener = new TcpListener(IPAddress.Any, port);
                _tcpListener.Start();

                    while (true) 
                    {
                    TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                    ClientHandler clientHandler = new ClientHandler(client, this);
                    _clientHandlers.Add(clientHandler);

                    await Task.Run(async () => await clientHandler.HandleClient(client));


                }
                } catch (Exception) { }
        }
        
        private void OnStopServerClick()
        {
            _tcpListener?.Stop();
        }


        public IRelayCommand StartServerCommand { get; }
        public IRelayCommand StopServerCommand { get; }

    }
}
