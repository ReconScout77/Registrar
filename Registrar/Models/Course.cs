using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Registrar.Models
{
  public class Course
  {
    private int _id;
    private string _name;
    private string _courseNumber;
    private int _departmentId;

    public Course(string name, string courseNumber, int departmentId, int id=0)
    {
      _id = id;
      _name = name;
      _courseNumber = courseNumber;
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

    public string GetCourseNumber()
    {
      return _courseNumber;
    }

    public int GetDepartmentId()
    {
      return _departmentId;
    }

    public string GetDetails()
    {
      return "ID: " +_id + ", Name: " + _name + ", Course Number: " + _courseNumber + ", Department ID: " + _departmentId;
    }

    public override bool Equals(System.Object otherCourse)
    {
      if(!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;
        bool idEquality = this._id == newCourse.GetId();
        bool nameEquality = this._name == newCourse.GetName();
        bool courseEquality = this._courseNumber == newCourse.GetCourseNumber();
        bool departmentEquality = this._departmentId == newCourse.GetDepartmentId();

        return (idEquality && nameEquality && courseEquality && departmentEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Course> GetAll()
    {
      List<Course> allCourses = new List<Course> {};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM courses;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);
        int departmentId = rdr.GetInt32(3);

        Course newCourse = new Course(name, courseNumber, departmentId, id);
        allCourses.Add(newCourse);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCourses;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO courses (name, number, department_id) VALUES (@name, @number, @departmentId);";

      MySqlParameter courseName = new MySqlParameter();
      courseName.ParameterName = "@name";
      courseName.Value = _name;
      cmd.Parameters.Add(courseName);

      MySqlParameter courseNumber = new MySqlParameter();
      courseNumber.ParameterName = "@number";
      courseNumber.Value = _courseNumber;
      cmd.Parameters.Add(courseNumber);

      MySqlParameter departmentId = new MySqlParameter();
      departmentId.ParameterName = "@departmentId";
      departmentId.Value = _departmentId;
      cmd.Parameters.Add(departmentId);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
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
      cmd.CommandText = @"DELETE FROM courses; DELETE FROM schedules;";

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
