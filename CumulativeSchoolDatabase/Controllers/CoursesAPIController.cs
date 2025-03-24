using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CumulativeSchoolDatabase.Models;
using System;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;

namespace CumulativeSchoolDatabase.Controllers
{
    [Route("api/Courses")]
    [ApiController]
    public class CoursesAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        // dependency injection of database context
        public CoursesAPIController(SchoolDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Courses in the system. 
        /// </summary>
        /// <example>
        /// GET https://localhost:7122/api/Courses/ListCourses -> [{"CourseId":1,"CourseCode":"http5101", "TeacherId":1},{"StartDate":"2018-09-04","FinishDate":"2018-12-14", "CourseName":"Web Application Development"},..]
        /// </example>
        /// <returns>
        /// A list of courses objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListCourses")]
        public List<Courses> ListCourses()
        {
            // Create an empty list of Courses
            List<Courses> Courses = new List<Courses>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for your database
                MySqlCommand Command = Connection.CreateCommand();


                //SQL QUERY
                Command.CommandText = "SELECT * FROM courses";


                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int CourseId = Convert.ToInt32(ResultSet["courseid"]);
                        string? CourseCode = ResultSet["coursecode"].ToString();

                        int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                        DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);
                        string? CourseName = ResultSet["coursename"].ToString();

                        //short form for setting all properties while creating the object
                        Courses CurrentCourses = new Courses()
                        {
                            CourseId = CourseId,
                            CourseCode = CourseCode,
                            TeacherId = TeacherId,
                            StartDate = StartDate,
                            FinishDate = FinishDate,
                            CourseNumber = CourseName,



                        };

                        Courses.Add(CurrentCourses);

                    }
                }
            }


            //Return the final list of courses

            return Courses;

    
        
        }


        /// <summary>
        /// Returns a course in the database by the ID.
        /// </summary>
        /// <example>
        /// GET https://localhost:7122/api/Courses/FindCourse/1-> [{"CourseId":1,"CourseCode":"http5101"},{"CourseName":"Web Application Development"},..]
        /// </example>
        /// <returns>
        /// A matching courses object by its ID. Empty object if courses not found.
        /// </returns>
        [HttpGet]
        [Route(template: "FindCourse/{id}")]
        public Courses FindCourses(int id)
        {
            // Empty Course object
            Courses SelectedCourse = new Courses();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for the database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a sanitized id
                Command.CommandText = "SELECT * FROM courses WHERE courseid = @id";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        // Access Column information by the DB column name as an index

                        int CourseId = Convert.ToInt32(ResultSet["courseid"]);
                        string? CourseCode = ResultSet["coursecode"].ToString();
                        string? CourseName = ResultSet["coursename"].ToString();



                        SelectedCourse.CourseId = CourseId;
                        SelectedCourse.CourseCode = CourseCode;
                        SelectedCourse.CourseNumber = CourseName;




                    }
                }
            }

            // Return the course object (could be empty if not found)
            return SelectedCourse;
        }




    }
}