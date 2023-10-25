using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    public class UserTest
    {
        public int Id { get; set; }
        public int PointsGrade { get; set; }
        public bool IsPassed { get; set; }
        public DateTime TakenDate { get; set; }
        public bool IsTaken { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int TestId { get; set; }
        public virtual Test Test { get; set; }
    }
}
