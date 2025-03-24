using CumulativeSchoolDatabase.Models;
using Microsoft.AspNetCore.Mvc;


namespace CumulativeSchoolDatabase.Controllers
{
    public class CoursesPageController : Controller
    {
        // Rely on the API to retrieve students information
        // Both the CoursesAPI and CoursesPage controllers should rely on a unified "Service", with an explicit interface
        private readonly CoursesAPIController _api;

        public CoursesPageController(CoursesAPIController api)
        {
            _api = api;
        }

        //GET : Courses/List
        [HttpGet]
        public IActionResult List()
        {
            List<Courses> Courses = _api.ListCourses();
            return View(Courses);
        }

        //GET : Courses/Show/{id}
        [HttpGet]
        public IActionResult Show(int id)
        {
            Courses SelectedCourse = _api.FindCourses(id);
            return View(SelectedCourse);
        }



    }
}