using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureSplitting
{
    public class FieldAliases
    {
        public string type { get; set; }
    }

    public class Field
    {
        public string name { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public int length { get; set; }
    }

    public class Attributes
    {
        public string type { get; set; }
    }

    public class Feature
    {
        public Attributes attributes { get; set; }
    }

    public class Response
    {
        public string displayFieldName { get; set; }
        public FieldAliases fieldAliases { get; set; }
        public List<Field> fields { get; set; }
        public List<Feature> features { get; set; }
    }

    public class ResponsePrinter
    {
        public static string printResponse(Response response)
        {
            string ret = "displayFieldName: " + response.displayFieldName + "\nfieldAliases.type: " + response.fieldAliases.type;

            foreach (Field field in response.fields)
            {
                ret += "\nfields.name: " + field.name;
                ret += "\nfields.type: " + field.type;
                ret += "\nfields.alias: " + field.alias;
                ret += "\nfields.length: " + field.length;
            }

            foreach (Feature feat in response.features)
            {
                ret += "\nfeature.attributes.type: " + feat.attributes.type;
            }

            return ret;
        }
    }
}
