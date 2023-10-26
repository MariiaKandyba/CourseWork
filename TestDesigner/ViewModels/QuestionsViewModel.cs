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
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using TestDesigner.Services;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TestDesigner.ViewModels
{
    public class QuestionsViewModel : ObservableObject
    {

        private Test _test = new Test();
        private ObservableCollection<Question> _questions = new ObservableCollection<Question>();
        private Question _selectedQuestion;
        private string _questionCount;
        private string _maxPoints;


        public ObservableCollection<Question> Questions
        {
            get { return _questions; }
            set 
            { 
                SetProperty(ref _questions, value); 
                UpdateQuestionCountAndMaxPoints();  
            }
        }

        public string QuestionCount
        {
            get { return _questionCount; }
            set { SetProperty(ref _questionCount, value); }
        }

        public string MaxPoints
        {
            get { return _maxPoints; }
            set { SetProperty(ref _maxPoints, value); }
        }

        public Test Test
        {
            get { return _test; }
            set
            {
                SetProperty(ref _test, value);
                UpdateQuestionCountAndMaxPoints();
            }
        }
        public Question SelectedQuestion
        {
            get { return _selectedQuestion; }
            set { SetProperty(ref _selectedQuestion, value); }
        }


        public IAsyncRelayCommand OpenTestCommand { get; }
        public IAsyncRelayCommand SaveTestCommand { get; }
        public IRelayCommand<object> CreateTestCommand{ get; }
        public IRelayCommand<object> AddQuestionCommand{ get; }
        public IRelayCommand<object> EditQuestionCommand { get; }
        public IRelayCommand<object> DeleteQuestionCommand { get; }

        private readonly ITestService _testService;

        public QuestionsViewModel(ITestService testService)
        {
            _testService = testService;

            OpenTestCommand = new AsyncRelayCommand(OnOpenTestClick);
            SaveTestCommand = new AsyncRelayCommand(OnSaveTestClick);
            CreateTestCommand = new RelayCommand<object>(OnCreateTestClick);

            AddQuestionCommand = new RelayCommand<object>(OnAddQuestionClick);
            EditQuestionCommand = new RelayCommand<object>(OnEditQuestionClick);
            DeleteQuestionCommand = new RelayCommand<object>(OnDeleteQuestionClick);
            Test = new Test { Questions = new() { Question = new() { } } };


        }


        private void UpdateQuestionCountAndMaxPoints()
        {
            if (Test != null)
            {
                QuestionCount = Questions.Count.ToString() ?? string.Empty;
                MaxPoints = Questions.Sum(q => int.Parse(q.Points)).ToString() ?? string.Empty;
            }
        }



        private void OnAddQuestionClick(object? obj)
        {
            NewQuestionWindow window = new();
            if (window.ShowDialog() == true)
            {
                Questions.Add(window.Question);
                UpdateQuestionCountAndMaxPoints();


            }
        }
      

        private void OnDeleteQuestionClick(object? obj)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the question?", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Questions.Remove(SelectedQuestion);
                UpdateQuestionCountAndMaxPoints();
            }
        }


        private void OnEditQuestionClick(object? obj)
        {
            var window = new NewQuestionWindow(SelectedQuestion);
            if (window.ShowDialog() == true)
            {
                int index = Questions.IndexOf(SelectedQuestion);
                if (index >= 0)  Questions[index] = window.Question;
                UpdateQuestionCountAndMaxPoints();
            }
        }

        private void OnCreateTestClick(object? obj)
        {
            Test = new Test { Questions = new() { Question = new() { } } };
            Questions.Clear();
            UpdateQuestionCountAndMaxPoints();
        }

        private async Task OnOpenTestClick()
        {
            var openFileDialog = new OpenFileDialog() { Filter = "XML files (*.xml)|*.xml" };
            if (openFileDialog.ShowDialog() == true)
            {
                Test = await _testService.LoadTestFromFileAsync(openFileDialog.FileName);
                Questions = new ObservableCollection<Question>(Test.Questions.Question);
            }
        }

        private async Task OnSaveTestClick()
        {
            var saveFileDialog = new SaveFileDialog() { Filter = "XML files (*.xml)|*.xml" };
            if (saveFileDialog.ShowDialog() == true)
            {
                Test = await _testService.AssembleTestAsync(Test, Questions.ToList());
                await _testService.SaveTestToFileAsync(Test, saveFileDialog.FileName);
            }
        }
    }
}


