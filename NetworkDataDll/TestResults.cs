using DALTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace NetworkDataDll
{
    [Serializable]
    public class TestResults
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Info { get; set; }
        public int PassPercent { get; set; }
        public DateTime LoadedDate { get; set; }
        public int PointsGrade { get; set; }
        public double ScoredPercent { get; set; }
        public bool IsPassed { get; set; }
        public DateTime TakenDate { get; set; }
        public bool IsTaken { get; set; }
        public int TotalPossiblePoints { get; set; }

        public List<QuestionModel> Questions { get; set; }
    }
    [Serializable]

    public class QuestionModel
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string QuestionText { get; set; }
        public string Img { get; set; }
        public List<AnswerModel>  Answers { get; set; }
    }
    [Serializable]

    public class AnswerModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsChecked { get; set; }

    }


}
