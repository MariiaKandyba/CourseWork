using System;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using TestDesigner.ViewModels;

namespace TestDesigner
{
    public partial class MainWindow : Window
    {
            public QuestionsViewModel ViewModel { get; set; } // Додайте властивість

            public MainWindow()
            {
                InitializeComponent();
                ViewModel = new QuestionsViewModel(); // Ініціалізуйте ViewModel
                DataContext = ViewModel; // Встановіть DataContext на ViewModel
            }

        //public MainWindow()
        //{
        //    InitializeComponent();
        //    // Створіть екземпляр QuestionsViewModel
        //    var viewModel = new QuestionsViewModel();

        //    // Прив'яжіть DataContext вашого вікна до створеного viewModel
        //    DataContext = viewModel;
        //    //string debugFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        //    //string filePath = System.IO.Path.Combine(debugFolderPath, "Funny_Математика_Логічні_задачі_4_клас.xml");

        //    //XDocument xdoc = XDocument.Load(filePath);

        //    //var test = xdoc.Root; // Отримуємо корінь документа

        //    //// Отримуємо значення з елементів
        //    //string author = test.Element("Author")?.Value;
        //    //string title = test.Element("Title")?.Value;
        //    //string description = test.Element("Description")?.Value;
        //    //string info = test.Element("Info")?.Value;
        //    //int passPercent = Convert.ToInt32(test.Element("PassPercent")?.Value);

        //    //// Виводимо дані на форму
        //    //AuthorTextBox.Text = author;
        //    //TitleTextBox.Text = title;
        //    //DescriptionTextBox.Text = description;
        //    //InfoTextBox.Text = info;
        //    //PassPercentTextBox.Text = passPercent.ToString();

        //    //// Отримуємо питання і бали
        //    //var questions = test.Element("Questions")?.Elements("Question")
        //    //    .Select(q => new
        //    //    {
        //    //        QuestionText = q.Element("QuestionText")?.Value,
        //    //        Points = Convert.ToInt32(q.Element("Points")?.Value)
        //    //    })
        //    //    .ToList();

        //    //// Встановлюємо джерело даних для DataGrid
        //    //QuestionDataGrid.ItemsSource = questions;


        //}



        //private void OpenButton_Click(object sender, RoutedEventArgs e)
        //{


        //}
    }
}
