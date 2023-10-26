//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TestDesigner.Models;

//namespace TestDesigner.Services
//{
//    public class TestService : ITestService
//    {
//        private readonly ISerializationService _serializationService;
//        private readonly IFileService _fileService;

//        public TestService(ISerializationService serializationService, IFileService fileService)
//        {
//            _serializationService = serializationService;
//            _fileService = fileService;
//        }

//        public Test LoadTestFromFile(string filePath)
//        {
//            string xml = _fileService.LoadFile(filePath);
//            return _serializationService.DeserializeObjectFromXml<Test>(xml);
//        }

//        public void SaveTestToFile(Test test, string filePath)
//        {

//            string xml = _serializationService.SerializeObjectToXml(test);
//            _fileService.SaveFile(filePath, xml);
//        }

//        public Test Assamble(Test test, List<Question> questions)
//        {
//            test.Questions.Question.Clear();

//            foreach (var question in questions)
//            {
//                var testQuestion = new Question()
//                {
//                    QuestionText = question.QuestionText,
//                    Points = question.Points,
//                    Answers = new Answers()
//                };
//                testQuestion.Answers.Answer = new List<Answer>();
//                foreach (var answer in question.Answers.Answer)
//                {
//                    testQuestion.Answers.Answer.Add(new Answer
//                    {
//                        TextAnswer = answer.TextAnswer,
//                        IsRight = answer.IsRight
//                    });
//                }

//                test.Questions.Question.Add(testQuestion);

//            }
//            return test;
//        }
//    }
//}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestServices
{
    public class TestService : ITestService
    {
        private readonly ISerializationService _serializationService;
        private readonly IFileService _fileService;

        public TestService(ISerializationService serializationService, IFileService fileService)
        {
            _serializationService = serializationService;
            _fileService = fileService;
        }

        public async Task<Test> LoadTestFromFileAsync(string filePath)
        {
            string xml = await _fileService.LoadFileAsync(filePath);
            return _serializationService.DeserializeObjectFromXml<Test>(xml);
        }

        public async Task SaveTestToFileAsync(Test test, string filePath)
        {
            string xml = _serializationService.SerializeObjectToXml(test);
            await _fileService.SaveFileAsync(filePath, xml);
        }

        public async Task<Test> AssembleTestAsync(Test test, List<Question> questions)
        {
            test.Questions.Question.Clear();

            foreach (var question in questions)
            {
                var testQuestion = new Question()
                {
                    QuestionText = question.QuestionText,
                    Points = question.Points,
                    Answers = new Answers()
                };
                testQuestion.Answers.Answer = new List<Answer>();
                foreach (var answer in question.Answers.Answer)
                {
                    testQuestion.Answers.Answer.Add(new Answer
                    {
                        TextAnswer = answer.TextAnswer,
                        IsRight = answer.IsRight
                    });
                }

                test.Questions.Question.Add(testQuestion);
            }

            return await Task.FromResult(test);
        }
    }
}

