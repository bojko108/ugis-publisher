using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UGIS
{
    public class ESRIConfig
    {
        public List<EsriLayer> layers { get; set; }
        public List<EsriTable> tables { get; set; }
        public SpatialReference spatialReference { get; set; }
    }

    public class EsriLayer
    {
        public int id { get; set; }
        public int parentLayerId { get; set; }
        public bool defaultVisibility { get; set; }
        //public int subLayerIds { get; set; }
        public double minScale { get; set; }
        public double maxScale { get; set; }
    }

    public class EsriTable
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class SpatialReference
    {
        public int wkid { get; set; }
        public int latestWkid { get; set; }
    }

    public class EsriLayerData
    {
        public int id { get; set; }
        public string name { get; set; }
        public double minScale { get; set; }
        public double maxScale { get; set; }
    }
}
