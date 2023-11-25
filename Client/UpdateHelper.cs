using Client.ViewModels;
using DALTest.Entities;
using NetworkDataDll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class UpdateHelper
    {
        private static  TcpClient tcpClient;
        private static string serverIpAddress = "127.0.0.1";
        private static int serverPort = 12345;
        public async static Task<List<List<TestResults>>> GetTests(User user)
        {
            try
            {
                using (tcpClient = new TcpClient())
                {
                    try
                    {
                        await tcpClient.ConnectAsync(serverIpAddress, serverPort);
                        using NetworkStream stream = tcpClient.GetStream();
                        NetworkData request = new()
                        {
                            MessageType = "TestList",
                            Data = user.Id
                        };
                        string requestJson = JsonConvert.SerializeObject(request);
                        byte[] requestBuffer = Encoding.UTF8.GetBytes(requestJson);
                        stream.Write(requestBuffer, 0, requestBuffer.Length);



                        byte[] responseBuffer = new byte[50000192];
                        int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                        string responseJson = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);


                        NetworkData response = JsonConvert.DeserializeObject<NetworkData>(responseJson);

                        if (response.MessageType == "TestListResponse" && (response.Data != null))
                            return JsonConvert.DeserializeObject<List<List<TestResults>>>(response.Data.ToString());
                        return null;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Problems occured while connecting. Try again lster");
                    }
                    return null;
                    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problems occured while connecting. Try again lster");
                throw;
            }
        }
    }
}
