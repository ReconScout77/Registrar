using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Registrar.Models;

namespace Registrar.Tests
{
  [TestClass]
  public class StudentTest : IDisposable
  {
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=registrar_test;";
    }

    [TestMethod]
    public void Save_SavesStudentToDatabase_StudentList()
    {
      Student testStudent = new Student("Billy Bob", "January 1, 2003");
      testStudent.Save();

      List<Student> expected = new List<Student> {testStudent};
      List<Student> result = Student.GetAll();

      CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void AddCourse_AddStudentToCourse_CourseList()
    {
      Student newStudent = new Student("Joe", "Quarter 1");
      Course testCourse = new Course("Intro to Math", "Math101", 0);
      Course testCourse2 = new Course("Ancient History", "Hist101", 0);

      newStudent.Save();
      testCourse.Save();
      testCourse2.Save();

      newStudent.AddCourse(testCourse);
      newStudent.AddCourse(testCourse2);

      List<Course> expected = new List<Course> {testCourse, testCourse2};
      List<Course> result = newStudent.GetCourses();

      CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetCompletedCourses_SetsCourseStatusToCompleteInSchedulesTable_CourseList()
    {
      Student newStudent = new Student("John", "Q4");
      newStudent.Save();
      Course history = new Course("History", "101", 0);
      history.Save();
      Course math = new Course("Math", "101", 1);
      math.Save();

      newStudent.AddCourse(history);
      newStudent.AddCourse(math);
      newStudent.CourseCompleted(history);

      List<Course> expected = new List<Course> {history};
      List<Course> actual = newStudent.GetCompletedCourses();

      foreach(var course in actual)
      {
        Console.WriteLine(course.GetDetails());
      }

      CollectionAssert.AreEqual(expected, actual);
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Course.DeleteAll();
      Department.DeleteAll();
    }
  }
}
