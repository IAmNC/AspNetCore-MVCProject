using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCoreProject.Models;
using Microsoft.Extensions.DependencyInjection;


namespace AspNetCoreProject.Data
{
    public class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Students.Any())
            {
                return;   // DB has been seeded
            }

            var instructors = new Instructor[]
            {
                new Instructor{FirstMidName ="Andrew", LastName ="Ostasz",
                    EmailAddress ="AndrewO@Email.com", HiredDate =DateTime.Parse("1999-06-03") },
                new Instructor{FirstMidName ="Scott", LastName ="Davidson",
                    EmailAddress ="Austensd@Email.com", HiredDate =DateTime.Parse("2000-02-03") },
                new Instructor{FirstMidName ="Daniel", LastName ="Proctor",
                    EmailAddress ="DanPo@Email.com", HiredDate =DateTime.Parse("1998-12-03") },
                new Instructor{FirstMidName ="Nicky", LastName ="Chau",
                    EmailAddress ="NickyChau@Email.com", HiredDate =DateTime.Parse("2000-07-03") },
                new Instructor{FirstMidName ="William", LastName ="Jones",
                    EmailAddress ="WilliamJ@Email.com", HiredDate =DateTime.Parse("2000-07-03") }
            };

            foreach (Instructor s in instructors)
            {
                context.Instructors.Add(s);
            }
            context.SaveChanges();


            var students = new Student[]
            {
                new Student{FirstMidName="John",LastName="Carson",
                    EnrollmentDate =DateTime.Parse("2001-09-12"), EmailAddress="StuEmail@mmu.com"},
                new Student{FirstMidName="Lisa",LastName="Luu",
                    EnrollmentDate =DateTime.Parse("2002-09-12"), EmailAddress="StuEmail@mmu.com"},
                new Student{FirstMidName="Freddie",LastName="Mecury",
                    EnrollmentDate =DateTime.Parse("2001-09-12"), EmailAddress="StuEmail@mmu.com"},
                new Student{FirstMidName="Kendrick",LastName="Lamar",
                    EnrollmentDate =DateTime.Parse("2003-09-12"), EmailAddress="StuEmail@mmu.com"},
                new Student{FirstMidName="Rock",LastName="Lee",
                    EnrollmentDate =DateTime.Parse("2005-09-12"), EmailAddress="StuEmail@mmu.com"},
                new Student{FirstMidName="Laura",LastName="Wilson",
                    EnrollmentDate =DateTime.Parse("2001-09-12"), EmailAddress="StuEmail@mmu.com"},
                new Student{FirstMidName="Steve",LastName="Irwin",
                    EnrollmentDate =DateTime.Parse("2003-09-12"), EmailAddress="StuEmail@mmu.com"},
                new Student{FirstMidName="Alex",LastName="Ovenchkin",
                    EnrollmentDate =DateTime.Parse("2000-09-12"), EmailAddress="StuEmail@mmu.com"}
            };
            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();


            var departments = new Department[]
          {
                new Department{InstructorID= instructors.Single(i => i.LastName=="Proctor").ID, Budget=500000,
                    Name ="School of Computing, Mathematics and Digital Technology", StartDate=DateTime.Parse("1990-09-01")},
                new Department{InstructorID= instructors.Single(i => i.LastName=="Ostasz").ID, Budget=500000,
                    Name ="Humanities, Languages and Social Science", StartDate=DateTime.Parse("1990-09-01")},
                new Department{InstructorID= instructors.Single(i => i.LastName=="Chau").ID, Budget=500000,
                    Name ="Manchester School of Art", StartDate=DateTime.Parse("1990-09-01")},
                new Department{InstructorID= instructors.Single(i => i.LastName=="Davidson").ID, Budget=500000,
                    Name ="Economics, Policy and International Business", StartDate=DateTime.Parse("1990-09-01")},
                new Department{InstructorID= instructors.Single(i => i.LastName=="Jones").ID, Budget=500000,
                    Name ="School of Science and The Environment", StartDate=DateTime.Parse("1990-09-01")}
          };
            foreach (Department s in departments)
            {
                context.Departments.Add(s);
            }
            context.SaveChanges();


            var courses = new Course[]
            {
                new Course{CourseID=2071,Title="Chemistry",Credits=30,
                    DepartmentID = departments.Single(i => i.Name == "School of Science and The Environment").ID},
                new Course{CourseID=4044,Title="Microeconomics",Credits=30,
                    DepartmentID = departments.Single(i => i.Name == "Economics, Policy and International Business").ID},
                new Course{CourseID=4045,Title="Macroeconomics",Credits=30,
                    DepartmentID = departments.Single(i => i.Name == "Economics, Policy and International Business").ID},
                new Course{CourseID=1033,Title="Calculus",Credits=30,
                    DepartmentID = departments.Single(i => i.Name == "School of Computing, Mathematics and Digital Technology").ID},
                new Course{CourseID=1043,Title="Trigonometry",Credits=30,
                    DepartmentID = departments.Single(i => i.Name == "School of Computing, Mathematics and Digital Technology").ID},
                new Course{CourseID=3025,Title="Composition",Credits=30,
                    DepartmentID = departments.Single(i => i.Name == "Manchester School of Art").ID},
                new Course{CourseID=1011,Title="Literature",Credits=30,
                    DepartmentID = departments.Single(i => i.Name == "Humanities, Languages and Social Science").ID}
            };
            foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            context.SaveChanges();


            var courseInstructor = new CourseAssignment[]
            {
                new CourseAssignment
                {
                    CourseID = courses.Single(v => v.Title == "Chemistry").CourseID,
                    InstructorID = instructors.Single(v => v.LastName =="Ostasz").ID
                },
                 new CourseAssignment
                {
                    CourseID = courses.Single(v => v.Title == "Microeconomics").CourseID,
                    InstructorID = instructors.Single(v => v.LastName =="Chau").ID
                },
                  new CourseAssignment
                {
                    CourseID = courses.Single(v => v.Title == "Macroeconomics").CourseID,
                    InstructorID = instructors.Single(v => v.LastName =="Proctor").ID
                },
                   new CourseAssignment
                {
                    CourseID = courses.Single(v => v.Title == "Calculus").CourseID,
                    InstructorID = instructors.Single(v => v.LastName =="Davidson").ID
                },
                    new CourseAssignment
                {
                    CourseID = courses.Single(v => v.Title == "Trigonometry").CourseID,
                    InstructorID = instructors.Single(v => v.LastName =="Davidson").ID
                },
                      new CourseAssignment
                {
                    CourseID = courses.Single(v => v.Title == "Composition").CourseID,
                    InstructorID = instructors.Single(v => v.LastName =="Jones").ID
                },
                        new CourseAssignment
                {
                    CourseID = courses.Single(v => v.Title == "Literature").CourseID,
                    InstructorID = instructors.Single(v => v.LastName =="Proctor").ID
                }
            };
            foreach (CourseAssignment ci in courseInstructor)
            {
                context.CourseAssignments.Add(ci);
            }
            context.SaveChanges();



            var enrollments = new Enrollment[]
            {  new Enrollment {
                StudentID = students.Single(s => s.LastName == "Carson").ID,
                CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                Grade = Grade.A
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Luu").ID,
                CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                Grade = Grade.C
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Luu").ID,
                CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Wilson").ID,
                CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                Grade = Grade.B
                },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Wilson").ID,
                CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                Grade = Grade.B
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Lamar").ID,
                CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                Grade = Grade.B
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Ovenchkin").ID,
                CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Mecury").ID,
                CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
                Grade = Grade.B
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Irwin").ID,
                CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
                Grade = Grade.B
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Lee").ID,
                CourseID = courses.Single(c => c.Title == "Composition").CourseID,
                Grade = Grade.B
                },
                new Enrollment {
                StudentID = students.Single(s => s.LastName == "Mecury").ID,
                CourseID = courses.Single(c => c.Title == "Literature").CourseID,
                Grade = Grade.B
                },
                   new Enrollment {
                StudentID = students.Single(s => s.LastName == "Lamar").ID,
                CourseID = courses.Single(c => c.Title == "Literature").CourseID,
                Grade = Grade.A
                }
            };
            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                        s.Student.ID == e.StudentID && s.Course.CourseID == e.CourseID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();
        }
    }
}
