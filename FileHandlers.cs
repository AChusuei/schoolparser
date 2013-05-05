using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FileHelpers;
using AutoMapper;

namespace FileTransform
{
    // FileHandlers are designed to encapsulate all the functionality needed to transform a file.
    public interface IFileHandler
    {
        /// <summary>
        /// Reads in and parses information from a file appropriate to this data format.
        /// </summary>
        /// <param name="fileName"></param>
        void ReadFile(string fileName);
        /// <summary>
        /// Serializes data from the internal data store to this specific data format.
        /// </summary>
        /// <param name="fileName"></param>
        void WriteFile(string fileName);
        /// <summary>
        /// Determines precisely how this FileHandler maps its information to other FileHandlers
        /// Note that this store must know how to interface with other file handler stores.
        /// If a new file handler type were to be introduced, it is assumed that this method would be updated as to include mapping to new file handlers. 
        /// </summary>
        /// <param name="filehandler"></param>
        void MapStoreOnto(IFileHandler filehandler);
        /// <summary>
        /// Indicates the type of file this file handler uses.
        /// </summary>
        string FileType { get; }
    }

    public class XMLFileHandler : IFileHandler
    {
        public XMLSchool Store { get; set; }
        private XmlSerializer _serializer;
        private XmlSerializerNamespaces _ns;

        public XMLFileHandler()
        {
            Store = new XMLSchool();
            // Please refer to the XMLDataStuctures class file to see how the xml tree is constructed.
            // Once an object representing the tree is properly constructed, .NET XMLSerializers map the information from the xml to the object accordingly.
            _serializer = new XmlSerializer(typeof(XMLSchool));
            _ns = new XmlSerializerNamespaces();
            _ns.Add(string.Empty, string.Empty);
        }

        public void ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                Store = (XMLSchool)_serializer.Deserialize(reader);
            }
        }

        public void WriteFile(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                _serializer.Serialize(writer, Store, _ns);
            }
        }

        public void MapStoreOnto(IFileHandler filehandler)
        {
            // The mapping used here is created from AddMappings() in the FileHandlerFactory
            if (filehandler.GetType() == typeof(CSVFileHandler))
                Mapper.Map(Store, ((CSVFileHandler)filehandler).Store);
        }

        public string FileType { get { return @"XML"; } }
    }

    public class CSVFileHandler : IFileHandler
    {
        public CSVSchool Store { get; set; }
        private bool _hasHeader;
        // I'm using the FileHelpers library to read and write CSV files.
        // The file format is described in the CSVDataStuctures class file.
        private FileHelperEngine<CSVRow> _serializer = new FileHelperEngine<CSVRow>();

        public CSVFileHandler(bool hasHeader = true)
        {
            Store = new CSVSchool();
            _hasHeader = hasHeader;
            _serializer.HeaderText = @"classroom id, classroom_name, teacher_1_id, teacher_1_last_name, teacher_1_first_name, teacher_2_id, teacher_2_last_name, teacher_2_first_name, student_id, student_last_name, student_first_name, student_grade";
        }

        public void ReadFile(string fileName)
        {
            var entries = _serializer.ReadFile(fileName);
            if (_hasHeader) entries = entries.ToList().Skip(1).ToArray();
            Store.Rows = entries.ToArray();
        }

        public void WriteFile(string fileName)
        {
            _serializer.WriteFile(fileName, Store.Rows);
        }

        public void MapStoreOnto(IFileHandler filehandler)
        {
            // The mapping used here is created from AddMappings() in the FileHandlerFactory
            if (filehandler.GetType() == typeof(XMLFileHandler))
                Mapper.Map(Store, ((XMLFileHandler)filehandler).Store);
        }

        public string FileType { get { return @"CSV"; } }
    }
}
