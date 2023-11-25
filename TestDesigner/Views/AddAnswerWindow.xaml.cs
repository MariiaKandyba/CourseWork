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

namespace TestDesigner.Views
{
    /// <summary>
    /// Interaction logic for AddAnswerWindow.xaml
    /// </summary>
    public partial class AddAnswerWindow : Window
    {
        public Answer Answer { get; set; }
        public AddAnswerWindow(Answer answer = null)
        {
            InitializeComponent();
            if(answer == null)
            {
                Answer = new Answer();
            }
            else
            {
                Answer = answer;
                AnswerTextBox.Text = answer.TextAnswer;
                IsTrueCheckBox.IsChecked = bool.Parse(answer.IsRight); 
            }

        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            Answer.TextAnswer = AnswerTextBox.Text;
            Answer.IsRight = IsTrueCheckBox.IsChecked.ToString();
            DialogResult = true;
            Close();
        }
    }
}
