using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DALTest.Entities
{
    [Serializable]
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Img { get; set; }
        public int Points { get; set; }

        public int TestId { get; set; }
        public virtual Test Test { get; set; }

        [JsonIgnore]

        public virtual ICollection<Answer> Answers { get; set; }
    }
}
