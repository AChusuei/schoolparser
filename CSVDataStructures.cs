using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileTransform
{
    public class CSVSchool
    {
        public CSVSchool() { }
        public CSVRow[] Rows { get; set; }
    }

    [DelimitedRecord(",")]
    public class CSVRow
    {
        [FieldTrim(TrimMode.Both)]
        public string ClassRoomId;

        [FieldTrim(TrimMode.Both)]
        public String ClassRoomName;

        [FieldTrim(TrimMode.Both)]
        public String Teacher1Id;

        [FieldTrim(TrimMode.Both)]
        public String Teacher1LastName;

        [FieldTrim(TrimMode.Both)]
        public String Teacher1FirstName;

        [FieldTrim(TrimMode.Both)]
        public String Teacher2Id;

        [FieldTrim(TrimMode.Both)]
        public String Teacher2LastName;

        [FieldTrim(TrimMode.Both)]
        public String Teacher2FirstName;

        [FieldTrim(TrimMode.Both)]
        public String StudentId;

        [FieldTrim(TrimMode.Both)]
        public String StudentLastName;

        [FieldTrim(TrimMode.Both)]
        public String StudentFirstName;

        [FieldTrim(TrimMode.Both)]
        public String StudentGrade;
    }
}
