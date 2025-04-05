using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative1.Models;
using System;
using MySql.Data.MySqlClient;
using Cumulative1.Models;
using Mysqlx.Datatypes;
using MySqlX.XDevAPI.Common;


namespace Cumulative1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchooldbContext _context;
        public TeacherAPIController(SchooldbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves a list of teachers, including their courses, with an optional filter by hire date range.
        /// </summary>
        /// <param name="StartDate">The start date of the hire date range (optional).</param>
        /// <param name="EndDate">The end date of the hire date range (optional).</param>
        /// <returns>
        /// A list of teachers, each containing their details such as ID, first name, last name, hire date, salary, employee number, and associated course names.
        /// </returns>
        /// <remarks>
        /// This method connects to the database, retrieves teacher and course information, and optionally filters the teachers by hire date range.
        /// If no date range is provided, all teachers will be returned along with their courses.
        /// </remarks>
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"teacherId": 1,"teacherFName": "Alexander","teacherLName": "Bennett","teacherHireDate": "2016-08-05T00:00:00","teacherSalary": "55.30","teacherEmpNu": "T378","courseNames": ["Web Application Development"]},....]
        /// GET api/Teacher/ListTeachers?StartDate=2016-01-01&EndDate=2018-01-01 -> [{"teacherId":1,"teacherFName":"Alexander","teacherLName":"Bennett","teacherHireDate":"2016-08-05T00:00:00","teacherSalary":"55.30","teacherEmpNu":"T378","courseNames":["Web Application Development"]},{"teacherId":6,"teacherFName":"Thomas","teacherLName":"Hawkins","teacherHireDate":"2016-08-10T00:00:00","teacherSalary":"54.45","teacherEmpNu":"T393","courseNames":["Career Connections"]}]
        /// </example>


        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers(DateTime? StartDate = null, DateTime? EndDate = null)
        {
            List<Teacher> Teachers = new List<Teacher>();

            using (MySqlConnection Connect = _context.AccessDatabase())
            {
                Connect.Open();
                MySqlCommand Command = Connect.CreateCommand();


                string query = "SELECT * FROM teachers LEFT JOIN courses ON teachers.teacherid = courses.teacherid";


                bool hasConditions = false;
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    query += " WHERE hiredate BETWEEN @startDate AND @endDate";
                    Command.Parameters.AddWithValue("@startDate", StartDate.Value);
                    Command.Parameters.AddWithValue("@endDate", EndDate.Value);
                    hasConditions = true;
                }

                Command.CommandText = query;
                Command.Prepare();

                using (MySqlDataReader Result = Command.ExecuteReader())
                {
                    Dictionary<int, Teacher> teacherDict = new Dictionary<int, Teacher>();

                    while (Result.Read())
                    {
                        int Id = Convert.ToInt32(Result["teacherid"]);
                        string FirstName = Result["teacherfname"].ToString();
                        string LastName = Result["teacherlname"].ToString();
                        string EmployeeNumber = Result["employeenumber"].ToString();
                        DateTime Hire = Convert.ToDateTime(Result["hiredate"]);
                        decimal Salary = Convert.ToDecimal(Result["salary"]);
                        string CourseName = Result["coursename"].ToString();

                        if (!teacherDict.ContainsKey(Id))
                        {
                            teacherDict[Id] = new Teacher()
                            {
                                TeacherId = Id,
                                TeacherFName = FirstName,
                                TeacherLName = LastName,
                                Hire = Hire,
                                Salary = Salary,
                                EmployeeNumber = EmployeeNumber,
                                CourseNames = new List<string>()
                            };
                        }
                        teacherDict[Id].CourseNames.Add(CourseName);
                    }

                    Teachers.AddRange(teacherDict.Values);
                }
            }

            return Teachers;
        }


        /// <summary>
        /// Retrieves a single teacher's details, including the courses they teach, by their teacher ID.
        /// </summary>
        /// <param name="id">The ID of the teacher to retrieve.</param>
        /// <returns>
        /// The details of the teacher, including their first name, last name, employee number, hire date, salary, and a list of the courses they teach.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves the teacher's information and the courses they teach.
        /// If the teacher exists, their details and course names will be returned.
        /// </remarks>
        /// <example>
        /// GET api/Teacher/FindTeacher/1 -> {"teacherId":1,"teacherFName":"Alexander","teacherLName":"Bennett","teacherHireDate":"2016-08-05T00:00:00","teacherSalary":"55.30","teacherEmpNu":"T378","courseNames":["Web Application Development"]}
        /// </example>

        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {


            Teacher SelectedTeacher = new Teacher();
            using (MySqlConnection Connect = _context.AccessDatabase())
            {
                Connect.Open();
                MySqlCommand Command = Connect.CreateCommand();
                Command.CommandText = "SELECT teachers.*, courses.courseName FROM courses INNER JOIN teachers ON teachers.teacherId = courses.teacherId WHERE teachers.teacherId = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader Result = Command.ExecuteReader())
                {
                    while (Result.Read())
                    {
                        int Id = Convert.ToInt32(Result["teacherid"]);
                        string FirstName = Result["teacherfname"].ToString();
                        string LastName = Result["teacherlname"].ToString();
                        string EmployeeNumber = Result["employeenumber"].ToString();

                        DateTime Hire = Convert.ToDateTime(Result["hiredate"]);
                        decimal Salary = Convert.ToDecimal(Result["salary"]);
                        string CourseName = Result["coursename"].ToString();
                        if (SelectedTeacher.TeacherId == 0)
                        {

                            SelectedTeacher.TeacherFName = FirstName;
                            SelectedTeacher.TeacherLName = LastName;
                            SelectedTeacher.Salary = Salary;
                            SelectedTeacher.Hire = Hire;
                            SelectedTeacher.EmployeeNumber = EmployeeNumber;
                            SelectedTeacher.CourseNames = new List<string>();
                        }
                        SelectedTeacher.CourseNames.Add(CourseName);
                    }
                }
            }
            return SelectedTeacher;
        }
        /// <summary>
        /// Retrieves a list of courses taught by a specific teacher, identified by their teacher ID.
        /// </summary>
        /// <param name="id">The ID of the teacher whose courses are to be retrieved.</param>
        /// <returns>
        /// A list of course names taught by the specified teacher.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves the list of courses assigned to the teacher with the given ID.
        /// If no courses are found, an empty list will be returned.
        /// </remarks>
        /// <example>
        /// GET api/Teacher/GetCoursesByTeacher/1 -> ["Web Application Development"]
        /// </example>

        [HttpGet]
        [Route("GetCoursesByTeacher/{id}")]
        public List<string> GetCoursesByTeacher(int id)
        {
            List<string> courses = new List<string>();

            using (MySqlConnection Connect = _context.AccessDatabase())
            {
                Connect.Open();
                MySqlCommand Command = Connect.CreateCommand();
                Command.CommandText = "SELECT CourseName FROM courses WHERE TeacherId = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        string courseName = ResultSet["CourseName"].ToString();
                        courses.Add(courseName);
                    }
                }
            }

            return courses;
        }

    }
}