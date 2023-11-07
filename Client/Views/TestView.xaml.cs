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
using Client.ViewModels;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    /// 
    public class QuestionPage
    {
        public string QuestionText { get; set; }
        public int Points{ get; set; }
        public List<Answer> Answers { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public int SelectedRadioButton{ get; set; }

    }



    public partial class TestView : Window
    {
        private List<Question> _questions;
        private List<QuestionPage> questionPages;
        private int currentPageIndex = 0;

        public List<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();


        StackPanel topButtonPanel = new StackPanel();
        StackPanel questionStackPanel = new StackPanel();
        Label questionLabel = new Label();
        Label markLabel = new Label();
        public TestView(List<Question> questions)
        {
            InitializeComponent();
            _questions = questions;
            Init();
            InitializePageInfos();
            ShowCurrentPage();

        }
        private void InitializePageInfos()
        {
            questionPages = new();
            foreach (var question in _questions)
            {
                QuestionPage questionPage = new()
                {
                    QuestionText = question.QuestionText,
                    Points = question.Points,
                    Answers = question.Answers.ToList(),
                    SelectedRadioButton = -1 
                };
                questionPages.Add(questionPage);
            }
        }
        public void Init()
        {
            topButtonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height = 50,
                Background = System.Windows.Media.Brushes.LightGray
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



            questionLabel.Content = "AAAAA";
            markLabel.Content = "AAAAA";

            Button confirmButton = new Button
            {
                Content = "Confirm Answers",
                Width = 100, Height = 50,
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

        List<RadioButton> radioButtons = new List<RadioButton>();

        private void ShowCurrentPage()
        {
            if (currentPageIndex >= 0 && currentPageIndex < questionPages.Count)
            {

                radioButtons.Clear();
                questionStackPanel.Children.Clear();
                questionStackPanel.Children.Add(questionLabel);
                questionStackPanel.Children.Add(markLabel);

                questionLabel.Content = (currentPageIndex + 1).ToString() + ". " + questionPages[currentPageIndex].QuestionText;
                markLabel.Content = questionPages[currentPageIndex].Points + " points";
                for (int i = 0; i < questionPages[currentPageIndex].Answers.Count; i++)
                {
                    RadioButton radio1 = new RadioButton
                    {
                        Content = questionPages[currentPageIndex].Answers[i].AnswerText,
                        IsChecked = questionPages[currentPageIndex].SelectedRadioButton == i // Встановлення відповідності обраній відповіді
                    };
                    radioButtons.Add(radio1);
                    radio1.Checked += Radio1_Checked;
                    questionStackPanel.Children.Add(radio1);
                }
            }
        }

        private void Radio1_Checked(object sender, RoutedEventArgs e)
        {
            questionPages[currentPageIndex].SelectedRadioButton = radioButtons.IndexOf((RadioButton)sender);
            questionPages[currentPageIndex].SelectedAnswerIndex = (_questions[currentPageIndex].Answers.FirstOrDefault(x=> x.AnswerText == ((RadioButton)sender).Content)).Id;
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < questionPages.Count - 1)
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
            

            foreach (var item in questionPages)
            {
                UserAnswers.Add(new UserAnswer()
                {
                    UserTestId = 2,
                    AnswerId = item.SelectedAnswerIndex,
                    IsChecked = item.SelectedRadioButton != -1,
                });
                //a += item.QuestionText + Environment.NewLine;
                //a += item.SelectedAnswerIndex + Environment.NewLine;
                //a += " IS CHECKED: " + item.SelectedRadioButton + Environment.NewLine + Environment.NewLine;
                Close();
            }
        }

    }

}
