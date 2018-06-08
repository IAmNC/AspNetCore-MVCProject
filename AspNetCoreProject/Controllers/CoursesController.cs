using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreProject.Data;
using AspNetCoreProject.Models;

namespace AspNetCoreProject.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Courses.Include(c => c.Department);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            // ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "ID");
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Credits,DepartmentID,Title")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, byte[] courseRowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await _context.Courses
                .SingleOrDefaultAsync(c => c.CourseID == id);
            if (courseToUpdate == null)
            {
                Course deletedCourse = new Course();
                await TryUpdateModelAsync(deletedCourse);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The selected course has been deleted by another administrator");

                return View(deletedCourse);
            }

            _context.Entry(courseToUpdate).Property("CourseRowVersion").OriginalValue = courseRowVersion;


            if (await TryUpdateModelAsync<Course>(courseToUpdate,
                "",
                c => c.Credits, c => c.DepartmentID, c => c.Title))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException  ex )
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Course)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save change. The selected instructor has been deleted by another administrator");
                    }
                    else
                    {
                        var databaseValues = (Course)databaseEntry.ToObject();
                        
                        if (databaseValues.Title != clientValues.Title)
                        {
                            ModelState.AddModelError("Title", $"Current Value: { databaseValues.Title}");
                        }
                        if (databaseValues.Credits != clientValues.Credits)
                        {
                            ModelState.AddModelError("Credits", $"Current Value: { databaseValues.Credits}");
                        }
                        if (databaseValues.DepartmentID != clientValues.DepartmentID)
                        {
                            Department departmentName = await _context.Departments.SingleOrDefaultAsync(m => m.ID == databaseValues.DepartmentID);
                            ModelState.AddModelError("DepartmentID", $"Current Value: { departmentName.Name}");
                        }
                        ModelState.AddModelError(string.Empty, "The selected course you attempted to edit "
                                                   + "was modified by another user after you got the original value. The "
                                                   + "edit operation was canceled and the current values in the database "
                                                   + "have been displayed. If you still want to edit this record, click "
                                                   + "the Save button again. Otherwise click the Back to List.");
                        courseToUpdate.CourseRowVersion= (byte[])databaseValues.CourseRowVersion;
                        ModelState.Remove("CourseRowVersion");
                    }
                }
            }

            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "ID", "Name", selectedDepartment);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);

            if (course == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = ("The selected course you attempted to delete "
                         + "was modified by another user after you got the original value. The "
                         + "delete operation was canceled and the current values in the database "
                         + "have been displayed. If you still want to delete this record, click "
                         + "the Delete button again. Otherwise click the Back to List.");
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Course course)
        {
            try
            {
                if (await _context.Courses.AnyAsync(m => m.CourseID == course.CourseID))
                {
                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = course.CourseID });
            }
          
          
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }
    }
}
