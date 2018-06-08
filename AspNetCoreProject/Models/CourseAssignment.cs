using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreProject.Models
{
    public class CourseAssignment
    {

        public int InstructorID { get; set; }
        public int CourseID { get; set; }

        [Timestamp]
        public byte[] CourseAssignmentRowVersion { get; set; }

        public Instructor Instructor { get; set; }
        public Course Course { get; set; }

    }
}
