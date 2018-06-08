using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AspNetCoreProject.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 30)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        [Timestamp]
        public byte[] CourseRowVersion { get; set; }

        public Department Department { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }



    }
}
