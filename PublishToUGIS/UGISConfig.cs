using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UGIS
{
    public class UGISConfig
    {
        public List<string[]> projections { get; set; }
        public MapConfig map { get; set; }
        public List<string> widgets { get; set; }
        public LayerDefinitions layers { get; set; }

        public UGISConfig()
        {
            this.map = new MapConfig();
            this.layers = new LayerDefinitions();
        }
    }

    public class MapConfig
    {
        public string baseUrl { get; set; }
        public bool flashOnClicks { get; set; }
        public ViewConfig view { get; set; }

        public MapConfig()
        {
            this.view = new ViewConfig();
        }
    }

    public class ViewConfig
    {
        public string projection { get; set; }
        public int zoomLevel { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class LayerDefinitions
    {
        public List<OperationalLayer> operational { get; set; }
        public List<BasemapLayer> basemaps { get; set; }

        public LayerDefinitions()
        {
            this.basemaps = new List<BasemapLayer>();
            this.operational = new List<OperationalLayer>();
        }
    }

    public class BasemapLayer
    {
        public string name { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string url { get; set; }
        public double minScale { get; set; }
        public double maxScale { get; set; }
        public bool visible { get; set; }
    }

    public class OperationalLayer
    {
        public string name { get; set; }
        public string title { get; set; }

        public string type { get { return "vector"; } }
        public string kindOf { get { return "esri"; } }
        public string layerId { get; set; }
        public string sourceTable { get; set; }

        public double minScale { get; set; }
        public double maxScale { get; set; }

        public bool visible { get; set; }

        public bool searchable { get; set; }
        public bool selectable { get; set; }
        public bool editable { get; set; }
        public bool snappable { get; set; }

        public bool displayPopupsOnHover { get; set; }
        public bool displayPopupsOnClick { get; set; }
        public string displayPopupsMask { get; set; }

        public List<string> styles { get; set; }

        public OperationalLayer() { this.styles = new List<string>(); }
    }
}
