using Cumulative1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Notice;
using MySqlX.XDevAPI.Common;
using System.Xml.Linq;

namespace Cumulative1.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchooldbContext _context;
        public StudentAPIController(SchooldbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(template: "listStudents")]
        public List<Student> ListStudent()
        {
            List<Student> Students = new List<Student>();
            using (MySqlConnection Connect = _context.AccessDatabase())
            {
                Connect.Open();
                MySqlCommand Command = Connect.CreateCommand();
                Command.CommandText = "Select * from students";
                Command.Prepare();
                using (MySqlDataReader Result = Command.ExecuteReader())
                {
                    while (Result.Read())
                    {
                        int Id = Convert.ToInt32(Result["studentid"]);
                        string SFName = Result["studentfname"].ToString();
                        string SLName = Result["studentlname"].ToString();
                        string SNumber = Result["studentnumber"].ToString();
                        DateTime EnrolDate = Convert.ToDateTime(Result["enroldate"]);
                        Student CurrentStudent = new Student()
                        {
                            Id = Id,
                            SFName = SFName,
                            SLName = SLName,
                            EnrollDate = EnrolDate,
                            SNumber = SNumber

                        };

                        Students.Add(CurrentStudent);

                    }
                }

            }
            return Students;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(template: "FindStudent/{id}")]
        public Student FindStudent(int id)
        {

            Student SelectedStudents = new Student();

            using (MySqlConnection Connect = _context.AccessDatabase())
            {
                Connect.Open();
                MySqlCommand Command = Connect.CreateCommand();
                Command.CommandText = "Select * from students WHERE studentid = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader Result = Command.ExecuteReader())
                {
                    while (Result.Read())
                    {
                        int Id = Convert.ToInt32(Result["studentid"]);
                        string SFName = Result["studentfname"].ToString();
                        string SLName = Result["studentlname"].ToString();
                        string SNumber = Result["studentnumber"].ToString();
                        DateTime EnrolDate = Convert.ToDateTime(Result["enroldate"]);


                        SelectedStudents.Id = Id;
                        SelectedStudents.SFName = SFName;
                        SelectedStudents.SLName = SLName;
                        SelectedStudents.EnrollDate = EnrolDate;
                        SelectedStudents.SNumber = SNumber;

                    }
                }
            }


            return SelectedStudents;
        }


    }
}
