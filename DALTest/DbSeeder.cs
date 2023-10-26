using DALTest.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestServices;

namespace DALTest
{
    public static class DbSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            var users = new List<Entities.User>
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

            modelBuilder.Entity<Entities.User>().HasData(users);


            var groups = new List<Entities.Group>
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
            modelBuilder.Entity<Entities.Group>().HasData(groups);

            modelBuilder.Entity<Entities.Group>()
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


            var tests = new List<Entities.Test>
            {
                new Entities.Test
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

            modelBuilder.Entity<Entities.Test>().HasData(tests);

            var questions = new List<Entities.Question>();
            var answers = new List<Entities.Answer>();
            int questionIndex = 0;
            int answerIndex = 0;
            foreach (var question in test.Questions.Question)
            {
                questions.Add(new Entities.Question
                {
                    Id = ++questionIndex,
                    QuestionText = question.QuestionText,
                    Img = string.Empty,
                    Points = Convert.ToInt32(question.Points),
                    TestId = 1
                });
                foreach (var answer in question.Answers.Answer)
                {
                    answers.Add(new  Entities.Answer
                    {
                        Id = ++answerIndex,
                        AnswerText = answer.TextAnswer,
                        IsRight = Convert.ToBoolean(answer.IsRight),
                        QuestionId = questionIndex
                    });
                }
            }



            modelBuilder.Entity<Entities.Question>().HasData(questions);
            modelBuilder.Entity<Entities.Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Entities.Answer>().HasData(answers);

            var userTests = new List<Entities.UserTest>
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

            modelBuilder.Entity<Entities.UserTest>().HasData(userTests);


            var userAnswers = new List<Entities.UserAnswer>
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





            modelBuilder.Entity<Entities.UserAnswer>().HasData(userAnswers);
            modelBuilder.Entity<Entities.UserAnswer>()
            .HasOne(ua => ua.Answer)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
