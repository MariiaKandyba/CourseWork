using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for NewQuestionWindow.xaml
    /// </summary>
    public partial class NewQuestionWindow : Window
    {
        public Question Question { get; set; }
        public BindingList<Answer> Answers { get; set; }

        public NewQuestionWindow(Question question = null!)
        {
            InitializeComponent();
            if (question != null)
            {
                Question = question;
                if (question.Img != null && question.Img.Length > 0)
                {
                    using MemoryStream stream = new MemoryStream(question.Img);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    imgPreview.Source = bitmapImage;
                }

                TextTB.Text = question.QuestionText;
                PointsTB.Text = question.Points;
                Answers = new BindingList<Answer>( question.Answers.Answer);

            }
            else
            {
                Question =  new Question();
                Answers = new BindingList<Answer>();

            }
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

        private void AddimageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath);
                bitmapImage.EndInit();

                imgPreview.Source = bitmapImage;

                // Зчитуємо бінарне представлення зображення

                // Закодоване у Base64 представлення для збереження в ImgBase64
                Question.Img = File.ReadAllBytes(imagePath);
            }
        }


    }
}
