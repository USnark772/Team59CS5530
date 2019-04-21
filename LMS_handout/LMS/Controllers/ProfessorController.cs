using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.Models.LMSModels;


namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            //select classId from classes
            //join with Enrolled and Students
            //select fname, lname, uid, dob, grade
            var classId_query =
                from c in db.Classes
                where c.Course.Abbr == subject && c.Course.Number == num && c.Semester == semester
                select c.ClassId;
            UInt32 classId = classId_query.SingleOrDefault();
            var query =
                from e in db.Enrolled
                where e.Class == classId
                select new
                {
                    fname = e.U.First,
                    lname = e.U.Last,
                    uid = e.U.UId,
                    dob = e.U.Dob,
                    grade = e.Grade
                };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            if (category == null)
            {
                var query =
                    from a in db.Assignments
                    where a.Cat.ClassNavigation.Semester == semester
                    && a.Cat.ClassNavigation.Course.Abbr == subject
                    && a.Cat.ClassNavigation.Course.Number == num
                    select new
                    {
                        aname = a.Name,
                        cname = a.Cat.Name,
                        due = a.DueDate,
                        submissions = a.Submissions.Count()
                    };
                return Json(query.ToArray());
            }
            else
            {
                var query =
                    from a in db.Assignments
                    where a.Cat.Name == category
                    && a.Cat.ClassNavigation.Semester == semester
                    && a.Cat.ClassNavigation.Course.Abbr == subject
                    && a.Cat.ClassNavigation.Course.Number == num
                    select new
                    {
                        aname = a.Name,
                        cname = a.Cat.Name,
                        due = a.DueDate,
                        submissions = a.Submissions.Count()
                    };
                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the following fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            var query =
                from a in db.AssignmentCategories
                where a.ClassNavigation.Semester == semester
                    && a.ClassNavigation.Course.Abbr == subject
                    && a.ClassNavigation.Course.Number == num
                select new
                {
                    name = a.Name,
                    weight = a.Weight
                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            //check if category already exists
            var check =
                from oc in db.AssignmentCategories
                where oc.Name == category
                    && oc.ClassNavigation.Semester == semester
                    && oc.ClassNavigation.Course.Number == num
                    && oc.ClassNavigation.Course.Abbr == subject
                select oc.CatId;
            if (check.ToArray().Count() > 0)
            {
                return Json(new { success = false });
            }

            var catID =
                from ac in db.AssignmentCategories
                orderby ac.CatId descending
                select ac.CatId;
            var classID =
                from c in db.Classes
                where c.Semester == semester
                    && c.Course.Number == num
                    && c.Course.Abbr == subject
                select c.ClassId;

            var catID_enum = catID.GetEnumerator();
            catID_enum.MoveNext();

            AssignmentCategories new_cat = new AssignmentCategories();
            new_cat.CatId = catID_enum.Current + 1;
            new_cat.Name = category;
            new_cat.Weight = (UInt32)catweight;
            new_cat.Class = classID.SingleOrDefault();
            db.AssignmentCategories.Add(new_cat);

            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });
            }

            var class_roster =
                (from e in db.Enrolled
                 where e.ClassNavigation.Course.Number == num
                 && e.ClassNavigation.Course.Abbr == subject
                 && e.ClassNavigation.Semester == semester
                 select e);

            foreach (var item in class_roster)
            {
                item.Grade = ReCalculateClassGradeForStudent(subject, num, season, year, category, item.UId);
            }

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
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            var catID =
                from ac in db.AssignmentCategories
                where ac.Name == category
                && ac.ClassNavigation.Semester == semester
                && ac.ClassNavigation.Course.Number == num
                && ac.ClassNavigation.Course.Abbr == subject
                select ac.CatId;
            var aID =
                from a in db.Assignments
                orderby a.AId descending
                select a.AId;
            var aID_enum = aID.GetEnumerator();
            aID_enum.MoveNext();
            Assignments new_assign = new Assignments();
            new_assign.AId = aID_enum.Current + 1;
            new_assign.Name = asgname;
            new_assign.Value = (UInt32)asgpoints;
            new_assign.Contents = asgcontents;
            new_assign.DueDate = asgdue;
            new_assign.CatId = catID.SingleOrDefault();
            db.Assignments.Add(new_assign);

            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });
            }
            var class_roster =
                (from e in db.Enrolled
                 where e.ClassNavigation.Course.Number == num
                 && e.ClassNavigation.Course.Abbr == subject
                 && e.ClassNavigation.Semester == semester
                 select e);

            foreach (var item in class_roster)
            {
                item.Grade = ReCalculateClassGradeForStudent(subject, num, season, year, category, item.UId);
            }
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
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            var query =
                from s in db.Submissions
                where s.A.Name == asgname
                    && s.A.Cat.Name == category
                    && s.A.Cat.ClassNavigation.Semester == semester
                    && s.A.Cat.ClassNavigation.Course.Number == num
                    && s.A.Cat.ClassNavigation.Course.Abbr == subject
                select new
                {
                    fname = s.U.First,
                    lname = s.U.Last,
                    uid = s.U.UId,
                    time = s.SubDate,
                    score = s.Score
                };
            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            // TODO: Implement
            string semester = season + year.ToString();
            var query =
                from s in db.Submissions
                where s.UId == uid
                    && s.A.Name == asgname
                    && s.A.Cat.Name == category
                    && s.A.Cat.ClassNavigation.Semester == semester
                    && s.A.Cat.ClassNavigation.Course.Number == num
                    && s.A.Cat.ClassNavigation.Course.Abbr == subject
                select s;
            Submissions sub = query.SingleOrDefault();
            sub.Score = (UInt32)score;

            try
            {
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success = false });
            }
            var enrollment_record =
                (from e in db.Enrolled
                 where e.UId == uid
                 && e.ClassNavigation.Course.Number == num
                 && e.ClassNavigation.Course.Abbr == subject
                 && e.ClassNavigation.Semester == semester
                 select e).SingleOrDefault();
            enrollment_record.Grade = ReCalculateClassGradeForStudent(subject, num, season, year, category, enrollment_record.UId);
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
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            // TODO: Implement
            var query =
                from c in db.Classes
                where c.UId == uid
                select new
                {
                    subject = c.Course.Abbr,
                    number = c.Course.Number,
                    name = c.Course.Name,
                    season = c.Semester.Substring(0, c.Semester.Length - 4),
                    year = c.Semester.Substring(c.Semester.Length - 4)
                };
            return Json(query.ToArray());
        }


        private string ReCalculateClassGradeForStudent(string subject, int num, string season, int year, string category, string uid)
        {
            string semester = season + year.ToString();
            string class_grade;
            float? grade = 0;
            UInt32? weight_sum = 0;
            var all_categories =
                from c in db.AssignmentCategories
                where c.ClassNavigation.Course.Number == num
                && c.ClassNavigation.Course.Abbr == subject
                && c.ClassNavigation.Semester == semester
                select c;
            foreach (var current_category in all_categories)
            {
                var all_scores =
                    from s in db.Submissions
                    where s.A.Cat.ClassNavigation.Semester == semester
                    && s.A.Cat.ClassNavigation.Course.Number == num
                    && s.A.Cat.ClassNavigation.Course.Abbr == subject
                    && s.A.Cat.Name == current_category.Name
                    && s.UId == uid
                    select s.Score;
                UInt32 score_sum = 0;
                foreach (var sub_score in all_scores)
                {
                    score_sum += (UInt32)sub_score;
                }
                var all_values =
                    from a in db.Assignments
                    where a.Cat.ClassNavigation.Semester == semester
                    && a.Cat.ClassNavigation.Course.Number == num
                    && a.Cat.ClassNavigation.Course.Abbr == subject
                    && a.Cat.Name == current_category.Name
                    select a.Value;
                UInt32 values_sum = 0;
                foreach (var assign_val in all_values)
                {
                    values_sum += (UInt32)assign_val;
                }
                weight_sum += current_category.Weight;
                if (values_sum > 0)
                    grade += (((float)score_sum / (float)values_sum) * current_category.Weight);
            }
            grade = grade * (100 / weight_sum);

            if (grade >= 93)
                class_grade = "A";
            else if (grade >= 90 && grade < 93)
                class_grade = "A-";
            else if (grade >= 87 && grade < 90)
                class_grade = "B+";
            else if (grade >= 83 && grade < 87)
                class_grade = "B";
            else if (grade >= 80 && grade < 83)
                class_grade = "B-";
            else if (grade >= 77 && grade < 80)
                class_grade = "C+";
            else if (grade >= 73 && grade < 77)
                class_grade = "C";
            else if (grade >= 70 && grade < 73)
                class_grade = "C-";
            else if (grade >= 67 && grade < 70)
                class_grade = "D+";
            else if (grade >= 63 && grade < 67)
                class_grade = "D";
            else if (grade >= 60 && grade < 63)
                class_grade = "D-";
            else
                class_grade = "E";
            return class_grade;
        }


        /*******End code to modify********/

    }
}