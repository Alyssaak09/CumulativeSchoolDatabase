using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CumulativeSchoolDatabase.Models;
using System;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;

namespace CumulativeSchoolDatabase.Controllers
{
    [Route("api/Students")]
    [ApiController]
    public class StudentsAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        // dependency injection of database context
        public StudentsAPIController(SchoolDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Students in the system. 
        /// </summary>
        /// <example>
        /// GET https://localhost:7122/api/Students/ListStudents -> [{"StudentId":1,"StudentFname":"Sarah", "StudentLName":"Valdez"},{"StudentId":2,"StudentFname":"Jennifer", "StudentLName":"Faulkner"},..]
        /// </example>
        /// <returns>
        /// A list of students objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListStudents")]
        public List<Students> ListStudents()
        {
            // Create an empty list of Students
            List<Students> Students = new List<Students>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for your database
                MySqlCommand Command = Connection.CreateCommand();


                //SQL QUERY
                Command.CommandText = "SELECT * FROM students";


                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["studentid"]);
                        string? FirstName = ResultSet["studentfname"].ToString();
                        string? LastName = ResultSet["studentlname"].ToString();
                        string? Number = ResultSet["studentnumber"].ToString();
                        DateTime EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]);


                        //short form for setting all properties while creating the object
                        Students CurrentStudent = new Students()
                        {
                            StudentId = Id,
                            StudentFName = FirstName,
                            StudentLName = LastName,
                            StudentNumber = Number,
                            EnrolDate = EnrolDate,


                        };

                        Students.Add(CurrentStudent);

                    }
                }
            }


            //Return the final list of students

            return Students;

        }




        /// <summary>
        /// Returns a student in the database by their ID.
        /// </summary>
        /// <example>
        /// GET https://localhost:7122/api/Students/FindStudent/1 -> {"StudentId":1,"StudentFname":"Sarah","StudentLName":"Valdez","StudentNumber":"N1678","EnrolDate":"2018-06-18"}
        /// </example>
        /// <returns>
        /// A matching student object by its ID. Empty object if student not found.
        /// </returns>
        [HttpGet]
        [Route(template: "FindStudent/{id}")]
        public Students FindStudent(int id)
        {
            // Empty Student object
            Students SelectedStudent = new Students();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for the database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a sanitized id
                Command.CommandText = "SELECT * FROM students WHERE studentid = @id";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        // Access Column information by the DB column name as an index

                        int Id = Convert.ToInt32(ResultSet["studentid"]);
                        string? FirstName = ResultSet["studentfname"].ToString();
                        string? LastName = ResultSet["studentlname"].ToString();
                        string? Number = ResultSet["studentnumber"].ToString();
                        DateTime EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]);



                        SelectedStudent.StudentId = Id;
                        SelectedStudent.StudentFName = FirstName;
                        SelectedStudent.StudentLName = LastName;
                        SelectedStudent.StudentNumber = Number;
                        SelectedStudent.EnrolDate = EnrolDate;



                    }
                }
            }

            // Return the student object (could be empty if not found)
            return SelectedStudent;
        }

    }
}
