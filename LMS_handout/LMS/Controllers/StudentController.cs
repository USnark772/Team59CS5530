using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.LMSModels;


namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            // TODO: Implement
            var query =
                from c in db.Enrolled
                where c.UId == uid
                select new
                {
                    subject = c.ClassNavigation.Course.Abbr,
                    number = c.ClassNavigation.Course.Number,
                    name = c.ClassNavigation.Course.Name,
                    season = c.ClassNavigation.Semester.Substring(0, c.ClassNavigation.Semester.Length - 4),
                    year = c.ClassNavigation.Semester.Substring(c.ClassNavigation.Semester.Length - 4),
                    grade = c.Grade
                };
            return Json(query.ToString());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            var query =
                from a in db.Assignments
                join e in db.Enrolled
                on a.Cat.ClassNavigation.ClassId equals e.Class
                where a.Cat.ClassNavigation.Course.Abbr == subject
                && a.Cat.ClassNavigation.Course.Number == num
                && a.Cat.ClassNavigation.Semester == semester
                select new
                {
                    aname = a.Name,
                    cname = a.Cat.Name,
                    due = a.DueDate,
                    score = a.Value
                };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            // TODO: Implement
            string semester = season + year.ToString();

            var class_subs =
                from s in db.Submissions
                where s.UId == uid
                && s.A.Cat.ClassNavigation.Course.Abbr == subject
                && s.A.Cat.ClassNavigation.Course.Number == num
                && s.A.Cat.ClassNavigation.Semester == semester
                && s.A.Cat.Name == category
                && s.A.Name == asgname
                select s;
            if(class_subs.Count() > 0)
            {
                // TODO: Finish this
            }

            return Json(new { success = false });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            // TODO: Implement
            var check =
                from e in db.Enrolled
                where e.UId == uid
                && e.ClassNavigation.Semester == season + year.ToString()
                && e.ClassNavigation.Course.Number == num
                && e.ClassNavigation.Course.Abbr == subject
                select e;
            if (check.Count() > 0)
            {
                return Json(new { success = false });
            }
            var oldID =
                (from e in db.Enrolled
                 orderby e.EId
                 select e.EId).SingleOrDefault();
            Enrolled new_enroll = new Enrolled();
            new_enroll.Class = (UInt32)num;
            new_enroll.UId = uid;
            new_enroll.Grade = "0";
            new_enroll.EId = oldID + 1;
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
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            // TODO: Implement
            return Json(null);
        }

        /*******End code to modify********/

    }
}