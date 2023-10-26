using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestServices;

namespace DALTest
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<UserTest> UserTests { get; set; }
        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Mary",
                    LastName = "Smith",
                    Login = "mary_smith",
                    Password = "pass123",
                    Description = "Regular user",
                    IsAdmin = false,
                    IsArchived = false,
                    RegisterDate = DateTime.Now,
                },
                new User
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Login = "john_doe",
                    Password = "password123",
                    Description = "Regular user",
                    IsAdmin = false,
                    IsArchived = false,
                    RegisterDate = DateTime.Now,
                },
                new User
                {
                    Id = 3,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Login = "bob_johnson",
                    Password = "password456",
                    Description = "Regular user",
                    IsAdmin = false,
                    IsArchived = false,
                    RegisterDate = DateTime.Now,
                },
                new User
                {
                    Id = 4,
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Login = "alice_johnson",
                    Password = "pass456",
                    Description = "Admin user",
                    IsAdmin = true,
                    IsArchived = false,
                    RegisterDate = DateTime.Now
                },
                new User
                {
                    Id = 5,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Login = "admin",
                    Password = "123",
                    Description = "Administrator",
                    IsAdmin = true,
                    IsArchived = false,
                    RegisterDate = DateTime.Now,
                }
            };

            modelBuilder.Entity<User>().HasData(users);


            var groups = new List<Group>
            {
                new Group
                {
                    Id = 1,
                    Name = "Group 1",
                    Description = "gr2023 1 semester",
                    IsAdminGroup = false,
                },
                new Group
                {
                    Id = 2,
                    Name = "Admin group 1",
                    Description = "admin group 2023",
                    IsAdminGroup = true,
                },
                new Group
                {
                    Id = 3,
                    Name = "Group 2",
                    Description = "gr2023 2 semester",
                    IsAdminGroup = false,
                }
            };
            modelBuilder.Entity<Group>().HasData(groups);

            modelBuilder.Entity<Group>()
            .HasMany(left => left.Users)
            .WithMany(right => right.Groups)
            .UsingEntity("GroupUser", typeof(Dictionary<string, object>),
                right => right.HasOne(typeof(User)).WithMany().HasForeignKey("UsersId"),
                left => left.HasOne(typeof(Group)).WithMany().HasForeignKey("GroupId"),
                join => join.ToTable("GroupUser")
            );
            modelBuilder.Entity("GroupUser").HasData(
              new Dictionary<string, object> { ["GroupId"] = 1, ["UsersId"] = 1 },
              new Dictionary<string, object> { ["GroupId"] = 1, ["UsersId"] = 2 },
              new Dictionary<string, object> { ["GroupId"] = 1, ["UsersId"] = 3 },
              new Dictionary<string, object> { ["GroupId"] = 2, ["UsersId"] = 4 },
              new Dictionary<string, object> { ["GroupId"] = 2, ["UsersId"] = 5 },
              new Dictionary<string, object> { ["GroupId"] = 3, ["UsersId"] = 1 }
            );

            ISerializationService _serializationService = new SerializationService();
            TestServices.Test test = _serializationService.DeserializeObjectFromXml<TestServices.Test>(Properties.Resources.EnglishA1); ;
          

            var tests = new List<Test>
            {
                new Test
                {
                    Id = 1,
                    Title = test.Title,
                    Author = test.Author,
                    Description = test.Description,
                    Info = test.Info,
                    PassPercent = Convert.ToInt32(test.PassPercent),
                    IsArchived = false,
                    LoadedDate = DateTime.Now,
                }
            };

            modelBuilder.Entity<Test>().HasData(tests);

            var questions = new List<Question>();
            var answers = new List<Answer>();
            int questionIndex = 0;
            int answerIndex = 0;
            foreach (var question in test.Questions.Question)
            {
                questions.Add(new Question
                {
                    Id = ++questionIndex,
                    QuestionText = question.QuestionText,
                    Img = string.Empty,
                    Points = Convert.ToInt32(question.Points),
                    TestId = 1
                });
                foreach (var answer in question.Answers.Answer)
                {
                    answers.Add(new Answer
                    {
                        Id = ++answerIndex,
                        AnswerText = answer.TextAnswer,
                        IsRight = Convert.ToBoolean(answer.IsRight),
                        QuestionId = questionIndex
                    });
                }
            }
          


            modelBuilder.Entity<Question>().HasData(questions);
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>().HasData(answers);

            //};
            //var questions = new List<Question>
            //{
            //    new Question
            //    {
            //        Id = 1,
            //        QuestionText = "What is the plural form of 'child'?",
            //        Img = "",
            //        Points = 10,
            //        TestId = 1
            //    },
            //    new Question
            //    {
            //        Id = 2,
            //        QuestionText = "Which of the following is a preposition: on, table, quickly?",
            //        Img = "",
            //        Points = 15,
            //        TestId = 1
            //    },


            //};

            //var answers = new List<Answer>
            //{
            //    new Answer
            //    {
            //        Id = 1,
            //        AnswerText = "children",
            //        IsRight = true,
            //        QuestionId = 1
            //    },
            //    new Answer
            //    {
            //        Id = 2,
            //        AnswerText = "childs",
            //        IsRight = true,
            //        QuestionId = 1
            //    },
            //    new Answer
            //    {
            //        Id = 3,
            //        AnswerText = "kinder",
            //        IsRight = false,
            //        QuestionId = 1
            //    },
            //     new Answer
            //    {
            //        Id = 4,
            //        AnswerText = "table",
            //        IsRight = false,
            //        QuestionId = 2
            //    },
            //    new Answer
            //    {
            //        Id = 5,
            //        AnswerText = "on",
            //        IsRight = true,
            //        QuestionId = 2
            //    },
            //     new Answer
            //    {
            //        Id = 6,
            //        AnswerText = "quickly",
            //        IsRight = false,
            //        QuestionId = 2
            //    },

            //};


            var userTests = new List<UserTest>
            {
                new UserTest
                {
                    Id = 1,
                    PointsGrade = 70,
                    IsPassed = true,
                    TakenDate = DateTime.Now,
                    IsTaken = true,
                    UserId = 1,
                    TestId = 1,
                },

            };

            modelBuilder.Entity<UserTest>().HasData(userTests);


            var userAnswers = new List<UserAnswer>
            {
                new UserAnswer
                {
                    Id = 1,
                    IsChecked = true,
                    UserTestId = 1,
                    AnswerId = 1,
                },
                new UserAnswer
                {
                    Id = 2,
                    IsChecked = true,
                    UserTestId = 1,
                    AnswerId = 6,
                },
                new UserAnswer
                {
                    Id = 3,
                    IsChecked = true,
                    UserTestId = 1,
                    AnswerId = 7,
                },
                new UserAnswer
                {
                    Id = 4,
                    IsChecked = true,
                    UserTestId = 1,
                    AnswerId = 10,
                },
                new UserAnswer
                {
                    Id = 5,
                    IsChecked = true,
                    UserTestId = 1,
                    AnswerId = 13,
                },
            };





            modelBuilder.Entity<UserAnswer>().HasData(userAnswers);
            modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.Answer)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
