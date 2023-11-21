using DALTest.Entities;
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

namespace Server.Views
{
    /// <summary>
    /// Interaction logic for TestPreview.xaml
    /// </summary>
    public partial class TestPreview : Window
    {


        private Test _test;

        public TestPreview(Test test)
        {
            InitializeComponent();
            _test = test;
            DataContext = _test;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
