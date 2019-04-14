using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public uint AId { get; set; }
        public string Name { get; set; }
        public uint? Value { get; set; }
        public string Contents { get; set; }
        public DateTime? DueDate { get; set; }
        public uint CatId { get; set; }

        public virtual AssignmentCategories Cat { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
