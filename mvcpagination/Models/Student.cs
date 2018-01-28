using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvcpagination.Models
{

    public class Student
    {
        public int serialNo { get; set; }
        public string USN { get; set; }
        public string studentName { get; set; }
        public string branch { get; set; }


        public static List<Student> ListOfStudents()
        {
            List<Student> students = new List<Student>();

            string[] branches = { "CSE", "ISE", "ECE", "EEE", "CE" };
                Random rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                students.Add(new Student() { serialNo = i, USN = "2BU10CS40" + i, branch = branches[rnd.Next(1, 5)], studentName = "Name" + i });
            }

            return students;
        }

    }

}