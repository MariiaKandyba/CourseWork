using NetworkDataDll;
using Server.ViewModels;
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

namespace Server.Views.Users
{
    /// <summary>
    /// Interaction logic for UsersResultsWindow.xaml
    /// </summary>
    public partial class UsersResultsWindow : Window
    {
        public UsersResultsWindow(List<TestResults> testResults)
        {
            InitializeComponent();
            var viewModel = new ResultsViewModel(testResults);
            DataContext = viewModel;
        }

      
    }
}
