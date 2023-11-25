using Client.ViewModels;
using NetworkDataDll;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for AssignmentTest.xaml
    /// </summary>
    public partial class AssignmentTest : Window
    {

        private TcpClient tcpClient;
        private string serverIpAddress = "127.0.0.1";
        private int serverPort = 12345;
        public TestResults TestResult { get; set; }
        private DispatcherTimer timer;
        private TimeSpan elapsedTime;


        public AssignmentTest(TestResults test, int userId)
        {
            InitializeComponent();
            TestResult = test;
            TestResult.UserId = userId;
            DataContext = TestResult;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            elapsedTime = TimeSpan.Zero;
            timer.Start();


        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            timerLabel.Content = elapsedTime.ToString(@"mm\:ss");
        }

        private async void CreateTestButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            List<QuestionModel> allTestResults = new List<QuestionModel>();

            foreach (var item in dataGrid.Items)
            {
                if (item is QuestionModel testResults)
                    allTestResults.Add(testResults);
            }
            List<AnswerModel> an = new();

            foreach (var item in allTestResults)
            {
                foreach (var a in item.Answers)
                    if (a.IsChecked) an.Add(a);
            }
            foreach (var item in TestResult.Questions)
            {
                foreach (var q in item.Answers)
                {
                    if (an.Any(a => a.Id == q.Id))
                        q.IsChecked = true;
                }
            }




            using (tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(serverIpAddress, serverPort);
                using NetworkStream stream = tcpClient.GetStream();
                NetworkData request = new()
                {
                    MessageType = "TestCompleted",
                    Data = TestResult,
                };

                string requestJson = JsonConvert.SerializeObject(request);
                byte[] requestBuffer = Encoding.UTF8.GetBytes(requestJson);
                await stream.WriteAsync(requestBuffer);


                byte[] responseBuffer = new byte[8011155];
                int bytesRead = await stream.ReadAsync(responseBuffer);
                string responseJson = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                NetworkData response = JsonConvert.DeserializeObject<NetworkData>(responseJson);
                if (response.MessageType == "CurrentTestResults")
                {
                    TestResults gottenTest = JsonConvert.DeserializeObject<TestResults>(JsonConvert.SerializeObject(response.Data));
                    PassedTestInfo passedTestInfo = new(gottenTest);
                    passedTestInfo.ShowDialog();

                }
            }
            

        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ImageViewerWindow imageViewerWindow = new(GetImageDataFromImage(sender as Image));
                imageViewerWindow.Show();
            }
            catch (Exception)
            {

            }

        }
        private static byte[] GetImageDataFromImage(Image image)
        {
            try
            {
                if (image.Source is BitmapSource bitmapSource)
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception)
            {
                throw;

            }
            return null;


        }

    }
}
