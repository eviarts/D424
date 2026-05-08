using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using D424.Classes;

namespace D424.Tests
{
    public class DbUnitTest2
    {
        private Database DB()
        {
            var path = Path.GetTempFileName();
            return new Database(path);
        }
        
        [Fact]
        public async Task InsertDeleteCourse()
        {
            var db = DB();
            await db.TestInitAsync();

            //Insert term for course
            var term = new Terms
            {
                TermTitle = "Term 1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            };
            await db.SaveTermAsync(term);

            var testTerm = await db.GetTermsAsync();
            Assert.Single(testTerm);

            //Insert course to delete
            var course = new Courses
            {
                CourseName = "Test Course",
                CourseStatus = "Active",
                InstructorName = "Jane Doe",
                InstructorEmail = "test@testmail.com",
                InstructorPhone = "888-888-8888",
                StartDate = DateTime.Today,
                EndDate= DateTime.Today.AddMonths(1),
                TermId = term.TermId
            };
            await db.SaveCourseAsync(course);

            var testCourse = await db.GetAllCourses();
            Assert.Single(testCourse);

            //Delete inserted course
            await db.DeleteCourseAsync(course);

            var deletedCourse = await db.GetAllCourses();
            Assert.Empty(deletedCourse);
        }
    }
}
