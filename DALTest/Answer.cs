using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public bool IsRight { get; set; }

        // Зв'язок з питанням
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}
