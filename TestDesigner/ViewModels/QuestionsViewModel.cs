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
using TestDesigner.Views;

namespace TestDesigner.ViewModels
{
    public class QuestionsViewModel : ObservableObject
    {
        public IRelayCommand<Question> SelectionChangedCommand { get; }
        public IRelayCommand<object> CreateTestCommand{ get; }
        public IRelayCommand<object> AddQuestionCommand{ get; }

        public QuestionsViewModel()
        {
            OpenClickCommand = new RelayCommand<object>(OnOpenClick);
            CreateTestCommand = new RelayCommand<object>(OnCreateClick);
            AddQuestionCommand = new RelayCommand<object>(OnAddClick);
            Test = new Test();
            Test.Questions = new();
            Test.Questions.Question = new();
        }



        private void OnAddClick(object? obj)
        {
            NewQuestionWindow window = new NewQuestionWindow();
            if (window.ShowDialog() == true)
            {
                Questions.Add(window.Question);
                Test.Questions.Question.Add(window.Question);
            }
        }


        private void OnCreateClick(object? obj)
        {
            Test = new Test();
            Test.Questions = new();
            Test.Questions.Question = new();
            QuestionCount = 0;
            MaxPoints = 0;
           
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
                    QuestionCount = value.Questions?.Question.Count ?? 0;
                    MaxPoints = value.Questions?.Question.Sum(q => int.Parse(q.Points)) ?? 0;
                }
            }
        }
        private ObservableCollection<Question> _questions = new ObservableCollection<Question>();

        public ObservableCollection<Question> Questions
        {
            get { return _questions; }
            set { SetProperty(ref _questions, value); }
        }





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
                Test = Test.LoadFileInfo(XDocument.Load(openFileDialog.FileName).Root);
            return null;
        }




    }
}
