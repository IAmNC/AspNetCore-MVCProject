using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNetCoreMVCProject.Models
{
    public class Instructor
    {
        public int ID { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(70)]
        [Column("Instructor First Name")]
        [Display(Name = "First Middle Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hired Date")]
        public DateTime HiredDate { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Timestamp]
        public byte[] InstructorRowVersion { get; set; }

        public string FullName
        {
            get { return LastName + ", " + FirstMidName; }
        }


        public ICollection<CourseAssignment> CourseAssignments { get; set; }
    }
}
