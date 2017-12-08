using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.esriSystem;

using Newtonsoft.Json;

namespace UGIS
{
    /// <summary>
    /// Provides functionality to publish ArcGIS Map Service to uGIS.
    /// </summary>
    public class UGISPublisher
    {
        /// <summary>
        /// Creates an iinstance of UGISPublisher class.
        /// </summary>
        /// <param name="ags">ArcGIS Map Service object</param>
        public UGISPublisher(IGxAGSObject3 ags)
        {
            this.mServerObject = ags.AGSServerObjectName as IAGSServerObjectName3;
            this.mServicePropertySet = this.readPropertySet(this.mServerObject.AGSServerConnectionName.ConnectionProperties);
        }

        #region PROPERTIES

        /// <summary>
        /// ArcGIS Map Service definition
        /// </summary>
        private IAGSServerObjectName3 mServerObject;
        /// <summary>
        /// list of ArcGIS Map Service properties
        /// </summary>
        private List<ServiceProperty> mServicePropertySet;

        /// <summary>
        /// service name
        /// </summary>
        public string Name { get { return this.mServerObject.Name; } }
        /// <summary>
        /// REST URL address
        /// </summary>
        public string RestUrl
        {
            get
            {
                ServiceProperty sp = this.mServicePropertySet.SingleOrDefault(p => { return p.Name.Equals("RestUrl"); });

                if (sp != null)
                    return String.Format("{0}/services/{1}/{2}", sp.Value, this.mServerObject.Name, this.mServerObject.Type);
                else
                    return null;
            }
        }
        /// <summary>
        /// SOAP URL address
        /// </summary>
        public string SoapUrl
        {
            get
            {
                ServiceProperty sp = this.mServicePropertySet.SingleOrDefault(p => { return p.Name.Equals("SoapUrl"); });

                if (sp != null)
                    return String.Format("{0}/services/{1}/{2}", sp.Value, this.mServerObject.Name, this.mServerObject.Type);
                else
                    return null;
            }
        }
        /// <summary>
        /// service property set
        /// </summary>
        public List<ServiceProperty> PropertySet { get { return this.mServicePropertySet; } }

        #endregion


        #region METHODS

        public string Publish()
        {
            UGISConfig ugisConfig = new UGISConfig();

            string requestString = this.makeRequest(this.RestUrl + "?f=pjson");
            ESRIConfig eConfig = JsonConvert.DeserializeObject<ESRIConfig>(requestString);
            // reverse layers
            eConfig.layers.Reverse();

            if (eConfig == null)
                throw new ArgumentException("esri config not found at address: " + this.RestUrl, "ESRIConfig");

            #region READ SPATIAL REFERENCE FROM EPSG.IO

            if (eConfig.spatialReference != null)
            {
                requestString = this.makeRequest("https://epsg.io/?format=json&q=" + eConfig.spatialReference.latestWkid);
                EPSGClass epsg = JsonConvert.DeserializeObject<EPSGClass>(requestString);

                if (epsg.status.Equals("ok") && epsg.number_result == 1)
                {
                    ugisConfig.projections = new List<string[]>();
                    ugisConfig.projections.Add(new string[2] { "EPSG:" + epsg.results[0].code, epsg.results[0].proj4 });
                }
            }

            #endregion

            #region READ LAYERS

            eConfig.layers.Reverse();

            foreach (EsriLayer eLayer in eConfig.layers)
            {
                string layerJsonData = this.makeRequest(String.Format("{0}/{1}/?f=pjson", this.RestUrl, eLayer.id.ToString()));

                if (String.IsNullOrEmpty(layerJsonData))
                    continue;

                EsriLayerData eLayerData = JsonConvert.DeserializeObject<EsriLayerData>(layerJsonData);
                OperationalLayer layer = new OperationalLayer();
                layer.name = eLayerData.name.Replace(" ","");
                layer.title = eLayerData.name;
                layer.layerId = eLayerData.id.ToString();

                ugisConfig.layers.operational.Add(layer);
            }

            #endregion

            return JsonConvert.SerializeObject(ugisConfig);
        }




        private string makeRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            request.Proxy = WebRequest.DefaultWebProxy;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }

            return null;
        }

        private List<ServiceProperty> readPropertySet(IPropertySet props)
        {
            List<ServiceProperty> result = new List<ServiceProperty>();

            object[] nameArray = new object[1];
            object[] valueArray = new object[1];
            props.GetAllProperties(out nameArray[0], out valueArray[0]);
            object[] names = (object[])nameArray[0];
            object[] values = (object[])valueArray[0];
            for (int i = 0; i < props.Count; i++)
            {
                result.Add(new ServiceProperty(names[i].ToString(), values[i].ToString()));
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// Provides access to service properties.
    /// </summary>
    public class ServiceProperty
    {
        /// <summary>
        /// property name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// property value
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Creates an instance of ServiceProperty class.
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public ServiceProperty(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
