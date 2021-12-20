using Newtonsoft.Json.Linq;

namespace Coeus.Results
{
    public class EmptyObject
    {
        public EmptyObject(JObject jObj, string fullPath)
        {
            JObj = jObj;
            FullPath = fullPath;
        }

        public JObject JObj { get; private set; }
        public string FullPath { get; private set; }
    }
}
