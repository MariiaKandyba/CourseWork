using NetworkDataDll;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DALTest.Entities;
using System.Net;
using Microsoft.VisualBasic.ApplicationServices;
using User = DALTest.Entities.User;

namespace Server.Helpers
{
    public class ClientHandler
    {
        private TcpClient _tcpClient;
        private readonly ServerViewModel _serverViewModel;

        private RepositoryHelper helper;

        public ClientHandler(TcpClient tcpClient, ServerViewModel serverViewModel)
        {
            _tcpClient = tcpClient;
            _serverViewModel = serverViewModel;
            helper = new RepositoryHelper();
        }
        public User currentUser { get; set; }
        public async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[8011155];
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
                if(currentUser != null)
                    _serverViewModel.AddConnectedClient(this);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)));
            }

            if (request.MessageType == "ClientDisconnect")
            {
                currentUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(request.Data));
                _serverViewModel.RemoveClientHandler(this);
            }


            if (request.MessageType == "TestList")
            {
                List<List<TestResults>> test = helper.GetAssignedAndUnassignedTestLists(Convert.ToInt32(request.Data));

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
            currentUser = helper.GetCurrentUser(login, password);
            return helper.GetCurrentUser(login, password) != null;
        }
    }
}
