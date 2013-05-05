using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileTransform
{
    [XmlRoot("school")]
    public class XMLSchool
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlElement("grade")]
        public Grade[] Grades { get; set; }

        public XMLSchool()
        {
            Name = @"WGen School";
            Id = @"100";
            Grades = new Grade[0]; // In case the XML object doesn't contain any grades
        }
    }

    public class Grade
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlElement("classroom")]
        public ClassRoom[] ClassRooms { get; set; }

        public Grade()
        {
            ClassRooms = new ClassRoom[0]; // In case the XML object doesn't contain any classrooms
        }
    }

    public class ClassRoom
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlElement("teacher")]
        public Teacher[] Teachers { get; set; }
        [XmlElement("student")]
        public Student[] Students { get; set; }

        public ClassRoom()
        {
            Teachers = new Teacher[0]; // In case the XML object doesn't contain any teachers
            Students = new Student[0]; // In case the XML object doesn't contain any students
        }
    }

    public class Teacher
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("first_name")]
        public string FirstName { get; set; }
        [XmlAttribute("last_name")]
        public string LastName { get; set; }
    }

    public class Student
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("first_name")]
        public string FirstName { get; set; }
        [XmlAttribute("last_name")]
        public string LastName { get; set; }
    }
}
