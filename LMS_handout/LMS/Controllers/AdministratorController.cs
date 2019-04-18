using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            // TODO: Implement
            // Get the department and then use the scaffolded link to get all courses?
            var query =
                from c in db.Courses
                where c.AbbrNavigation.Abbr == subject
                select new
                {
                    name = c.Name,
                    number = c.Number
                };
            return Json(query.ToArray());
        }





        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            // TODO: Implement
            var query =
                from p in db.Professors
                where p.MajorNavigation.Abbr == subject
                select new
                {
                    lname = p.First,
                    fname = p.Last,
                    uid = p.UId
                };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            // TODO: Implement
            var cid =
                from c in db.Courses orderby c.CId descending select c.CId;
            Courses new_course = new Courses();
            new_course.Abbr = subject;
            new_course.Number = (UInt32)number;
            new_course.Name = name;
            new_course.CId = cid.SingleOrDefault() + 1;
            db.Courses.Add(new_course);
            try
            {
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            // TODO: Implement
            TimeSpan start_time = start.TimeOfDay;
            TimeSpan end_time = end.TimeOfDay;
            var other_classes =
                from c in db.Classes
                where c.Location == location
                select new { c.Start, c.End };
            foreach (var item in other_classes)
            {
                if ((start_time <= item.End && start_time >= item.Start)
                    || (end_time <= item.End && end_time >= item.Start))
                {
                    return Json(new { success = false });
                }
            }
            var same_course =
                from c in db.Classes
                where c.CourseId == (UInt32)number
                select c;
            if(same_course.Count() > 0)
            {
                return Json(new { success = false });
            }
            var cid =
                from c in db.Classes orderby c.ClassId descending select c.ClassId;
            Classes new_class = new Classes();
            new_class.ClassId = cid.SingleOrDefault() + 1;
            new_class.CourseId = (UInt32)number;
            new_class.Start = start.TimeOfDay;
            new_class.End = end.TimeOfDay;
            new_class.Location = location;
            new_class.Semester = season + year.ToString();
            new_class.UId = instructor;
            try
            {
                db.Classes.Add(new_class);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        /*******End code to modify********/

    }
}