using DALTest.Entities;
using NetworkDataDll;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestServices;
using Answer = DALTest.Entities.Answer;
using Test = DALTest.Entities.Test;
using Question = DALTest.Entities.Question;

namespace Server.Helpers
{
    public class RepositoryFilter
    {
        private readonly IGenericRepository<Test> _testRepository;
        private readonly IGenericRepository<UserTest> _userTestRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
        private readonly IGenericRepository<UserAnswer> _userAnswerRepository;
        private readonly IGenericRepository<User> _userRepository;

        public RepositoryFilter(
            IGenericRepository<Test> testRepository,
            IGenericRepository<UserTest> userTestRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Answer> answerRepository,
            IGenericRepository<UserAnswer> userAnswerRepository,
            IGenericRepository<User> userRepository)
        {
            _testRepository = testRepository;
            _userTestRepository = userTestRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userAnswerRepository = userAnswerRepository;
            _userRepository = userRepository;
        }

        public List<TestResults> GetTestResults(int id, bool isTaken)
        {
            var takenTests = _userTestRepository
                        .FindAll(x => x.UserId == id && x.IsTaken == isTaken)
                        .Select(ut => new
                        {
                            ut.Id,
                            ut.TestId,
                            ut.PointsGrade,
                            ut.IsPassed,
                            ut.TakenDate,
                            ut.IsTaken
                        })
                        .ToList();
            List<Test> taken = _testRepository.FindAll(test => takenTests.Select(x => x.TestId).Contains(test.Id) && !test.IsArchived).ToList();


            var questions = _questionRepository.FindAll(x => taken.Select(x => x.Id).Contains(x.TestId));

            var userAnswers = _userAnswerRepository
                .FindAll(x => takenTests
                .Select(test => test.Id)
                .Contains(x.UserTestId))
                .ToList();
            var answers = _answerRepository.FindAll(a => questions.Select(a => a.Id).Contains(a.QuestionId));
            List<AnswerModel> answerModels = answers
                .Select(value => new AnswerModel
                {
                    Id = value.Id,
                    QuestionId = value.QuestionId,
                    AnswerText = value.AnswerText,
                    IsChecked = isTaken && userAnswers.Any(ua => ua.AnswerId == value.Id)

                }).ToList();



            List<Answer> correctAnswers = answers.Where(x => x.IsRight).ToList();


            List<QuestionModel> selectedData = questions.Select(value => new QuestionModel
            {
                QuestionText = value.QuestionText,
                Id = value.Id,
                TestId = value.TestId,
                Img = value.Img,
                Answers = answerModels.Where(x => x.QuestionId == value.Id).ToList(),
            }).ToList();



            List<TestResults> takenToSend = taken
                .Select(value => new TestResults
                {
                    Id = value.Id,
                    Title = value.Title,
                    Author = value.Author,
                    Description = value.Description,
                    Info = value.Info,
                    PassPercent = value.PassPercent,
                    LoadedDate = value.LoadedDate,
                    TotalPossiblePoints = value.Questions.Sum(x => x.Points),
                    PointsGrade = isTaken ? takenTests.FirstOrDefault(x => x.TestId == value.Id)?.PointsGrade ?? 0 : 0,
                    IsPassed = isTaken ? takenTests.FirstOrDefault(x => x.TestId == value.Id)?.IsPassed ?? false : false,
                    TakenDate = isTaken ? takenTests.FirstOrDefault(x => x.TestId == value.Id)?.TakenDate ?? DateTime.MinValue : DateTime.MinValue,
                    IsTaken = isTaken,
                    Questions = selectedData.Where(x => x.TestId == value.Id).ToList(),

                }).ToList();

            if (isTaken)
            {
                foreach (var item in takenToSend)
                {
                    item.ScoredPercent = item.PointsGrade / (double)item.TotalPossiblePoints * 100;
                }
            }



            return takenToSend;
        }

        public double CalculateGrade(List<UserAnswer> userAnswers, int testId)
        {
            var testQuestion = _questionRepository.FindAll(x => x.TestId == testId);
            var UsersAnswersIds = userAnswers.Select(x => x.AnswerId);

            var userPoints = UsersAnswersIds.Sum(answerId => testQuestion.FirstOrDefault(question => question.Answers.Any(answer => answer.Id == answerId && answer.IsRight))?.Points ?? 0);
            return userPoints;

        }

        public bool IsPassed(int testId, double points)
        {
            var test = _testRepository.FindById(testId);
            double PassPercent = test.PassPercent;

            var TotalPossiblePoints = test.Questions.Sum(x => x.Points);

            var ScoredPercent = points / TotalPossiblePoints * 100;

            if (ScoredPercent < PassPercent) return false;
            else return true;


        }

        public TestResults GetTestResultsToShow(TestResults gottenTest, UserTest userTest, List<UserAnswer> userAnswer)
        {
            var test = _testRepository.FindById(userTest.TestId);
            double PassPercent = test.PassPercent;

            var TotalPossiblePoints = test.Questions.Sum(x => x.Points);

            var ScoredPercent = userTest.PointsGrade / TotalPossiblePoints * 100;


            gottenTest.PointsGrade = userTest.PointsGrade;
            gottenTest.IsPassed = userTest.IsPassed;
            gottenTest.ScoredPercent = ScoredPercent;
            gottenTest.TakenDate = userTest.TakenDate;
            gottenTest.IsTaken = userTest.IsTaken;
            gottenTest.TotalPossiblePoints = TotalPossiblePoints;

            foreach (var item in gottenTest.Questions)
            {
                foreach (var answer in item.Answers)
                {
                    var correspondingUserAnswer = userAnswer.FirstOrDefault(ua => ua.AnswerId == answer.Id);

                    if (correspondingUserAnswer != null)
                    {
                        answer.IsChecked = correspondingUserAnswer.IsChecked;
                    }
                }
            }
            return gottenTest;

        }
        public User VerifyAdmin(string login, string password)
        {
            return _userRepository.FindAll(x => x.IsAdmin && x.Login == login && x.Password == password).FirstOrDefault();

        }
    }
}
