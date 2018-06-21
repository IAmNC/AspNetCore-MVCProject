using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPNetCoreMVCProject.Data;
using ASPNetCoreMVCProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASPNetCoreMVCProject.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Departments.Include(d => d.Administrator);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "EmailAddress");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Instructor"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(m => m.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

            if (department == null)
            {
                return NotFound();
            }
            ViewData["Instructor"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)//[Bind("ID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _context.Departments
                .Include(m => m.Administrator)
                .SingleOrDefaultAsync(m => m.ID == id);

            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department has been by another adminitrator.");
                ViewData["Instructor"] = new SelectList(_context.Instructors, "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }

            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate,
                "",
                m => m.Name, m => m.Budget, m => m.StartDate, m => m.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                // Catches if the user is trying to edit database data that is different from  
                // client values shown on the edit page
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. the department was deleted by another adminstrator.");
                    }
                    else
                    {  //Informs the user that the values that are being edited, have been changed during the time the user
                       //has entered the edit page for the specificed department
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current Value: {databaseValues.Name}");
                        }
                        if (databaseValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"Current Value: {databaseValues.Budget}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current Value: {databaseValues.StartDate}");
                        }
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                        {
                            //Used to retrieve instructor name.
                            Instructor databaseInstructor = await _context.Instructors.SingleOrDefaultAsync(m => m.ID == databaseValues.InstructorID);
                            ModelState.AddModelError("InsturctorID", $"Current Value: {databaseInstructor?.FullName}");
                        }
                        ModelState.AddModelError(string.Empty, "The selected department you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List.");
                        departmentToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            ViewData["Instructor"] = new SelectList(_context.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);

        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

            if (department == null)
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

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            try
            {
                if (await _context.Departments.AnyAsync(m => m.ID == department.ID))
                {
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /*ex */)
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = department.ID });
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.ID == id);
        }

    }
}