// Зараз ви можете розпарсити xmlContent за допомогою XmlTextReader або інших методів для роботи з XML-даними.

//var tests = new List<Test>
//{
//    new Test
//    {
//        Id = 1,
//        Title = "English Grammar Test",
//        Author = "John Doe",
//        Description = "Test your English grammar skills.",
//        Info = "This test covers various aspects of English grammar.",
//        PassPercent = 70,
//        IsArchived = false,
//        LoadedDate = DateTime.Now,
//    }
//};


//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace DALTest
//{
//    public class Context : DbContext
//    {
//        public DbSet<User> Users { get; set; }
//        public DbSet<Group> Groups { get; set; }
//        //public DbSet<Test> Tests { get; set; }
//        //public DbSet<Question> Questions { get; set; }
//        //public DbSet<Answer> Answers { get; set; }
//        //public DbSet<UserAnswer> UserAnswers { get; set; }
//        //public DbSet<UserTest> UserTests { get; set; }
//        public Context(DbContextOptions<Context> options) : base(options)
//        {
//            Database.EnsureCreated();
//        }
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            var users = new List<User>
//            {
//                new User
//                {
//                    Id = 1,
//                    FirstName = "Mary",
//                    LastName = "Smith",
//                    Login = "mary_smith",
//                    Password = "pass123",
//                    Description = "Regular user",
//                    IsAdmin = false,
//                    IsArchived = false,
//                    RegisterDate = DateTime.Now,
//                },
//                new User
//                {
//                    Id = 2,
//                    FirstName = "John",
//                    LastName = "Doe",
//                    Login = "john_doe",
//                    Password = "password123",
//                    Description = "Regular user",
//                    IsAdmin = false,
//                    IsArchived = false,
//                    RegisterDate = DateTime.Now,
//                },
//                new User
//                {
//                    Id = 3,
//                    FirstName = "Bob",
//                    LastName = "Johnson",
//                    Login = "bob_johnson",
//                    Password = "password456",
//                    Description = "Regular user",
//                    IsAdmin = false,
//                    IsArchived = false,
//                    RegisterDate = DateTime.Now,
//                },
//                new User
//                {
//                    Id = 4,
//                    FirstName = "Alice",
//                    LastName = "Johnson",
//                    Login = "alice_johnson",
//                    Password = "pass456",
//                    Description = "Admin user",
//                    IsAdmin = true,
//                    IsArchived = false,
//                    RegisterDate = DateTime.Now 
//                },
//                new User
//                {
//                    Id = 5,
//                    FirstName = "Admin",
//                    LastName = "Admin",
//                    Login = "admin",
//                    Password = "123",
//                    Description = "Administrator",
//                    IsAdmin = true,
//                    IsArchived = false,
//                    RegisterDate = DateTime.Now,
//                }
//            };

