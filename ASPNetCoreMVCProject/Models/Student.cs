using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace ASPNetCoreMVCProject.Models
{
    public class Student
    {
        public int ID { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(70)]
        [Display(Name = "First Middle Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [Required] 
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstMidName; }
        }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
