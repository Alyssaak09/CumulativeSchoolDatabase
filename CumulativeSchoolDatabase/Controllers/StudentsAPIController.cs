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


        /// <summary>
        /// Adds a Student to the database
        /// </summary>
        /// <param name="StudentsData">Student Object</param>
        /// <example>
        /// POST: https://localhost:7122/api/Students/AddStudent
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "StudentFname":"Lisa",
        ///     "StudentLname":"William",
        ///     "StudentNumber":"N1888",
        ///     "EnrolDate":"2025-04-02",
        /// } -> 409
        /// </example>
        /// <returns>
        /// The inserted StudentId from the database if successful. 0 if Unsuccessful
        /// </returns>
        [HttpPost("AddStudent")]
        public int AddStudent([FromBody] Students StudentsData)
        {
            try
            {
                // 'using' will close the connection after the code executes
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();
                    // Establish a new command (query) for our database
                    MySqlCommand Command = Connection.CreateCommand();

                    Command.CommandText = "insert into students (studentfname, studentlname, studentnumber, enroldate) values (@studentfname, @studentlname, @studentnumber, @enroldate)";
                    Command.Parameters.AddWithValue("@studentfname", StudentsData.StudentFName);
                    Command.Parameters.AddWithValue("@studentlname", StudentsData.StudentLName);
                    Command.Parameters.AddWithValue("@studentnumber", StudentsData.StudentNumber);
                    Command.Parameters.AddWithValue("@enroldate", StudentsData.EnrolDate);

                    // Execute the query
                    int rowsAffected = Command.ExecuteNonQuery();

                   
                    if (rowsAffected > 0)
                    {
                        
                        return Convert.ToInt32(Command.LastInsertedId);
                    }
                    else
                    {
                       
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"Error: {ex.Message}");

                
                return 0;
            }
        }


        /// <summary>
        /// Deletes a Student from the database
        /// </summary>
        /// <param name="StudentId">Primary key of the student to delete</param>
        /// <example>
        /// DELETE: https://localhost:7122/api/Students/DeleteStudent/43
        /// </example>
        /// <returns>
        /// Number of rows affected by delete operation.
        /// </returns>
        [HttpDelete("DeleteStudent/{StudentId}")]
        public IActionResult DeleteStudent(int StudentId)
        {
            try
            {
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();

                    // Check if student exists before attempting delete
                    MySqlCommand checkCommand = Connection.CreateCommand();
                    checkCommand.CommandText = "SELECT COUNT(*) FROM students WHERE studentid = @id";
                    checkCommand.Parameters.AddWithValue("@id", StudentId);

                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        // Student doesn't exist
                        return NotFound("Student not found.");
                    }

                    // Proceed with the delete operation
                    MySqlCommand deleteCommand = Connection.CreateCommand();
                    deleteCommand.CommandText = "DELETE FROM students WHERE studentid = @id";
                    deleteCommand.Parameters.AddWithValue("@id", StudentId);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    
                    if (rowsAffected > 0)
                    {
                        return Ok($"Student with ID {StudentId} deleted successfully.");
                    }

                    
                    return StatusCode(500, "Error deleting student.");
                }
            }
            catch (Exception)
            {
               
                return StatusCode(500, "An error occurred while attempting to delete the student.");
            }
        }


    }
}

