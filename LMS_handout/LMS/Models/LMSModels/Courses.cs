using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public uint CId { get; set; }
        public string Name { get; set; }
        public uint? Number { get; set; }
        public string Abbr { get; set; }

        public virtual Departments AbbrNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
