using System.Collections.Generic;
using Microsoft.Win32;
using TestDesigner.Models;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Xml.Linq;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;

namespace TestDesigner.ViewModels
{
    public class QuestionsViewModel : ObservableObject
    {
        public QuestionsViewModel()
        {
            OpenClickCommand = new RelayCommand<object>(OnOpenClick);

        }
        private int _questionCount;
        public int QuestionCount
        {
            get { return _questionCount; }
            set { SetProperty(ref _questionCount, value); }
        }

        private int _maxPoints;
        public int MaxPoints
        {
            get { return _maxPoints; }
            set { SetProperty(ref _maxPoints, value); }
        }

        private Test _test;

        public Test Test
        {
            get { return _test; }
            set
            {
                SetProperty(ref _test, value);

                if (value != null)
                {
                    // Отримайте кількість запитань та загальну кількість максимальних балів з тесту
                    QuestionCount = value.Questions?.Question.Count ?? 0;
                    MaxPoints = value.Questions?.Question.Sum(q => int.Parse(q.Points)) ?? 0;
                }
            }
        }



        public IRelayCommand<Question> SelectionChangedCommand { get; }

       
        private Question _selectedQuestion;
        public  Question SelectedQuestion
        {
            get { return _selectedQuestion; }
            set { SetProperty(ref _selectedQuestion, value); }
        }
      

        public IRelayCommand<object> OpenClickCommand { get; }


        

        private void OnOpenClick(object parameter)
        {
            OpenFileDialogAndSelectFile();

        }


        public string OpenFileDialogAndSelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                var test = LoadTestFromFile(selectedFilePath);
                Test = test;
            }
            return null;
        }

        private Test LoadTestFromFile(string filePath)
        {
            XDocument xdoc = XDocument.Load(filePath);

            var test = xdoc.Root; // Отримуємо корінь документа

            string author = test.Element("Author")?.Value;
            string title = test.Element("Title")?.Value;
            string description = test.Element("Description")?.Value;
            string info = test.Element("Info")?.Value;
            int passPercent = Convert.ToInt32(test.Element("PassPercent")?.Value);

            var loadedTest = new Test
            {
                Author = author,
                Title = title,
                Description = description,
                Info = info,
                PassPercent = passPercent.ToString()
            };

            var questions = test.Element("Questions")?.Elements("Question")
                .Select(q => new Question
                {
                    QuestionText = q.Element("QuestionText")?.Value,
                    Points = q.Element("Points")?.Value,
                    Img = q.Element("Img")?.Value,
                    Answers = new Answers
                    {
                        Answer = q.Element("Answers")?.Elements("Answer")
                            .Select(a => new Answer
                            {
                                TextAnswer = a.Element("TextAnswer")?.Value,
                                IsRight = a.Element("IsRight")?.Value
                            })
                            .ToList()
                    }
                })
                .ToList();

            loadedTest.Questions = new Questions
            {
                Question = questions
            };

            return loadedTest;
        }



    }
}
