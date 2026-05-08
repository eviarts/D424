using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SQLite;


namespace D424.Classes
{
    public class Database
    {
        private SQLiteAsyncConnection _database;

        public Database()
        {
            var dbPath = Path.GetTempFileName();

            _database = new SQLiteAsyncConnection(dbPath);
            _ = DbInitAsync();
        }

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        public Task TestInitAsync()
        {
            return DbInitAsync();
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

        public Task<List<Courses>> CoursesByInstructor(string name)
        {
            return _database.Table<Courses>()
                .Where(c => c.InstructorName == name)
                .ToListAsync();
        }      

        

    }
}