//            modelBuilder.Entity<User>().HasData(users);


//            var groups = new List<Group>
//            {
//                new Group
//                {
//                    Id = 1,
//                    Name = "Group 1",
//                    Description = "gr2023 1 semester",
//                    IsAdminGroup = false,
//                },
//                new Group
//                {
//                    Id = 2,
//                    Name = "Admin group 1",
//                    Description = "admin group 2023",
//                    IsAdminGroup = true,
//                },
//                new Group
//                {
//                    Id = 3,
//                    Name = "Group 2",
//                    Description = "gr2023 2 semester",
//                    IsAdminGroup = false,
//                }
//            };
//            modelBuilder.Entity<Group>().HasData(groups);

//            modelBuilder.Entity<Group>()
//            .HasMany(left => left.Users)
//            .WithMany(right => right.Groups)
//            .UsingEntity("GroupUser", typeof(Dictionary<string, object>),
//                right => right.HasOne(typeof(User)).WithMany().HasForeignKey("UsersId"),
//                left => left.HasOne(typeof(Group)).WithMany().HasForeignKey("GroupId"),
//                join => join.ToTable("GroupUser")
//            );
//            modelBuilder.Entity("GroupUser").HasData(
//              new Dictionary<string, object> { ["GroupId"] = 1, ["UsersId"] = 1 },
//              new Dictionary<string, object> { ["GroupId"] = 1, ["UsersId"] = 2 },
//              new Dictionary<string, object> { ["GroupId"] = 1, ["UsersId"] = 3 },
//              new Dictionary<string, object> { ["GroupId"] = 2, ["UsersId"] = 4 },
//              new Dictionary<string, object> { ["GroupId"] = 2, ["UsersId"] = 5 },
//              new Dictionary<string, object> { ["GroupId"] = 3, ["UsersId"] = 1 }
//            );


