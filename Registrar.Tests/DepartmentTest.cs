using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Registrar.Models;

namespace Registrar.Tests
{
  [TestClass]
  public class DepartmentTest : IDisposable
  {
    public DepartmentTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=registrar_test;";
    }

    public void Dispose()
    {
      Student.DeleteAll();
      Course.DeleteAll();
      Department.DeleteAll();
    }

    [TestMethod]
    public void Save_SavesDepartmentToDatabase_DepartmentList()
    {
      Department newDepartment = new Department("Math");

      newDepartment.Save();

      List<Department> expected = new List<Department> {newDepartment};
      List<Department> actual = Department.GetAll();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AddStudent_AddStudentToDepartmentInDatabase_StudentList()
    {
      Department compSci = new Department("Computer Science");
      Student studentOne = new Student("Johnny", "Fall 2017");
      Student studentTwo = new Student("Jill", "Fall 2017");

      compSci.Save();
      studentOne.Save();
      studentTwo.Save();

      compSci.AddStudent(studentOne);
      compSci.AddStudent(studentTwo);

      List<Student> expected = new List<Student> {studentOne, studentTwo};
      List<Student> actual = compSci.GetStudents();

      CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetCourses_GetCoursesWithDepartmentId_CourseList()
    {
      Department history = new Department("History");
      history.Save();
      Course courseOne = new Course("American History", "AH100", history.GetId());
      Course courseTwo = new Course("Relevant History", "RH200", history.GetId());


      courseOne.Save();
      courseTwo.Save();

      List<Course> expected = new List<Course> {courseOne, courseTwo};
      List<Course> actual = history.GetCourses();


      CollectionAssert.AreEqual(expected, actual);
    }
  }
}
