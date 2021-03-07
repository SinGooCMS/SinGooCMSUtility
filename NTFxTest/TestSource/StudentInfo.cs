using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NTFxTest
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
        public DateTime Birthday { get; set; } = new DateTime(1900, 1, 1);
        public DateTime AutoTimeStamp { get; set; } = new DateTime(1900, 1, 1);

        public int score = 100;
    }
}
