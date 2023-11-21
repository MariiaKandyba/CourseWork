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
        public IGenericRepository<Group> GroupRepository { get; set; }
        public IGenericRepository<Test> TestRepository { get; set; }
        public IGenericRepository<UserTest> UserTestRepository { get; set; }
        public IGenericRepository<Question> QuestionRepository { get; set; }
        public IGenericRepository<Answer> AnswerRepository { get; set; }
        public IGenericRepository<UserAnswer> UserAnswerRepository { get; set; }
        GenericUnitOfWork _unitOfWork;
        public RepositoryFilter RepositoryFilter { get; set; }


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
            GroupRepository = _unitOfWork.Repository<Group>();
            TestRepository = _unitOfWork.Repository<Test>();
            UserTestRepository = _unitOfWork.Repository<UserTest>();
            QuestionRepository = _unitOfWork.Repository<Question>();
            AnswerRepository = _unitOfWork.Repository<Answer>();
            UserAnswerRepository = _unitOfWork.Repository<UserAnswer>();

            RepositoryFilter = new(TestRepository, UserTestRepository, QuestionRepository, AnswerRepository, UserAnswerRepository, UserRepository);

        }

        public bool IsVerifed(string login, string password)
        {
            return RepositoryFilter.VerifyAdmin(login, password) != null;

        }

        public List<List<TestResults>> GetAssignedAndUnassignedTestLists(int userId)
        {
            return new()
                {
                    RepositoryFilter.GetTestResults(userId, false),
                    RepositoryFilter.GetTestResults(userId, true)
                };

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


            double grade = RepositoryFilter.CalculateGrade(actualAnswers, gottenTest.Id);
            userTestId.PointsGrade = (int)grade;
            userTestId.IsPassed = RepositoryFilter.IsPassed(gottenTest.Id, grade);
            userTestId.TakenDate = DateTime.Now;
            userTestId.IsTaken = true;
            userTestId.UserId = userId;
            userTestId.TestId = gottenTest.Id;

            UserTestRepository.Update(userTestId);
            foreach (var answer in actualAnswers)
                UserAnswerRepository.Add(answer);

            return RepositoryFilter.GetTestResultsToShow(gottenTest, userTestId, actualAnswers);

        }

        public User GetCurrentUser(string login, string password)
        {
            return UserRepository.GetAll().FirstOrDefault(x => x.Login == login && x.Password == password && !x.IsArchived);
        }
    }

}
