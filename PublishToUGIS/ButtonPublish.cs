using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

using UGIS;

namespace PublishToUGIS
{
    public class ButtonPublish : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ButtonPublish() { }

        protected override void OnClick()
        {
            try
            {
                IApplication app = (ArcCatalog.Application != null ? ArcCatalog.Application : ArcMap.Application);

                if (app == null)
                    return;

                IGxAGSObject3 ags = (app as IGxApplication).SelectedObject as IGxAGSObject3;

                if (ags == null)
                    return;

                UGISPublisher ugisPublisher = new UGISPublisher(ags);

                string json = ugisPublisher.Publish();

                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "JSON Files (*.json)|*.json";
                sf.FileName = "map-config";

                if (sf.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(File.Open(sf.FileName, FileMode.Create), Encoding.UTF8))
                    {
                        sw.Write(json);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnUpdate()
        {
            Enabled = (ArcCatalog.Application != null || ArcMap.Application != null);
        }
    }
}