//            var tests = new List<Test>
//            {
//                new Test
//                {
//                    Id = 1,
//                    Title = "English Grammar Test",
//                    Author = "John Doe",
//                    Description = "Test your English grammar skills.",
//                    Info = "This test covers various aspects of English grammar.",
//                    PassPercent = 70,
//                    IsArchived = false,
//                    LoadedDate = DateTime.Now,
//                }
//                //,
//                //new Test
//                //{
//                //    Id = 2,
//                //    Title = "Math Quiz",
//                //    Author = "Alice Johnson",
//                //    Description = "Test your mathematical skills.",
//                //    Info = "This quiz contains various math problems.",
//                //    PassPercent = 75,
//                //    IsArchived = false,
//                //    LoadedDate = DateTime.Now,
//                //},
//                //new Test
//                //{
//                //    Id = 3,
//                //    Title = "History Quiz",
//                //    Author = "Mary Smith",
//                //    Description = "Test your knowledge of history.",
//                //    Info = "This quiz covers various historical events.",
//                //    PassPercent = 80,
//                //    IsArchived = true,
//                //    LoadedDate = DateTime.Now,
//                //}
//            };

//            modelBuilder.Entity<Test>().HasData(tests);


//            var questions = new List<Question>
//            {
//                new Question
//                {
//                    Id = 1,
//                    QuestionText = "What is the plural form of 'child'?",
//                    Img = "",
//                    Points = 10,
//                    TestId = 1
//                },
//                new Question
//                {
//                    Id = 2,
//                    QuestionText = "Which of the following is a preposition: on, table, quickly?",
//                    Img = "",
//                    Points = 15,
//                    TestId = 1
//                },

//                //new Question
//                //{
//                //    Id = 3,
//                //    QuestionText = "Solve for x: 2x + 5 = 15",
//                //    Img = "",
//                //    Points = 10,
//                //    TestId = 2 // Питання для тесту з Id = 2 (Math Quiz)
//                //},
//                //new Question
//                //{
//                //    Id = 4,
//                //    QuestionText = "What is the square root of 144?",
//                //    Img = "",
//                //    Points = 10,
//                //    TestId = 2 // Питання для тесту з Id = 2 (Math Quiz)
//                //},
//                //new Question
//                //{
//                //    Id = 5,
//                //    QuestionText = "Who was the first President of the United States?",
//                //    Img = "",
//                //    Points = 10,
//                //    TestId = 3 // Питання для тесту з Id = 3 (History Quiz)
//                //},
//                //new Question
//                //{
//                //    Id = 6,
//                //    QuestionText = "In which year did the Titanic sink?",
//                //    Img = "",
//                //    Points = 15,
//                //    TestId = 3 // Питання для тесту з Id = 3 (History Quiz)
//                //}
//            };


//            modelBuilder.Entity<Question>().HasData(questions);


//            var answers = new List<Answer>
//            {
//                new Answer
//                {
//                    Id = 1,
//                    AnswerText = "children",
//                    IsRight = true,
//                    QuestionId = 1 
//                },
//                new Answer
//                {
//                    Id = 2,
//                    AnswerText = "childs",
//                    IsRight = true,
//                    QuestionId = 1
//                },
//                new Answer
//                {
//                    Id = 3,
//                    AnswerText = "kinder",
//                    IsRight = false,
//                    QuestionId = 1
//                },
//                 new Answer
//                {
//                    Id = 4,
//                    AnswerText = "table",
//                    IsRight = false,
//                    QuestionId = 2
//                },
//                new Answer
//                {
//                    Id = 5,
//                    AnswerText = "on",
//                    IsRight = true,
//                    QuestionId = 2
//                },
//                 new Answer
//                {
//                    Id = 6,
//                    AnswerText = "quickly",
//                    IsRight = false,
//                    QuestionId = 2
//                },
//                //new Answer
//                //{
//                //    Id = 4,
//                //    AnswerText = "7",
//                //    IsRight = true,
//                //    QuestionId = 3 // Відповідь для питання з Id = 3
//                //},
//                //new Answer
//                //{
//                //    Id = 5,
//                //    AnswerText = "12",
//                //    IsRight = false,
//                //    QuestionId = 3 // Відповідь для питання з Id = 3
//                //},
//                //new Answer
//                //{
//                //    Id = 6,
//                //    AnswerText = "sqrt(144)",
//                //    IsRight = true,
//                //    QuestionId = 4 // Відповідь для питання з Id = 4
//                //},
//                //new Answer
//                //{
//                //    Id = 7,
//                //    AnswerText = "12",
//                //    IsRight = false,
//                //    QuestionId = 4 // Відповідь для питання з Id = 4
//                //},
//                //new Answer
//                //{
//                //    Id = 8,
//                //    AnswerText = "George Washington",
//                //    IsRight = true,
//                //    QuestionId = 5 // Відповідь для питання з Id = 5
//                //},
//                //new Answer
//                //{
//                //    Id = 9,
//                //    AnswerText = "Thomas Jefferson",
//                //    IsRight = false,
//                //    QuestionId = 5 // Відповідь для питання з Id = 5
//                //},
//                //new Answer
//                //{
//                //    Id = 10,
//                //    AnswerText = "James Madison",
//                //    IsRight = false,
//                //    QuestionId = 5 // Відповідь для питання з Id = 5
//                //},
//                //new Answer
//                //{
//                //    Id = 11,
//                //    AnswerText = "1782",
//                //    IsRight = false,
//                //    QuestionId = 6 // Відповідь для питання з Id = 6
//                //},
//                //new Answer
//                //{
//                //    Id = 12,
//                //    AnswerText = "1912",
//                //    IsRight = false,
//                //    QuestionId = 6 // Відповідь для питання з Id = 6
//                //},
//                //new Answer
//                //{
//                //    Id = 13,
//                //    AnswerText = "1914",
//                //    IsRight = true,
//                //    QuestionId = 6 // Відповідь для питання з Id = 6
//                //}
//            };



