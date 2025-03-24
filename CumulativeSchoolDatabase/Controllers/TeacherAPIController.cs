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

    }
}
