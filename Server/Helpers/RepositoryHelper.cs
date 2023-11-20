using DALTest.Entities;
using DALTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetworkDataDll;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Helpers
{
    public class RepositoryHelper
    {
        public IGenericRepository<User> UserRepository { get; set; }
        public IGenericRepository<Test> TestRepository { get; set; }
        public IGenericRepository<UserTest> UserTestRepository { get; set; }
        public IGenericRepository<Question> QuestionRepository { get; set; }
        public IGenericRepository<Answer> AnswerRepository { get; set; }
        public IGenericRepository<UserAnswer> UserAnswerRepository { get; set; }
        GenericUnitOfWork _unitOfWork;
        private readonly RepositoryFilter _repositoryFilter;


        public RepositoryHelper()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            string conStr = builder.Build().GetConnectionString("DefaultConnection")!;

            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            var options = optionsBuilder.UseLazyLoadingProxies().UseSqlServer(conStr).Options;
            _unitOfWork = new GenericUnitOfWork(new Context(options));
            UserRepository = _unitOfWork.Repository<User>();
            TestRepository = _unitOfWork.Repository<Test>();
            UserTestRepository = _unitOfWork.Repository<UserTest>();
            QuestionRepository = _unitOfWork.Repository<Question>();
            AnswerRepository = _unitOfWork.Repository<Answer>();
            UserAnswerRepository = _unitOfWork.Repository<UserAnswer>();

            _repositoryFilter = new(TestRepository, UserTestRepository, QuestionRepository, AnswerRepository, UserAnswerRepository);

        }
        public List<List<TestResults>> GetAssignedAndUnassignedTestLists(int userId)
        {
            return new()
                {
                    _repositoryFilter.GetTestResults(userId, false),
                    _repositoryFilter.GetTestResults(userId, true)
                };

        }
        private List<Test> GetAssignedTestList(int userId)
        {
            var assignedtestIds = UserTestRepository.FindAll(x => x.UserId == userId && !x.IsTaken)
                                           .Select(ut => ut.TestId)
                                           .ToList();
            List<Test> assigned = TestRepository.FindAll(test => assignedtestIds.Contains(test.Id)).ToList();
            return assigned.Select(value => new Test
            {
                Id = value.Id,
                Title = value.Title,
                Author = value.Author,
                Description = value.Description,
                Info = value.Info,
                PassPercent = value.PassPercent,
                IsArchived = value.IsArchived,
                LoadedDate = value.LoadedDate,
            }).ToList();
        }

        public TestResults GetResultsAfterTakingTest(int userId, TestResults gottenTest)
        {
            UserTest userTestId = UserTestRepository.FindAll(x => x.UserId == userId && x.TestId == gottenTest.Id).FirstOrDefault();

            string aa = string.Empty;

            List<UserAnswer> actualAnswers = new();
            foreach (var item in gottenTest.Questions)
            {
                foreach (var ans in item.Answers)
                {
                    if (ans.IsChecked)
                    {
                        UserAnswer answer = new UserAnswer()
                        {
                            IsChecked = true,
                            AnswerId = ans.Id,
                            UserTestId = userTestId.Id,
                        };
                        actualAnswers.Add(answer);
                    }

                }

            }


            double grade = _repositoryFilter.CalculateGrade(actualAnswers, gottenTest.Id);
            userTestId.PointsGrade = (int)grade;
            userTestId.IsPassed = _repositoryFilter.IsPassed(gottenTest.Id, grade);
            userTestId.TakenDate = DateTime.Now;
            userTestId.IsTaken = true;
            userTestId.UserId = userId;
            userTestId.TestId = gottenTest.Id;

            UserTestRepository.Update(userTestId);
            foreach (var answer in actualAnswers)
                UserAnswerRepository.Add(answer);

            return _repositoryFilter.GetTestResultsToShow(gottenTest, userTestId, actualAnswers);

        }

        public User GetCurrentUser(string login, string password)
        {
            return UserRepository.GetAll().FirstOrDefault(x => x.Login == login && x.Password == password && !x.IsArchived);
        }
    }

}
