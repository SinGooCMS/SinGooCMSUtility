using NTFxTest.Locale;
using SinGooCMS.Utility.Extension;
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
        [DisplayExt("姓名", "Name", typeof(Resource))]
        public string UserName { get; set; }
        [Display(Name = "年龄")]
        public int Age { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Birthday { get; set; } = new DateTime(1900, 1, 1);
        public DateTime AutoTimeStamp { get; set; } = new DateTime(1900, 1, 1);

        public int score = 100;
    }
}
