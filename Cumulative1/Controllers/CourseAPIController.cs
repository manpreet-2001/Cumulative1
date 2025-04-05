using Cumulative1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Security.Cryptography;

namespace Cumulative1.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly SchooldbContext _context;
        public CourseAPIController(SchooldbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves a list of all courses from the database.
        /// </summary>
        /// <returns>
        /// A list of all courses, including their course ID, teacher ID, course code, course name, start date, and finish date.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves all courses from the `courses` table.
        /// It returns a collection of course objects that include the course details such as ID, teacher, and course dates.
        /// </remarks>
        /// <example>
        /// GET api/Course/listCourse ->[{"courseId":1,"coursecode":"http5101","teacherid":1,"startdate":"2018-09-04T00:00:00","finishdate":"2018-12-14T00:00:00","coursename":"Web Application Development"},...]
        /// </example>
        [HttpGet]
        [Route(template: "listCourse")]
        public List<Course> ListCourse()
        {
            List<Course> Courses = new List<Course>();
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "Select * from courses";
                Command.Prepare();
                using (MySqlDataReader Result = Command.ExecuteReader())
                {
                    while (Result.Read())
                    {
                        int CId = Convert.ToInt32(Result["courseid"]);
                        int Tid = Convert.ToInt32(Result["teacherid"]);
                        string Ccode = Result["coursecode"].ToString();
                        string Cname = Result["coursename"].ToString();
                        DateTime StartDate = Convert.ToDateTime(Result["startdate"]);
                        DateTime FinishDate = Convert.ToDateTime(Result["finishdate"]);

                        Course CurrentCourse = new Course()
                        {
                            CId = CId,
                            Tid = Tid,
                            Ccode = Ccode,
                            Cname = Cname,
                            startdate = StartDate,
                            finishdate = FinishDate

                        };

                        Courses.Add(CurrentCourse);

                    }
                }

            }
            return Courses;
        }
        /// <summary>
        /// Retrieves details of a specific course by its course ID.
        /// </summary>
        /// <param name="id">The ID of the course to retrieve.</param>
        /// <returns>
        /// The details of the specified course, including its course ID, teacher ID, course code, course name, start date, and finish date.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves the details of a specific course from the `courses` table
        /// based on the provided course ID. If the course is found, the details will be returned.
        /// </remarks>
        /// <example>
        /// GET api/Course/FindCourse/1 -> {"courseId":1,"coursecode":"http5101","teacherid":1,"startdate":"2018-09-04T00:00:00","finishdate":"2018-12-14T00:00:00","coursename":"Web Application Development"}
        /// </example>
        [HttpGet]
        [Route(template: "FindCourse/{id}")]
        public Course FindCourse(int id)
        {

            Course SelectedCourse = new Course();

            using (MySqlConnection Connect = _context.AccessDatabase())
            {
                Connect.Open();
                MySqlCommand Command = Connect.CreateCommand();

                Command.CommandText = "Select * from courses WHERE courseid = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader Result = Command.ExecuteReader())
                {
                    while (Result.Read())
                    {
                        int CId = Convert.ToInt32(Result["courseid"]);
                        int Tid = Convert.ToInt32(Result["teacherid"]);
                        string Ccode = Result["coursecode"].ToString();
                        string Cname = Result["coursename"].ToString();
                        DateTime StartDate = Convert.ToDateTime(Result["startdate"]);
                        DateTime FinishDate = Convert.ToDateTime(Result["finishdate"]);

                        SelectedCourse.CId = CId;
                        SelectedCourse.Tid = Tid;
                        SelectedCourse.Ccode = Ccode;
                        SelectedCourse.Cname = Cname;
                        SelectedCourse.startdate = StartDate;
                        SelectedCourse.finishdate = FinishDate;
                    }
                }
            }
            return SelectedCourse;
        }

    }
}
