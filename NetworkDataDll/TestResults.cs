using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDataDll
{
    [Serializable]
    public class TestResults
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
        public string Img { get; set; }
        public List<AnswerModel> Answers { get; set; }
        public List<UserAnswerModel> UserAnswers { get; set; }


    }
    [Serializable]

    public class AnswerModel
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
    }
    [Serializable]

    public class UserAnswerModel
    {
        public int AnswerId { get; set; }
        public bool IsChecked { get; set; }
    }

}
