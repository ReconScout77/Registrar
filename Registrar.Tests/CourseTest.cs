using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Registrar.Models;

namespace Registrar.Tests
{
  [TestClass]
  public class CourseTest : IDisposable
  {
    public CourseTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=registrar_test;";
    }

    [TestMethod]
    public void Save_SavesCourseToDatabase_CourseList()
    {
      Course testCourse = new Course("Intro to C#", "CSC 101", 0);
      testCourse.Save();

      List<Course> expected = new List<Course> {testCourse};
      List<Course> result = Course.GetAll();

      CollectionAssert.AreEqual(expected, result);
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Course.DeleteAll();
      Department.DeleteAll();
    }
  }
}
