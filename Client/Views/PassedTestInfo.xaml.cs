using Client.Models;
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

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for PassedTestInfo.xaml
    /// </summary>
    public partial class PassedTestInfo : Window
    {
        TestResult _testResult;
        public PassedTestInfo(TestResult result)
        {
            InitializeComponent();
            _testResult = result;
            Init();
            ShowCurrentPage();

        }
        private int currentPageIndex = 0;


        StackPanel topButtonPanel = new StackPanel();
        StackPanel questionStackPanel = new StackPanel();
        Label questionLabel = new Label();
           
        
        public void Init()
        {
            topButtonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height = 50,
                Background = Brushes.LightGray
            };

            Button prevButton = new()
            {
                Content = "Previous",
                Width = 100,
                Margin = new Thickness(5)
            };

            Button nextButton = new Button
            {
                Content = "Next",
                Width = 100,
                Margin = new Thickness(5)
            };
            prevButton.Click += PrevBtn_Click;
            nextButton.Click += NextBtn_Click;
            topButtonPanel.Children.Add(prevButton);
            topButtonPanel.Children.Add(nextButton);



            questionLabel.Content = "";

            Button confirmButton = new Button
            {
                Content = "Finish Review",
                Width = 100,
                Height = 50,
            };
            confirmButton.Click += ConfirmBtn_Click;
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            grid.Children.Add(topButtonPanel);
            grid.Children.Add(questionStackPanel);
            grid.Children.Add(confirmButton);

            Grid.SetRow(topButtonPanel, 0);
            Grid.SetRow(questionStackPanel, 1);
            Grid.SetRow(confirmButton, 2);

            Content = grid;
        }


        private void ShowCurrentPage()
        {
               
        if (currentPageIndex >= 0 && currentPageIndex < _testResult.Test.Questions.Count)
            {

                questionStackPanel.Children.Clear();
                questionStackPanel.Children.Add(questionLabel);


                var currentQuestion = _testResult.Test.Questions.ToList()[currentPageIndex];
                questionLabel.Content = (currentPageIndex + 1).ToString() + ". " + currentQuestion.QuestionText;



                for (int i = 0; i < currentQuestion.Answers.ToList().Count; i++)
                {
                    Label label = new() { Content = " * " + currentQuestion.Answers.ToList()[i].AnswerText, };
                    var answered = currentQuestion.Answers.ToList()[i];
                    if (_testResult.UserAnswers.Select(x => x.AnswerId).Contains(answered.Id) && !answered.IsRight)
                    {
                        label.Content += " - your answer";
                        label.Foreground = Brushes.Red;



                    }
                    if (answered.IsRight)
                    {
                        label.Content += " - correct answer";
                        label.Foreground = Brushes.Green;
                    }

                    questionStackPanel.Children.Add(label);
                }

                questionStackPanel.Children.Add(new Label() { Content = " RESULT  " + _testResult.UserAnswers.Select(x => x.UserTest.PointsGrade).FirstOrDefault(), });

            }
        }

        

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < _testResult.Test.Questions.Count - 1)
            {
                currentPageIndex++;
                ShowCurrentPage();
            }
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                ShowCurrentPage();
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {

            
            Close();
        }
    }
}
