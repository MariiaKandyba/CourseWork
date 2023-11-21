using DALTest.Entities;
using System;
using System.Collections.Generic;
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
using TestServices;
using Question = DALTest.Entities.Question;
using Answer = DALTest.Entities.Answer;
using NetworkDataDll;
using static System.Net.Mime.MediaTypeNames;
using Ookii.Dialogs.Wpf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.Win32;
using Image = System.Windows.Controls.Image;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for PassedTestInfo.xaml
    /// </summary>
    public partial class PassedTestInfo : Window
    {
        private TestResults _testResult;

        public PassedTestInfo(TestResults test)
        {
            InitializeComponent();
            _testResult = test;
            DataContext = _testResult;
        }
        
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PDFTestButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                string filePath = System.IO.Path.Combine(dialog.SelectedPath, $"{_testResult.Title}.pdf");
                PdfGenerator.GeneratePdf(_testResult, filePath, null);
            }

        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ImageViewerWindow imageViewerWindow = new(GetImageDataFromImage(sender as Image));
                imageViewerWindow.Show();
            }
            catch (Exception){}

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
            catch (Exception) { throw; }
            return null;


        }

    }
}
