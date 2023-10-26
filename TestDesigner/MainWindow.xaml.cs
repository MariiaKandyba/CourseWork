using System;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using TestDesigner.ViewModels;
using TestServices;

namespace TestDesigner
{
    public partial class MainWindow : Window
    {

        public QuestionsViewModel ViewModel { get; set; } 
        private readonly ITestService _testService;
        public MainWindow(ITestService testService)
        {
            _testService = testService;
            InitializeComponent();
            ViewModel = new QuestionsViewModel(_testService); 
            DataContext = ViewModel; 
        }

       
    }
}
