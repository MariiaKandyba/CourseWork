using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALTest
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }
        public bool IsChecked { get; set; }

        public int UserTestId { get; set; }
        public virtual UserTest UserTest { get; set; }

        public int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
    }

}
