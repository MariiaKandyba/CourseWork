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
        public IRelayCommand<object> OpenClickCommand { get; }
        public IRelayCommand<object> EditQuestionCommand { get; }
        public IRelayCommand<object> DeleteQuestionCommand { get; }
        public IRelayCommand<object> SaveTestCommand { get; }


        public QuestionsViewModel()
        {
            OpenClickCommand = new RelayCommand<object>(OnOpenClick);
            CreateTestCommand = new RelayCommand<object>(OnCreateClick);
            AddQuestionCommand = new RelayCommand<object>(OnAddQuestionClick);
            EditQuestionCommand = new RelayCommand<object>(OnEditQuestionClick);
            DeleteQuestionCommand = new RelayCommand<object>(OnDeleteQuestionClick);
            SaveTestCommand = new RelayCommand<object>(OnSaveTestClick);
            Test = new Test();
            Test.Questions = new();
            Test.Questions.Question = new();
        }


        // питання
        private void OnAddQuestionClick(object? obj)
        {
            NewQuestionWindow window = new NewQuestionWindow();
            if (window.ShowDialog() == true)
                Questions.Add(window.Question);
        }
        private void OnSaveTestClick(object? obj)
        {
            string testInfo = $"Author: {Test.Author}\n" +
                  $"Title: {Test.Title}\n" +
                  $"Description: {Test.Description}\n" +
                  $"Passing Percent: {Test.PassPercent}\n" +
                  $"Additional Information: {Test.Info}\n";

            MessageBox.Show(testInfo);

        }
        private void OnDeleteQuestionClick(object? obj)
        {
            MessageBoxResult result = MessageBox.Show("Ви впевнені, що хочете видалити це питання?", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Questions.Remove(SelectedQuestion);
            }
        }


        private void OnEditQuestionClick(object? obj)
        {
            var window = new NewQuestionWindow(SelectedQuestion);
            if (window.ShowDialog() == true)
            {
                int index = Questions.IndexOf(SelectedQuestion);
                if (index >= 0)  Questions[index] = window.Question;
            }
        }


        private void OnCreateClick(object? obj)
        {
            Test = new Test();
            Test.Questions = new();
            Test.Questions.Question = new();
            QuestionCount = string.Empty;
            MaxPoints = string.Empty;

            Questions.Clear();
            
           
        }

        private string _questionCount;
        public string QuestionCount
        {
            get { return _questionCount; }
            set { SetProperty(ref _questionCount, value); }
        }

        private string _maxPoints;
        public string MaxPoints
        {
            get { return _maxPoints; }
            set { SetProperty(ref _maxPoints, value); }
        }

        private Test _test;

        public Test Test
        {
            get { return _test; }
            set { SetProperty(ref _test, value);  }
        }
        private void UpdateQuestionCountAndMaxPoints()
        {
            if (Test != null)
            {
                QuestionCount = Test.Questions?.Question.Count.ToString() ?? string.Empty;
                MaxPoints = Test.Questions?.Question.Sum(q => int.Parse(q.Points)).ToString() ?? string.Empty;
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
                Test = Test.LoadFileInfo(XDocument.Load(openFileDialog.FileName).Root);
                Questions = new ObservableCollection<Question>(Test.Questions.Question);
            }
            return null;
        }




    }
}

//QuestionCount = string.Empty;
//MaxPoints = string.Empty;

//private void UpdateQuestionCountAndMaxPoints()
//{
//    if (Test != null)
//    {
//        QuestionCount = Test.Questions?.Question.Count.ToString() ?? string.Empty;
//        MaxPoints = Test.Questions?.Question.Sum(q => int.Parse(q.Points)).ToString() ?? string.Empty;
//        PassPercent = Test.PassPercent;
//    }
//}

//private string _passPercent;
//public string PassPercent
//{
//    get { return _passPercent; }
//    set { SetProperty(ref _passPercent, value); }
//}
