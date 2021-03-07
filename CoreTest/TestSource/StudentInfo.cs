using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoreTest
{
    [Serializable]
    [Table("Student")]
    public class StudentInfo
    {
        [Key]
        public int AutoID { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime AutoTimeStamp { get; set; }

        public int score = 100;
    }
}
