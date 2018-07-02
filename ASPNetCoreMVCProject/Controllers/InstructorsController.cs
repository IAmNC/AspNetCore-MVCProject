using ASPNetCoreMVCProject.Data;
using ASPNetCoreMVCProject.Models;
using ASPNetCoreMVCProject.Models.UniversityViewModel;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetCoreMVCProject.Controllers
{
    [Authorize]
    public class InstructorsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly DbConnection _connection;
        private readonly string ConnStr;

        public InstructorsController(ApplicationDbContext context, DbConnection connection)
        {
           
            _context = context;

            //Connection for Dapper Query
            //_connection = connection;
            //Used for Async Dapper Queries
            ConnStr = connection.ConnectionString;
            
        }


        // GET: Instructors --- 
        // Testing Dapper Micro ORM 
        [AllowAnonymous]
        public async Task<IActionResult> IndexDapp(string sortOrder, int? id, int? courseID)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            var ViewM = new List<Instructor>();
          
         //  List<Instructor> ViewM = new List<Instructor>();
            using (var connection = new SqlConnection(ConnStr))
            {

                await connection.OpenAsync();

                var sql = "SELECT* FROM Instructor ORDER BY LastName";

    
               ViewM = connection.QueryAsync<Instructor>(sql).Result.ToList();
  

                switch (sortOrder)
                {
                    case "name_desc":
                        ViewM = connection.QueryAsync<Instructor>("SELECT * FROM Instructor ORDER BY LastName Desc").Result.ToList();
                        break;
                    case "Date":
                        ViewM = connection.QueryAsync<Instructor>("SELECT * FROM Instructor ORDER BY HiredDate").Result.ToList();
                        break;
                    case "date_desc":
                        ViewM = connection.QueryAsync<Instructor>("SELECT * FROM Instructor ORDER BY HiredDate Desc").Result.ToList();
                        break;
                    default:
                        ViewM = connection.QueryAsync<Instructor>("SELECT * FROM Instructor ORDER BY LastName ").Result.ToList();
                        break;

                }

                return View(ViewM);
            }

            /*
             * Used normal connection that is not Async
            using (IDbConnection dbConnection = _connection)
            {   
                    dbConnection.Open();
                ViewM = _connection.Query<Instructor>("SELECT * FROM Instructor ORDER BY LastName").ToList();

                switch (sortOrder)
                {
                    case "name_desc":
                        ViewM = _connection.Query<Instructor>("SELECT * FROM Instructor ORDER BY LastName Desc").ToList();
                        break;
                    case "Date":
                        ViewM = _connection.Query<Instructor>("SELECT * FROM Instructor ORDER BY HiredDate").ToList();
                        break;
                    case "date_desc":
                        ViewM = _connection.Query<Instructor>("SELECT * FROM Instructor ORDER BY HiredDate Desc").ToList();
                        break;
                    default:
                        ViewM = _connection.Query<Instructor>("SELECT * FROM Instructor ORDER BY LastName ").ToList();
                        break;
                }
            }
            */
      
        }


        // GET: Instructors
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, int? id, int? courseID)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var viewModel = new InstructorIndexData
            {
                Instructors = await _context.Instructors
                  .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                  .OrderBy(i => i.LastName)
                  .ToListAsync()
            };

            switch (sortOrder)
            {
                case "name_desc":
                    viewModel.Instructors = await _context.Instructors
                                                  .OrderByDescending(i => i.LastName)
                                                  .ToListAsync();
                    break;
                case "Date":
                    viewModel.Instructors = await _context.Instructors
                                                  .OrderBy(i => i.HiredDate)
                                                  .ToListAsync();
                    break;
                case "date_desc":
                    viewModel.Instructors = await _context.Instructors
                                                  .OrderByDescending(i => i.HiredDate)
                                                  .ToListAsync();
                    break;
                default:
                    viewModel.Instructors = await _context.Instructors
                                                  .OrderBy(i => i.LastName)
                                                  .ToListAsync();
                    break;

            }

            //Shows all the courses the instructor is teaching
            if (id != null)
            {
                ViewData["InstructorID"] = id.Value;
                Instructor instructor = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single();
                viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }

            //Shows all students and their grade in the course after selected the specified course
            if (courseID != null)
            {
                ViewData["CourseID"] = courseID.Value;
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
                await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
                }
                viewModel.Enrollments = selectedCourse.Enrollments;
            }


            return View(viewModel);
        }

        // GET: Instructors/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FromSql("SELECT * From Instructor WHERE ID = {0}", id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,HiredDate,EmailAddress, InstructorRowVersion")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

            if (instructor == null)
            {
                return NotFound();
            }

            PopulateInstructorsCourse(instructor);
            return View(instructor);
        }


        private void PopulateInstructorsCourse(Instructor instructor)
        {
            var allCourse = _context.Courses;
            var instructorCourse = new HashSet<int>(instructor.CourseAssignments.Select(m => m.CourseID));
            var viewModel = new List<InstructorCourseData>();
            foreach (var course in allCourse)
            {
                viewModel.Add(new InstructorCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourse.Contains(course.CourseID)
                });
            }
            ViewData["Courses"] = viewModel;
        }


        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourse, byte[] instructorRowVersion) // [Bind("ID,LastName,FirstMidName,HiredDate,EmailAddress")] Instructor instructor)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                .SingleOrDefaultAsync(m => m.ID == id);


            if (instructorToUpdate == null)
            {
                Instructor deletedInstructor = new Instructor();
                await TryUpdateModelAsync(deletedInstructor);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The selected instructor has been by deleted another adminitrator.");

                return View(deletedInstructor);
            }

            _context.Entry(instructorToUpdate).Property("InstructorRowVersion").OriginalValue = instructorRowVersion;

            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "",
                i => i.FirstMidName, i => i.LastName, i => i.HiredDate, i => i.EmailAddress))
            {
                UpdateInstructorCourses(selectedCourse, instructorToUpdate);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Instructor)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();



                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save change. The selected instructor has been deleted by another administrator");
                    }
                    else
                    {

                        var databaseValues = (Instructor)databaseEntry.ToObject();

                        if (databaseValues.LastName != clientValues.LastName)
                        {
                            ModelState.AddModelError("LastName", $"Current Value: {databaseValues.LastName}");
                        }
                        if (databaseValues.FirstMidName != clientValues.FirstMidName)
                        {
                            ModelState.AddModelError("FirstMidName", $"Current Value: {databaseValues.FirstMidName}");
                        }
                        if (databaseValues.HiredDate != clientValues.HiredDate)
                        {
                            ModelState.AddModelError("HiredDate", $"Current Value: {databaseValues.HiredDate}");
                        }
                        if (databaseValues.EmailAddress != clientValues.EmailAddress)
                        {
                            ModelState.AddModelError("EmailAddress", $"Current Value: {databaseValues.FirstMidName}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                           + "was modified by another user after you got the original value. The "
                           + "edit operation was canceled and the current values in the database "
                           + "have been displayed. If you still want to edit this record, click "
                           + "the Save button again. Otherwise click the Back to List.");
                        instructorToUpdate.InstructorRowVersion = (byte[])databaseValues.InstructorRowVersion;
                        ModelState.Remove("InstructorRowVersion");

                    }
                }
            }
            UpdateInstructorCourses(selectedCourse, instructorToUpdate);
            PopulateInstructorsCourse(instructorToUpdate);
            return View(instructorToUpdate);
        }


        private void UpdateInstructorCourses(string[] selectedCourse, Instructor instructorToUpdate)
        {
            if (selectedCourse == null)
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourse);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.CourseAssignments.Select(c => c.Course.CourseID));
            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = course.CourseID });
                    }
                }
                else
                {

                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }
        }


        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .SingleOrDefaultAsync(m => m.ID == id);

            if (instructor == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = ("The record you attempted to delete "
                          + "was modified by another user after you got the original value. The "
                          + "delete operation was canceled and the current values in the database "
                          + "have been displayed. If you still want to delete this record, click "
                          + "the Delete button again. Otherwise click the Back to List.");
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Instructor instructor)
        {
            try
            {
                if (await _context.Instructors.AnyAsync(m => m.ID == instructor.ID))
                {
                    var departments = await _context.Departments
                    .Where(m => m.InstructorID == instructor.ID)
                    .ToListAsync();
                    departments.ForEach(m => m.InstructorID = null);

                    _context.Instructors.Remove(instructor);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = instructor.ID });
            }
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
