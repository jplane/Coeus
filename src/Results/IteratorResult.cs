using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coeus.Results
{
    public class IteratorResult : ParserResult
    {
        public IteratorResult()
        {
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            if (token is JArray array)
            {
                return array.Select(item => item);
            }
            else if (token is JObject jobj)
            {
                return jobj.Properties().Select(prop => prop.Value);
            }
            else
            {
                throw new InvalidOperationException("Expecting JArray or JObject.");
            }
        }
    }
}
