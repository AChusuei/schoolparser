using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace FileTransform
{
    /// <summary>
    /// I decided to do a simple factory pattern, as one might want to change the factory that produces file handlers.
    /// </summary>
    public interface IFileHandlerFactory
    {
        IFileHandler GetFileHandler(string extension);
        void AddMappings();
    }

    public class FileHandlerFactory : IFileHandlerFactory
    {
        private Dictionary<string, Func<IFileHandler>> _handlerTypes;

        public FileHandlerFactory()
        {
            // This is where one configures what kind of file handlers this factory can produce. 
            // One could do this via the config file.
            _handlerTypes = new Dictionary<string, Func<IFileHandler>>();
            _handlerTypes[".xml"] = () => new XMLFileHandler();
            _handlerTypes[".csv"] = () => new CSVFileHandler();
            AddMappings();
        }

        public IFileHandler GetFileHandler(string extension)
        {
            if (_handlerTypes.ContainsKey(extension.ToLowerInvariant())) return _handlerTypes[extension]();
            return null;
        }

        /// <summary>
        /// It's a gargantuan task trying to flatten XML towards a row and column structure in CSV, and similarly to roll up CSV data into a hierarchical XML format.
        /// To reduce the workload involved, I used the AutoMapper libraries, where one can map all the data from one object to another, 
        /// by stating the relationships between properties in the two objects. .NET allows you to use LINQ functional syntax to do this.
        /// Unfotunately, because functional syntax is pretty terse, the algorithms below are quite hard to follow, but generally speaking:
        /// 1) Going from CSV To XML, data needs to be grouped and rolled up in order to accomodate the hierarchical structure of the XML schema.
        /// 2) Going from XML to CSV, we need to denormalize the XML DOM, which flattens the schema for the CSV file structure.
        /// </summary>
        public void AddMappings()
        {
            // Adding mappings between store objects
            Mapper.CreateMap<CSVSchool, XMLSchool>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Grades, opt => opt.MapFrom(src => src.Rows
                    .GroupBy(e => e.StudentGrade, (gid, ggr) => new Grade
                    {
                        Id = gid,
                        ClassRooms = ggr
                            .GroupBy(ge => new { CId = ge.ClassRoomId, CName = ge.ClassRoomName }, (cid, cgr) =>
                            new ClassRoom
                            {
                                Id = cid.CId,
                                Name = cid.CName,
                                Students = cgr.Select(se => new Student
                                {
                                    Id = se.StudentId,
                                    FirstName = se.StudentFirstName,
                                    LastName = se.StudentLastName
                                }).Where(s => !String.IsNullOrWhiteSpace(s.Id)).ToArray(), // removes empty students
                                Teachers = cgr.GroupBy(te => new
                                {
                                    T1Id = te.Teacher1Id,
                                    T1FN = te.Teacher1FirstName,
                                    T1LN = te.Teacher1LastName,
                                    T2Id = te.Teacher2Id,
                                    T2FN = te.Teacher2FirstName,
                                    T2LN = te.Teacher2LastName
                                },
                                    (to, tgr) => (new[] { new Teacher { Id = to.T1Id, FirstName = to.T1FN, LastName = to.T1LN }, 
                                            new Teacher { Id = to.T2Id, FirstName = to.T2FN, LastName = to.T2LN } })
                                        .Where(t => !String.IsNullOrWhiteSpace(t.Id)).ToArray()).First()
                            }) // removes empty teachers
                            .ToArray()
                    }).ToArray()))
                ;



            Mapper.CreateMap<XMLSchool, CSVSchool>()
                 .ForMember(dest => dest.Rows, opt => opt.MapFrom(school => school.Grades.SelectMany(g => g.ClassRooms, (g, cr) => new { GradeId = g.Id, ClassRoom = cr })
                                    .SelectMany(gcr => gcr.ClassRoom.Students ?? new[] { new Student() }, (gcr, s) => new { GradeId = gcr.GradeId, ClassRoom = gcr.ClassRoom, Teacher1 = gcr.ClassRoom.Teachers.FirstOrDefault() ?? new Teacher(), Teacher2 = gcr.ClassRoom.Teachers.ElementAtOrDefault(1) ?? new Teacher(), Student = s })
                                    .Select(so => new CSVRow
                                    {
                                        ClassRoomId = so.ClassRoom.Id,
                                        ClassRoomName = so.ClassRoom.Name,
                                        StudentGrade = so.GradeId,
                                        StudentId = so.Student.Id,
                                        StudentFirstName = so.Student.FirstName,
                                        StudentLastName = so.Student.LastName,
                                        Teacher1Id = so.Teacher1.Id,
                                        Teacher1FirstName = so.Teacher1.FirstName,
                                        Teacher1LastName = so.Teacher1.LastName,
                                        Teacher2Id = so.Teacher2.Id,
                                        Teacher2FirstName = so.Teacher2.FirstName,
                                        Teacher2LastName = so.Teacher2.LastName
                                    })
                                    .ToArray()))
                 ;

            // Adding mappings between handler objects, but these don't seem to be working.
            Mapper.CreateMap<CSVFileHandler, XMLFileHandler>()
                ;

            Mapper.CreateMap<XMLFileHandler, CSVFileHandler>()
                ;

            // Mapper.AssertConfigurationIsValid();
        }
    }
}
