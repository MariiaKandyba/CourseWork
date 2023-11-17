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
                string folderPath = dialog.SelectedPath;
                string fileName = $"{_testResult.Title}.pdf";
                string filePath = System.IO.Path.Combine(folderPath, fileName);
                PdfGenerator.GeneratePdf(_testResult, filePath, null);

            }

        }
    }
}
