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

namespace Server.Views.Tests
{
    /// <summary>
    /// Interaction logic for RestoreTest.xaml
    /// </summary>
    public partial class RestoreTest : Window
    {

            public List<Test> Tests { get; set; }
            public List<Test> SelectedTests{ get; set; } = new();
            public RestoreTest(List<Test> tests)
            {
                InitializeComponent();
                Tests = tests;
                grid.ItemsSource = Tests;
            }

            private void RestoreButton_Click(object sender, RoutedEventArgs e)
            {
                foreach (var item in grid.SelectedItems)
                {
                    if (item is Test test)
                        SelectedTests.Add(test);
                }
                DialogResult = true;

            }
    }
}
