using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using TestDesigner.Models;

namespace TestDesigner.Views
{
    /// <summary>
    /// Interaction logic for NewQuestionWindow.xaml
    /// </summary>
    public partial class NewQuestionWindow : Window
    {
        public Question Question { get; set; }
        public BindingList<Answer> Answers { get; set; }

        public NewQuestionWindow(Question question = null!)
        {
            InitializeComponent();
            Question = question ?? new Question();
            Answers = new BindingList<Answer>();
            AnswersDataGrid.ItemsSource = Answers;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Question.QuestionText = TextTB.Text;
            Question.Points = PointsTB.Text;
            Question.Answers = new();
            Question.Answers.Answer = Answers.ToList();
            DialogResult = true;
            Close();
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            AddAnswerWindow window = new();
            if (window.ShowDialog() == true)
                Answers.Add(window.Answer); 
        }

        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            Answer selectedAnswer = (Answer)AnswersDataGrid.SelectedItem; 
            if (selectedAnswer != null)
            {
                AddAnswerWindow window = new(selectedAnswer);
                if (window.ShowDialog() == true)
                {
                    int index = Answers.IndexOf(selectedAnswer);
                    if (index >= 0)
                        Answers[index] = window.Answer; 
                }
            }
        }

        private void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            if (AnswersDataGrid.SelectedItem is not Answer selectedAnswer) return;

            if (MessageBox.Show("Ви впевнені, що бажаєте видалити цей елемент?", "Підтвердження видалення", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Answers.Remove(selectedAnswer);
        }

    }
}
