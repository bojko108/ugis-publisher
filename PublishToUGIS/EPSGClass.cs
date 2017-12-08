using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UGIS
{
    public class EPSGClass
    {
        public string status { get; set; }
        public int number_result { get; set; }
        public List<EPSGDefinition> results { get; set; }
    }

    public class EPSGDefinition
    {
        public string code { get; set; }
        public string proj4 { get; set; }
    }
}
