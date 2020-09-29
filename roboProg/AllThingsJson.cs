using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboProg
{
    class AllThingsJson
    {
        public class Rootobject
        {
            public Datashape dataShape { get; set; }
            public Row[] rows { get; set; }
        }

        public class Datashape
        {
            public Fielddefinitions fieldDefinitions { get; set; }
        }

        public class Fielddefinitions
        {
            public Avatar avatar { get; set; }
            public Description description { get; set; }
            public Homemashup homeMashup { get; set; }
            public Issystemobject isSystemObject { get; set; }
            public Name name { get; set; }
            public Tags tags { get; set; }
        }

        public class Avatar
        {
            public string name { get; set; }
            public string description { get; set; }
            public string baseType { get; set; }
            public int ordinal { get; set; }
            public Aspects aspects { get; set; }
        }

        public class Aspects
        {
        }

        public class Description
        {
            public string name { get; set; }
            public string description { get; set; }
            public string baseType { get; set; }
            public int ordinal { get; set; }
            public Aspects1 aspects { get; set; }
        }

        public class Aspects1
        {
        }

        public class Homemashup
        {
            public string name { get; set; }
            public string description { get; set; }
            public string baseType { get; set; }
            public int ordinal { get; set; }
            public Aspects2 aspects { get; set; }
        }

        public class Aspects2
        {
        }

        public class Issystemobject
        {
            public string name { get; set; }
            public string description { get; set; }
            public string baseType { get; set; }
            public int ordinal { get; set; }
            public Aspects3 aspects { get; set; }
        }

        public class Aspects3
        {
        }

        public class Name
        {
            public string name { get; set; }
            public string description { get; set; }
            public string baseType { get; set; }
            public int ordinal { get; set; }
            public Aspects4 aspects { get; set; }
        }

        public class Aspects4
        {
            public bool isPrimaryKey { get; set; }
        }

        public class Tags
        {
            public string name { get; set; }
            public string description { get; set; }
            public string baseType { get; set; }
            public int ordinal { get; set; }
            public Aspects5 aspects { get; set; }
        }

        public class Aspects5
        {
            public string tagType { get; set; }
        }

        public class Row
        {
            public string avatar { get; set; }
            public string description { get; set; }
            public string homeMashup { get; set; }
            public bool isSystemObject { get; set; }
            public string name { get; set; }
            public Tag[] tags { get; set; }

            override public string ToString() { return name; }
        }

        public class Tag
        {
            public string vocabulary { get; set; }
            public string vocabularyTerm { get; set; }
        }
    }
}
