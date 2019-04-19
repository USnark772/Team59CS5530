using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        /*******Begin code to modify********/

        // TODO: Uncomment and change 'X' after you have scaffoled
        // Done
        protected Team59LMSContext db;

        public CommonController()
        {
            db = new Team59LMSContext();
        }

        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different LibraryContext - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor 
         *          (look this up if interested).
        */

        // TODO: Uncomment and change 'X' after you have scaffoled
        // Done
        public void UseLMSContext(Team59LMSContext ctx)
        {
            db = ctx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            // TODO: Do not return this hard-coded array.
            // TODO: Fill this out
            var query =
                from d in db.Departments
                select new { subject = d.Abbr, name = d.Name };
            return Json(query);
            //return Json(new[] { new { name = "None", subject = "NONE" } });
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            // TODO: Implement
            // Done but untested.
            var query =
                from d in db.Departments
                select new
                {
                    subject = d.Abbr,
                    dname = d.Name,
                    courses =
                    from c in db.Courses
                    where d.Abbr == c.Abbr
                    select new { number = c.Number, cname = c.Name }
                };
            return Json(query);
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            // TODO: Implement
            var query =
                from c in db.Classes
                where c.Course.Abbr == subject
                && c.Course.Number == number
                join p in db.Professors
                on c.UId equals p.UId
                select new
                {
                    season = c.Semester.Substring(0, c.Semester.Length - 4),
                    year = c.Semester.Substring(c.Semester.Length - 4),
                    location = c.Location,
                    start = c.Start,
                    end = c.End,
                    fname = p.First,
                    lname = p.Last,
                };
            return Json(query);
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            // TODO: Implement
            var query =
                from a in db.Assignments
                where a.Cat.ClassNavigation.Course.AbbrNavigation.Abbr == subject
                && a.Cat.ClassNavigation.Semester == season + year.ToString()
                && a.Cat.ClassNavigation.Course.Number == num
                && a.Cat.Name == category
                && a.Name == asgname
                select a.Contents;
            return Content(query.SingleOrDefault());
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            string text = "";
            var query =
                from s in db.Submissions
                where s.UId == uid
                && s.A.Name == asgname
                && s.A.Cat.Name == category
                && s.A.Cat.ClassNavigation.Semester == semester
                && s.A.Cat.ClassNavigation.Course.Number == num
                && s.A.Cat.ClassNavigation.Course.Abbr == subject
                select s;
            if (query.Count() > 0)
                text = query.SingleOrDefault().Contents;
            return Content(text);
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            // TODO: Implement
            var record =
                from a in db.Administrators
                where a.UId == uid
                select new
                {
                    fname = a.First,
                    lname = a.Last,
                    uid = a.UId
                };
            if(record.Count() > 0)
                return Json(record);

            var record2 =
                from p in db.Professors
                where p.UId == uid
                select new
                {
                    fname = p.First,
                    lname = p.Last,
                    uid = p.UId,
                    department = p.MajorNavigation.Name
                };
            if (record2.Count() > 0)
                return Json(record2);

            var record3 =
                from s in db.Students
                where s.UId == uid
                select new
                {
                    fname = s.First,
                    lname = s.Last,
                    uid = s.UId,
                    department = s.MajorNavigation.Name
                };
            if (record3.Count() > 0)
                return Json(record3);

            return Json(new { success = false });
        }


        /*******End code to modify********/

    }
}