using System.Windows;
using ESRI.ArcGIS.Client;
using System.Net;
using System.IO;
using System.Collections.Generic;
using ESRI.ArcGIS.Client.Tasks;
using System.Web.Script.Serialization;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace FeatureSplitting
{

    public partial class MainWindow : Window
    {
        private const string MAPSERVER_URL = "REPLACE/WITH/MAPSERVER/0";
        private const string TYPE_URL = "REPLACE/WITH/QUERY/FOR/TYPE/URL&f=pjson";
        private const string COLLECTION_NAME = "TypeCheckBoxCollection";

        public MainWindow()
        {
            // License setting and ArcGIS Runtime initialization is done in Application.xaml.cs.

            InitializeComponent();

            List<string> featureTypes = RequestFeatures();

            SetupFeatureList(featureTypes);

            _map.ExtentChanged += _map_ExtentChanged;
        }

        /* Performs a GET web request on a predefinied url, parses the json, and returns a list of available types*/
        List<string> RequestFeatures()
        {
            List<string> featureTypes = new List<string>();

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(TYPE_URL);
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            StreamReader stream = new StreamReader(webresponse.GetResponseStream());
            string json = stream.ReadToEnd();
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Response response = serializer.Deserialize<Response>(json);
                        
            foreach (Feature feature in response.features)
            {
                featureTypes.Add(feature.attributes.type);
            }

            return featureTypes;
        }

        private void SetupFeatureList(List<string> featureTypes)
        {
            ObservableCollection<TypeCheckBox> collection = new ObservableCollection<TypeCheckBox>();

            foreach (string type in featureTypes)
            {
                collection.Add(new TypeCheckBox() { Name = type, IsSelected = true });
            }

            Resources[COLLECTION_NAME] = collection;
        }

        private void _map_ExtentChanged(object sender, ESRI.ArcGIS.Client.ExtentEventArgs e)
        {
            UpdateFeatures();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            UpdateFeatures();
        }

        private void UpdateFeatures()
        {
            GraphicsLayer graphicsLayer = _map.Layers["_graphicsLayer"] as GraphicsLayer;
            graphicsLayer.ClearGraphics();

            /*Query for Features*/
            QueryTask queryTask = new QueryTask();
            queryTask.Url = MAPSERVER_URL;

            Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.OutSpatialReference = _map.SpatialReference;
            query.Geometry = _map.Extent;
            query.ReturnGeometry = true;
            query.OutFields.Add("*");

            ObservableCollection<TypeCheckBox> collection = Resources[COLLECTION_NAME] as ObservableCollection<TypeCheckBox>;

            foreach (TypeCheckBox cb in collection)
            {
                if (cb.IsSelected)
                {
                    query.Where = "type = '" + cb.Name + "'";
                    FeatureSet featureSet = queryTask.Execute(query);
                    DrawFeatures(graphicsLayer, featureSet);
                }
            }
        }

        private void DrawFeatures(GraphicsLayer graphicsLayer, FeatureSet featureSet)
        {
            for (int i = 0; i < featureSet.Features.Count; i++)
            {
                // REPLACE THIS WITH PROPER IMAGE
                featureSet.Features[i].Symbol = new SimpleMarkerSymbol()
                {
                    Color = System.Windows.Media.Brushes.Red,
                    Size = 20
                };

                graphicsLayer.Graphics.Add(featureSet.Features[i]);
            }
        }
                
    }
}
