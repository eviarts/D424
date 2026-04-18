using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SQLite;


namespace C971App.Classes
{
    public class Database
    {
        private SQLiteAsyncConnection _database;

        public Database()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "database.db");

            //To reset for testing
            /*if (File.Exists(dbPath))
            {
                File.Delete(dbPath); 
            }*/

            _database = new SQLiteAsyncConnection(dbPath);
            _ = DbInitAsync();
        }

        public Task<List<SearchResults>> Search(string query)
        {
            query = $"{query}%";

            string sqlQuery = @"
                    SELECT c.CourseName AS CourseName,
                           c.InstructorName,
                           t.TermTitle AS TermTitle
                    FROM Courses c
                    JOIN Terms t ON c.TermId = t.TermId
                    WHERE (c.CourseName IS NOT NULL AND LOWER(c.CourseName) LIKE (?))
                    OR (c.InstructorName IS NOT NULL AND LOWER(c.InstructorName) LIKE (?))";

            return _database.QueryAsync<SearchResults>(sqlQuery, query, query);
        }

        public async Task DbInitAsync()
        {
            await _database.CreateTableAsync<Terms>();
            await _database.CreateTableAsync<Courses>();
            await _database.CreateTableAsync<Assessments>();
        }       

        public Task<int> SaveTermAsync(Terms term)
        {
            if (term.TermId != 0)
                return _database.UpdateAsync(term);
            else
                return _database.InsertAsync(term);
        }

        public Task<int> SaveCourseAsync(Courses course)
        {
            if (course.CourseId != 0)
                return _database.UpdateAsync(course);
            else
                return _database.InsertAsync(course);
        }

        public Task<int> SaveAssessmentAsync(Assessments assessment)
        {
            if (assessment.AsmntId != 0)
                return _database.UpdateAsync(assessment);
            else
                return _database.InsertAsync(assessment);
        }

        public Task<int> DeleteTermAsync(Terms term)
        {
            return _database.DeleteAsync(term);
        }

        public Task<int> DeleteCourseAsync(Courses course)
        {
            return _database.DeleteAsync(course);
        }

        public Task<int> DeleteAssessment(Assessments assessment)
        {
            return _database.DeleteAsync(assessment);
        }

        public Task<List<Terms>> GetTermsAsync()
        {
            return _database.Table<Terms>().ToListAsync();
        }

        public Task<List<Courses>> GetAllCourses()
        {
            return _database.Table<Courses>().ToListAsync();
        }

        public Task<List<Courses>> GetCoursesPerTerm(int termId)
        {
            return _database.Table<Courses>()
                .Where(c => c.TermId == termId)
                .ToListAsync();
        } 

        public Task<List<Assessments>> GetAssessemtnsPerCourse(int courseId)
        {
            return _database.Table<Assessments>()
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }

        public Task<List<Courses>> CoursesByDate(DateTime start, DateTime end)
        {
            return _database.Table<Courses>()
                .Where(c => c.StartDate >= start && c.EndDate <= end)
                .ToListAsync();
        }

        

        //Test data
        public static async Task InsertTestData()
        {
            //Terms...................................................................................
            var term1 = new Terms
            {
                TermTitle = "Term 1",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 6, 30)
            };
            await App.db.SaveTermAsync(term1);

            var term2 = new Terms
            {
                TermTitle = "Term 2",
                StartDate = new DateTime(2026, 7, 1),
                EndDate = new DateTime(2026, 12, 31)
            };
            await App.db.SaveTermAsync(term2);

            var term3 = new Terms
            {
                TermTitle = "Term 3",
                StartDate = new DateTime(2027, 1, 1),
                EndDate = new DateTime(2027, 6, 30)
            };
            await App.db.SaveTermAsync(term3);

            var term4 = new Terms
            {
                TermTitle = "Term 4",
                StartDate = new DateTime(2027, 7, 1),
                EndDate = new DateTime(2027, 12, 31)
            };
            await App.db.SaveTermAsync(term4);

            //Courses...................................................................................
            var course1 = new Courses
            {
                CourseName = "English 101",
                CourseStatus = "Active",
                InstructorName = "Anika Patel",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                InstructorPhone = "555 - 123 - 4567",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 2, 1),
                TermId = term1.TermId
            };
            await App.db.SaveCourseAsync(course1);

            var course2 = new Courses
            {
                CourseName = "Math 101",
                CourseStatus = "Active",
                InstructorName = "Anika Patel",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                InstructorPhone = "555 - 123 - 4567",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 3, 1),
                TermId = term1.TermId
            };
            await App.db.SaveCourseAsync(course2);

            var course3 = new Courses
            {
                CourseName = "Art 101",
                CourseStatus = "Active",
                InstructorName = "Anika Patel",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                InstructorPhone = "555 - 123 - 4567",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 4, 1),
                TermId = term1.TermId
            };
            await App.db.SaveCourseAsync(course3);

            var course4 = new Courses
            {
                CourseName = "History 101",
                CourseStatus = "Inactive",
                InstructorName = "Anika Patel",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                InstructorPhone = "555 - 123 - 4567",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 5, 1),
                TermId = term1.TermId
            };
            await App.db.SaveCourseAsync(course4);

            var course5 = new Courses
            {
                CourseName = "Science 101",
                CourseStatus = "Inactive",
                InstructorName = "Anika Patel",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                InstructorPhone = "555 - 123 - 4567",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 5, 1),
                TermId = term1.TermId
            };
            await App.db.SaveCourseAsync(course5);

            var course6 = new Courses
            {
                CourseName = "Astronomy 101",
                CourseStatus = "Inactive",
                InstructorName = "Anika Patel",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                InstructorPhone = "555 - 123 - 4567",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 6, 30),
                TermId = term1.TermId
            };
            await App.db.SaveCourseAsync(course6);

            //Assessments...................................................................................
            var oa1 = new Assessments
            {
                AsmntName = "Test 1",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 2, 1),
                AsmntType = "Objective",
                CourseId = course1.CourseId
            };
            await App.db.SaveAssessmentAsync(oa1);

            var pa1 = new Assessments
            {
                AsmntName = "Project 1",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 2, 1),
                AsmntType = "Performance",
                CourseId = course1.CourseId
            };
            await App.db.SaveAssessmentAsync(pa1);

            var oa2 = new Assessments
            {
                AsmntName = "Test 2",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 3, 1),
                AsmntType = "Objective",
                CourseId = course2.CourseId
            };
            await App.db.SaveAssessmentAsync(oa2);

            var pa2 = new Assessments
            {
                AsmntName = "Project 2",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 3, 1),
                AsmntType = "Performance",
                CourseId = course2.CourseId
            };
            await App.db.SaveAssessmentAsync(pa2);

            var oa3 = new Assessments
            {
                AsmntName = "Test 3",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 4, 1),
                AsmntType = "Objective",
                CourseId = course3.CourseId
            };
            await App.db.SaveAssessmentAsync(oa3);

            var pa3 = new Assessments
            {
                AsmntName = "Project 3",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 4, 1),
                AsmntType = "Performance",
                CourseId = course3.CourseId
            };
            await App.db.SaveAssessmentAsync(pa3);

            var oa4 = new Assessments
            {
                AsmntName = "Test 4",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 5, 1),
                AsmntType = "Objective",
                CourseId = course4.CourseId
            };
            await App.db.SaveAssessmentAsync(oa4);

            var pa4 = new Assessments
            {
                AsmntName = "Project 4",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 5, 1),
                AsmntType = "Performance",
                CourseId = course4.CourseId
            };
            await App.db.SaveAssessmentAsync(pa4);

            var oa5 = new Assessments
            {
                AsmntName = "Test 5",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 5, 1),
                AsmntType = "Objective",
                CourseId = course5.CourseId
            };
            await App.db.SaveAssessmentAsync(oa5);

            var pa5 = new Assessments
            {
                AsmntName = "Project 5",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 5, 1),
                AsmntType = "Performance",
                CourseId = course5.CourseId
            };
            await App.db.SaveAssessmentAsync(pa5);

            var oa6 = new Assessments
            {
                AsmntName = "Test 6",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 6, 30),
                AsmntType = "Objective",
                CourseId = course6.CourseId
            };
            await App.db.SaveAssessmentAsync(oa6);

            var pa6 = new Assessments
            {
                AsmntName = "Project 6",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 6, 30),
                AsmntType = "Performance",
                CourseId = course6.CourseId
            };
            await App.db.SaveAssessmentAsync(pa6);
        }

    }
}
