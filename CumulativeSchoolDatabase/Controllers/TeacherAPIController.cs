using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CumulativeSchoolDatabase.Models;
using System;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;

namespace CumulativeSchoolDatabase.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        // dependency injection of database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Teachers in the system. 
        /// </summary>
        /// <example>
        /// GET https://localhost:7122/api/Teacher/ListTeachers -> [{"TeacherId":1,"TeacherFname":"Alexander", "TeacherLName":"Bennett"},{"TeacherId":2,"TeacherFname":"Caitlin", "TeacherLName":"Cummings"},..]
        /// </example>
        /// <returns>
        /// A list of teacher objects 
        /// </returns>
        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            // Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for your database
                MySqlCommand Command = Connection.CreateCommand();

          
                //SQL QUERY
                Command.CommandText = "SELECT * FROM teachers";
                

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string? FirstName = ResultSet["teacherfname"].ToString();
                        string? LastName = ResultSet["teacherlname"].ToString();
                       
                        
                        
                        DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        string? EmployeeNumber = ResultSet["employeenumber"].ToString();
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);


                        //short form for setting all properties while creating the object
                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId = Id,
                            TeacherFName = FirstName,
                            TeacherLName = LastName,
                            HireDate = HireDate,
                            EmployeeNumber = EmployeeNumber,
                            Salary = Salary

                        };

                        Teachers.Add(CurrentTeacher);

                    }
                }
            }


            //Return the final list of teachers
     
            return Teachers;
   
        }




        /// <summary>
        /// Returns a teacher in the database by their ID.
        /// </summary>
        /// <example>
        /// GET http://localhost:7122/api/Teacher/FindTeacher/1 -> {"TeacherId":1,"TeacherFname":"Alexander","TeacherLName":"Bennet","HireDate":"2016-08-05", "EmployeeNumber":"T378", "Salary":55.30}
        /// </example>
        /// <returns>
        /// A matching teacher object by its ID. Empty object if teacher not found.
        /// </returns>
        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            // Empty Teacher object
            Teacher SelectedTeacher = new Teacher();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for the database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a sanitized id
                Command.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        // Access Column information by the DB column name as an index

                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string? FirstName = ResultSet["teacherfname"].ToString();
                        string? LastName = ResultSet["teacherlname"].ToString();
                      
                        
                        DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        string? EmployeeNumber = ResultSet["employeenumber"].ToString();
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);



                        SelectedTeacher.TeacherId = Id;
                        SelectedTeacher.TeacherFName = FirstName;
                        SelectedTeacher.TeacherLName = LastName;
                        SelectedTeacher.HireDate = HireDate;
                        SelectedTeacher.EmployeeNumber = EmployeeNumber;
                        SelectedTeacher.Salary = Salary;
                  
                       
                    }
                }
            }

            // Return the teacher object (could be empty if not found)
            return SelectedTeacher;
        }



        /// <summary>
        /// Adds a Teacher to the database
        /// </summary>
        /// <param name="TeacherData">Teacher Object</param>
        /// <example>
        /// POST: https://localhost:7122/api/Teacher/AddTeacher
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "TeacherFname":"Kayla",
        ///     "TeacherLname":"Wilson",
        ///     "HireDate":"2025-04-02",
        ///     "EmployeeNumber":"T550",
        ///     "Salary":85.55 
        /// } -> 409
        /// </example>
        /// <returns>
        /// The inserted Teacher Id from the database if successful. 0 if Unsuccessful
        /// </returns>
        [HttpPost("AddTeacher")]
        public int AddTeacher([FromBody] Teacher TeacherData)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(TeacherData.TeacherFName) || string.IsNullOrWhiteSpace(TeacherData.TeacherLName))
                {
                   
                    return 409;
                }

                // 'using' will close the connection after the code executes
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();
                    // Establish a new command (query) for our database
                    MySqlCommand Command = Connection.CreateCommand();

                    Command.CommandText = "insert into teachers (teacherfname, teacherlname, hiredate, employeenumber, salary) values (@teacherfname, @teacherlname, @hiredate, @employeenumber, @salary)";
                    Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                    Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                    Command.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);
                    Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);
                    Command.Parameters.AddWithValue("@salary", TeacherData.Salary);

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
        /// Deletes a Teacher from the database
        /// </summary>
        /// <param name="TeacherId">Primary key of the teacher to delete</param>
        /// <example>
        /// DELETE: https://localhost:7122/api/Teacher/DeleteTeacher/25
        /// </example>
        /// <returns>
        /// The number of rows affected by the delete operation. 
        /// </returns>
        [HttpDelete("DeleteTeacher/{TeacherId}")]
        public IActionResult DeleteTeacher(int TeacherId)
        {
            try
            {
                using (MySqlConnection Connection = _context.AccessDatabase())
                {
                    Connection.Open();

                    
                    MySqlCommand checkCommand = Connection.CreateCommand();
                    checkCommand.CommandText = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                    checkCommand.Parameters.AddWithValue("@id", TeacherId);

                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        
                        return NotFound("Teacher not found.");
                    }

                    // Proceed with the delete operation
                    MySqlCommand deleteCommand = Connection.CreateCommand();
                    deleteCommand.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                    deleteCommand.Parameters.AddWithValue("@id", TeacherId);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        
                        return Ok($"Teacher with ID {TeacherId} deleted successfully.");
                    }

                   
                    return StatusCode(500, "Error deleting teacher.");
                }
            }
            catch (Exception)
            {
               
                return StatusCode(500, "An error occurred while attempting to delete the teacher.");
            }
        }

        /// <summary>
        /// Updates an Teacher in the database. Data is Teacher object, request query contains ID
        /// </summary>
        /// <param name="TeacherData">Teacher Object</param>
        /// <param name="TeacherId">The Teacher ID primary key</param>
        /// <example>
        /// PUT: https://localhost:7122/api/Teacher/UpdateTeacher/12
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///	    "TeacherFname":"Kayla",
        ///	    "TeacherLname":"Wilson",
        ///	    "HireDate":"2025-04-02",
        ///	    "EmployeeNumber":"T550",
        ///	    "Salary":"85.55"
        /// } -> 
        /// {
        ///     "TeacherId":12,
        ///	    "TeacherFname":"Kayla",
        ///	    "TeacherLname":"Wilson-Jones",
        ///	    "HireDate":"2025-04-02",
        ///	    "EmployeeNumber":"T550",
        ///	    "Salary":"90.00"
        /// }
        /// </example>
        /// <returns>
        /// The updated Teacher object
        /// </returns>
        [HttpPut(template: "UpdateTeacher/{TeacherId}")]
        public ActionResult<Teacher> UpdateTeacher(int TeacherId, [FromBody] Teacher TeacherData)
        {
            
            if (string.IsNullOrWhiteSpace(TeacherData.TeacherFName) || string.IsNullOrWhiteSpace(TeacherData.TeacherLName))
            {
                return BadRequest(new { message = "Teacher first name and last name cannot be empty." });
            }

            
            if (TeacherData.HireDate > DateTime.Now)
            {
                return BadRequest(new { message = "Hire date cannot be in the future." });
            }

            
            if (TeacherData.Salary < 0)
            {
                return BadRequest(new { message = "Salary must be a non-negative value." });
            }

            int rowsAffected = 0;

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "UPDATE teachers SET teacherfname = @teacherfname, teacherlname = @teacherlname, hiredate = @hiredate, employeenumber = @employeenumber, salary = @salary WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);
                Command.Parameters.AddWithValue("@salary", TeacherData.Salary);
                Command.Parameters.AddWithValue("@id", TeacherId);

                rowsAffected = Command.ExecuteNonQuery();
            }

            if (rowsAffected == 0)
            {
                return NotFound(new { message = $"Teacher with ID {TeacherId} not found." });
            }

            return Ok(FindTeacher(TeacherId));
        }
    }
}
