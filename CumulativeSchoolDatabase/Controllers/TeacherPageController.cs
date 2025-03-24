using CumulativeSchoolDatabase.Models;
using Microsoft.AspNetCore.Mvc;


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

    }
}