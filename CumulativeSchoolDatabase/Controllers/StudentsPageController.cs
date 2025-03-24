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

    }
}

