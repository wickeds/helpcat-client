using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Script.Serialization;

namespace WickedFramework.Serialization
{
    public static class JsonDeserializer
    {
        static JavaScriptSerializer js = new JavaScriptSerializer();

        public static dynamic Deserialize(string input)
        {
            return DeserializePartial(js.Deserialize<IDictionary<string, object>>(input));
        }

        private static object DeserializePartial(dynamic obj)
        {
            if (obj.GetType().IsArray)
            {
                List<object> dyn = new List<object>();
                foreach (object entry in obj)
                    dyn.Add(DeserializePartial(entry));
                return (IList<dynamic>)dyn;
            }
            else if (obj is IDictionary)
            {
                IDictionary<string, object> dyn = (IDictionary<string, object>)new ExpandoObject();
                foreach (KeyValuePair<string, object> pair in obj)
                    dyn.Add(pair.Key, DeserializePartial(pair.Value));
                return (ExpandoObject)dyn;
            }
            else
            {
                return obj;
            }
        }
    }
}
