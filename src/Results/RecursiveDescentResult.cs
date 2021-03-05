using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Coeus.Results
{
    public class RecursiveDescentResult : ParserResult
    {
        public RecursiveDescentResult()
        {
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            IEnumerable<JToken> GetValues(JToken jtok)
            {
                var values = new List<JToken>();

                values.Add(jtok);

                if (jtok is JObject jobj)
                {
                    foreach (var v in jobj.Properties().Select(prop => prop.Value))
                    {
                        values.AddRange(GetValues(v));
                    }
                }
                else if (jtok is JArray array)
                {
                    foreach (var v in array.Select(item => item))
                    {
                        values.AddRange(GetValues(v));
                    }
                }

                return values;
            }

            return GetValues(token);
        }
    }
}
