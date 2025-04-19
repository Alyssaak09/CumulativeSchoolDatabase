using System;
using CumulativeSchoolDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Datatypes;


namespace CumulativeSchoolDatabase.Controllers
{
    public class TeacherPageController : Controller
    {
        // Rely on the API to retrieve teacher information
        // Both the TeacherAPI and TeacherPage controllers should rely on a unified "Service", with an explicit interface
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }

        //GET : Teacher/List
        [HttpGet]
        public IActionResult List()
        {
            List<Teacher> Teachers = _api.ListTeachers();
            return View(Teachers);
        }

        //GET : Teacher/Show/{id}
        [HttpGet]
        public IActionResult Show(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }


        // GET : TeacherPage/New
        [HttpGet]
        public IActionResult New()
        {

            return View(new Teacher());
        }

        // POST: TeacherPage/Create
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {

            if (string.IsNullOrWhiteSpace(NewTeacher.TeacherFName) || string.IsNullOrWhiteSpace(NewTeacher.TeacherLName))
            {

                ModelState.AddModelError("", "Teacher's first and last name are required.");


                return View("New", NewTeacher);
            }


            if (!ModelState.IsValid)
            {

                return View("New", NewTeacher);
            }


            int TeacherId = _api.AddTeacher(NewTeacher);


            if (TeacherId > 0)
            {
                // Redirect to the "Show" action to view the newly created teacher
                return RedirectToAction("Show", new { id = TeacherId });
            }
            else
            {

                ModelState.AddModelError("", "Failed to create teacher. Please try again.");

                return View("New", NewTeacher);
            }
        }


        // GET : TeacherPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);

            if (SelectedTeacher == null)
            {
                // Handle the case where the teacher doesn't exist
                TempData["ErrorMessage"] = "Teacher not found.";
                return RedirectToAction("List");
            }

            return View(SelectedTeacher);
        }


        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {

            var result = _api.DeleteTeacher(id);


            if (result is OkObjectResult)
            {
                TempData["SuccessMessage"] = "Teacher successfully deleted.";
            }

            else if (result is NotFoundObjectResult)
            {
                TempData["ErrorMessage"] = "Teacher not found.";
            }

            else if (result is StatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 500)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the teacher.";
            }


            return RedirectToAction("List");
        }

        // GET : TeacherPage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        // POST: TeacherPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string TeacherFName, string TeacherLName, DateTime HireDate, string EmployeeNumber, decimal salary)
        {
            Teacher UpdatedTeacher = new Teacher();
            UpdatedTeacher.TeacherFName = TeacherFName;
            UpdatedTeacher.TeacherLName = TeacherLName;
            UpdatedTeacher.HireDate = HireDate;
            UpdatedTeacher.EmployeeNumber = EmployeeNumber;
            UpdatedTeacher.Salary = salary;

            _api.UpdateTeacher(id, UpdatedTeacher);

            return RedirectToAction("Show", new { id = id });
        }

    }
}