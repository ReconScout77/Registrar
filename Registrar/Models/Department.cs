using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Registrar.Models
{
  public class Department
  {
    private int _id;
    private string _name;

    public Department(string name, int id = 0)
    {
      _id = id;
      _name = name;
    }

    public int GetId()
    {
      return _id;
    }

    public string GetName()
    {
      return _name;
    }

    public override bool Equals(System.Object otherDepartment)
    {
      if(!(otherDepartment is Department))
      {
        return false;
      }
      else
      {
        Department newDepartment = (Department) otherDepartment;
        bool idEquality = this._id == newDepartment.GetId();
        bool nameEquality = this._name == newDepartment.GetName();

        return (idEquality && nameEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO departments (name) VALUES (@name);";

      MySqlParameter department = new MySqlParameter();
      department.ParameterName = "@name";
      department.Value = _name;
      cmd.Parameters.Add(department);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();

      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public void AddStudent(Student newStudent)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE students SET department_id = @departmentId WHERE id = @studentId;";

      MySqlParameter departmentId = new MySqlParameter();
      departmentId.ParameterName = "@departmentId";
      departmentId.Value = this._id;
      cmd.Parameters.Add(departmentId);

      MySqlParameter studentId = new MySqlParameter();
      studentId.ParameterName = "@studentId";
      studentId.Value = newStudent.GetId();
      cmd.Parameters.Add(studentId);

      newStudent.SetDepartmentId(_id);
      cmd.ExecuteNonQuery();
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Student> GetStudents()
    {
      List<Student> departmentStudents = new List<Student> {};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM students WHERE department_id = @departmentId;";

      MySqlParameter departmentId = new MySqlParameter();
      departmentId.ParameterName = "@departmentId";
      departmentId.Value = _id;
      cmd.Parameters.Add(departmentId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string enrollmentDate = rdr.GetString(2);
        int department = rdr.GetInt32(3);


        Student student = new Student(name, enrollmentDate,  department, id);
        departmentStudents.Add(student);
      }

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return departmentStudents;
    }

    public List<Course> GetCourses()
    {
      List<Course> departmentCourses = new List<Course>{};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM courses WHERE department_id = @departmentId;";

      MySqlParameter departmentId = new MySqlParameter();
      departmentId.ParameterName = "@departmentId";
      departmentId.Value = _id;
      cmd.Parameters.Add(departmentId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);
        int department = rdr.GetInt32(3);
        Course newCourse = new Course(name, courseNumber, department, id);
        departmentCourses.Add(newCourse);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return departmentCourses;
    }

    public static List<Department> GetAll()
    {
      List<Department> allDepartments = new List<Department> {};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM departments;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        Department foundDepartment = new Department(name, id);
        allDepartments.Add(foundDepartment);
      }
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return allDepartments;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM departments;";

      cmd.ExecuteNonQuery();
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
