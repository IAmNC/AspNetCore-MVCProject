using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreProject.Models
{
    public class Department
    {
        public int ID { get; set; }

        [StringLength(128, MinimumLength = 3)]
        [Display(Name= "Department Name")]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        
        public int? InstructorID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<Course> Courses { get; set; }
        public Instructor Administrator { get; set; }


    }
}
