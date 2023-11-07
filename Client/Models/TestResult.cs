using DALTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class TestResult
    {
        public Test Test { get; set; }
        //public int UserTestId {  get; set; }

        public List<UserAnswer> UserAnswers { get; set; }
    }
}
