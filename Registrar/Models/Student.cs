using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Registrar.Models
{
  public class Student
  {
    private int _id;
    private string _name;
    private string _enrollmentDate;
    private int _departmentId;

    public Student(string name, string enrollment, int departmentId = 0, int id=0)
    {
      _id = id;
      _name = name;
      _enrollmentDate = enrollment;
      _departmentId = departmentId;
    }

    public int GetId()
    {
      return _id;
    }

    public string GetName()
    {
      return _name;
    }

    public string GetEnrollmentDate()
    {
      return _enrollmentDate;
    }

    public int GetDepartmentId()
    {
      return _departmentId;
    }

    public void SetDepartmentId(int departmentId)
    {
      _departmentId = departmentId;
    }

    public string GetDetails()
    {
      return "ID: " +_id + ", Name: " + _name + ", Enrollment Date: " + _enrollmentDate + ", Department ID" + _departmentId;
    }

    public override bool Equals(System.Object otherStudent)
    {
      if(!(otherStudent is Student))
      {
        return false;
      }
      else
      {
        Student newStudent = (Student) otherStudent;
        bool idEquality = this._id == newStudent.GetId();
        bool nameEquality = this._name == newStudent.GetName();
        bool enrollmentEquality = this._enrollmentDate == newStudent.GetEnrollmentDate();
        bool departmentEquality = this._departmentId == newStudent.GetDepartmentId();

        return (idEquality && nameEquality && enrollmentEquality && departmentEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Student> GetAll()
    {
      List<Student> allStudents = new List<Student> {};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM students;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string enrollmentDate = rdr.GetString(2);
        int department = rdr.GetInt32(3);

        Student newStudent = new Student(name, enrollmentDate, department, id);
        allStudents.Add(newStudent);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allStudents;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students (name, enrollment_date, department_id) VALUES (@name, @enrollment, @departmentId);";

      MySqlParameter studentName = new MySqlParameter();
      studentName.ParameterName = "@name";
      studentName.Value = _name;
      cmd.Parameters.Add(studentName);

      MySqlParameter enrollmentDate = new MySqlParameter();
      enrollmentDate.ParameterName = "@enrollment";
      enrollmentDate.Value = _enrollmentDate;
      cmd.Parameters.Add(enrollmentDate);

      MySqlParameter department = new MySqlParameter();
      department.ParameterName = "@departmentId";
      department.Value = _departmentId;
      cmd.Parameters.Add(department);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public void AddCourse(Course myCourse)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO schedules (course_id, student_id, course_completion) VALUES (@courseId, @studentId, @courseCompletion);";

      MySqlParameter courseId = new MySqlParameter();
      courseId.ParameterName = "@courseId";
      courseId.Value = myCourse.GetId();
      cmd.Parameters.Add(courseId);

      MySqlParameter studentId = new MySqlParameter();
      studentId.ParameterName = "@studentId";
      studentId.Value = this._id;
      cmd.Parameters.Add(studentId);

      MySqlParameter courseCompletion = new MySqlParameter();
      courseCompletion.ParameterName = "@courseCompletion";
      courseCompletion.Value = false;
      cmd.Parameters.Add(courseCompletion);

      cmd.ExecuteNonQuery();

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Course> GetCourses()
    {
      List<Course> studentCourses = new List<Course> {};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT courses.* FROM students JOIN schedules ON (students.id = schedules.student_id) JOIN courses ON (schedules.course_id = courses.id) WHERE students.id = @studentId;";

      MySqlParameter studentId = new MySqlParameter();
      studentId.ParameterName = "@studentId";
      studentId.Value = _id;
      cmd.Parameters.Add(studentId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);
        int department = rdr.GetInt32(3);
        Course foundCourse = new Course(courseName, courseNumber, department, courseId);
        studentCourses.Add(foundCourse);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return studentCourses;
    }

    public List<Course> GetCompletedCourses()
    {
      List<Course> studentCourses = new List<Course> {};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT courses.* FROM students JOIN schedules ON (students.id = schedules.student_id) JOIN courses ON (schedules.course_id = courses.id) WHERE students.id = @studentId AND course_completion = @courseCompletion;";

      MySqlParameter studentId = new MySqlParameter();
      studentId.ParameterName = "@studentId";
      studentId.Value = _id;
      cmd.Parameters.Add(studentId);

      MySqlParameter courseCompletion = new MySqlParameter();
      courseCompletion.ParameterName = "@courseCompletion";
      courseCompletion.Value = true;
      cmd.Parameters.Add(courseCompletion);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);
        int department = rdr.GetInt32(3);
        Course foundCourse = new Course(courseName, courseNumber, department, courseId);
        studentCourses.Add(foundCourse);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return studentCourses;
    }

    public void CourseCompleted(Course completedCourse)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE schedules SET course_completion = @courseCompletion WHERE course_id = @courseId AND student_id = @studentId;";

      MySqlParameter courseId = new MySqlParameter();
      courseId.ParameterName = "@courseId";
      courseId.Value = completedCourse.GetId();
      cmd.Parameters.Add(courseId);

      MySqlParameter studentId = new MySqlParameter();
      studentId.ParameterName = "@studentId";
      studentId.Value = _id;
      cmd.Parameters.Add(studentId);

      MySqlParameter course = new MySqlParameter();
      course.ParameterName = "@courseCompletion";
      course.Value = true;
      cmd.Parameters.Add(course);

      cmd.ExecuteNonQuery();
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM students; DELETE FROM schedules;";

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
