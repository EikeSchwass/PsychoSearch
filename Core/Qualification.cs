using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core
{
    public class Qualification
    {
        [XmlAttribute]
        public string Category { get; set; }
        public List<string> Content { get; set; }
    }
}