//            modelBuilder.Entity<Answer>().HasData(answers);
//            var userTests = new List<UserTest>
//            {
//                new UserTest
//                {
//                    Id = 1,
//                    PointsGrade = 70,
//                    IsPassed = true,
//                    TakenDate = DateTime.Now,
//                    IsTaken = true,
//                    UserId = 1, 
//                    TestId = 1, 
//                },
//                //new UserTest
//                //{
//                //    Id = 2,
//                //    PointsGrade = 60,
//                //    IsPassed = false,
//                //    TakenDate = DateTime.Now,
//                //    IsTaken = true,
//                //    UserId = 2, 
//                //    TestId = 1, 
//                //},
//                //new UserTest
//                //{
//                //    Id = 3,
//                //    PointsGrade = 80,
//                //    IsPassed = true,
//                //    TakenDate = DateTime.Now,
//                //    IsTaken = true,
//                //    UserId = 3, // Призначити існуючого користувача
//                //    TestId = 3, // Призначити існуючий тест
//                //},
//            };

//            modelBuilder.Entity<UserTest>().HasData(userTests);
//            var userAnswers = new List<UserAnswer>
//            {
//                new UserAnswer
//                {
//                    Id = 1,
//                    IsChecked = true,
//                    UserTestId = 1,
//                    AnswerId = 1,
//                },
//                new UserAnswer
//                {
//                    Id = 2,
//                    IsChecked = true,
//                    UserTestId = 1, // Призначити існуючий UserTest
//                    AnswerId = 2,  // Призначити існуючу відповідь
//                },
//                new UserAnswer
//                {
//                    Id = 3,
//                    IsChecked = false,
//                    UserTestId = 1, // Призначити існуючий UserTest
//                    AnswerId = 3,  // Призначити існуючу відповідь
//                },
//                new UserAnswer
//                {
//                    Id = 4,
//                    IsChecked = true,
//                    UserTestId = 1, // Призначити існуючий UserTest
//                    AnswerId = 5,  // Призначити існуючу відповідь
//                },
//                new UserAnswer
//                {
//                    Id = 5,
//                    IsChecked = true,
//                    UserTestId = 3, // Призначити існуючий UserTest
//                    AnswerId = 8,  // Призначити існуючу відповідь
//                },
//                new UserAnswer
//                {
//                    Id = 6,
//                    IsChecked = true,
//                    UserTestId = 3, // Призначити існуючий UserTest
//                    AnswerId = 9,  // Призначити існуючу відповідь
//                },
//            };

//            modelBuilder.Entity<UserAnswer>().HasData(userAnswers);

//        }
//    }
//}
