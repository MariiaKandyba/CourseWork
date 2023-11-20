using NetworkDataDll;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Server.Views.Users
{
    /// <summary>
    /// Interaction logic for DetailedResultWindow.xaml
    /// </summary>
    public partial class DetailedResultWindow : Window
    {
        private TestResults _testResult;

        public DetailedResultWindow(TestResults test)
        {
            InitializeComponent();
            _testResult = test;
            DataContext = _testResult;
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
