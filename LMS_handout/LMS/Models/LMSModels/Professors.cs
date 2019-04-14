using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Classes = new HashSet<Classes>();
        }

        public string UId { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public DateTime? Dob { get; set; }
        public string Major { get; set; }

        public virtual Departments MajorNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
