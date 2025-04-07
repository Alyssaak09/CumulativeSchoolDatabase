using CumulativeSchoolDatabase.Models;
using Microsoft.AspNetCore.Mvc;


namespace CumulativeSchoolDatabase.Controllers
{
    public class StudentsPageController : Controller
    {
        // Rely on the API to retrieve students information
        // Both the StudentsAPI and StudentsPage controllers should rely on a unified "Service", with an explicit interface
        private readonly StudentsAPIController _api;

        public StudentsPageController(StudentsAPIController api)
        {
            _api = api;
        }

        //GET : Students/List
        [HttpGet]
        public IActionResult List()
        {
            List<Students> Students = _api.ListStudents();
            return View(Students);
        }

        //GET : Students/Show/{id}
        [HttpGet]
        public IActionResult Show(int id)
        {
            Students SelectedStudent = _api.FindStudent(id);
            return View(SelectedStudent);
        }

        // GET : StudentsPage/New
        [HttpGet]
        public IActionResult New()
        {
            
            return View(new Students());
        }

        // POST: StudentsPage/Create
        [HttpPost]
        public IActionResult Create(Students NewStudent)
        {
            
            if (!ModelState.IsValid)
            {
                
                return View("New", NewStudent);
            }

          
            int StudentId = _api.AddStudent(NewStudent);

           
            if (StudentId > 0)
            {
                // Redirect to the "Show" action to view the newly created student
                return RedirectToAction("Show", new { id = StudentId });
            }
            else
            {
                
                ModelState.AddModelError("", "Failed to create student. Please try again.");
               
                return View("New", NewStudent);
            }
        }

        // GET : StudentsPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Students SelectedStudent = _api.FindStudent(id);

            if (SelectedStudent == null)
            {
                
                TempData["ErrorMessage"] = "Student not found.";
                return RedirectToAction("List");
            }

            return View(SelectedStudent);
        }

        // POST: StudentsPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            
            var result = _api.DeleteStudent(id);

            if (result is OkObjectResult)
            {
                
                TempData["SuccessMessage"] = "Student successfully deleted.";
            }
            else if (result is NotFoundObjectResult)
            {
               
                TempData["ErrorMessage"] = "Student not found.";
            }
            else
            {
               
                TempData["ErrorMessage"] = "An error occurred while deleting the student.";
            }

         
            return RedirectToAction("List");
        }

    }
}


