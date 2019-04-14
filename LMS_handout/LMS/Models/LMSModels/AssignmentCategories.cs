using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategories
    {
        public AssignmentCategories()
        {
            Assignments = new HashSet<Assignments>();
        }

        public uint CatId { get; set; }
        public string Name { get; set; }
        public uint Class { get; set; }
        public uint? Weight { get; set; }

        public virtual Classes ClassNavigation { get; set; }
        public virtual ICollection<Assignments> Assignments { get; set; }
    }
}
